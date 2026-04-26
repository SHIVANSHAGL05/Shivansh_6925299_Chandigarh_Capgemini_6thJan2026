using System.Security.Claims;
using AutoMapper;
using LearningPlatform.API.Data;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CoursesController> _logger;

    private const string CoursesCacheKey = "all_courses";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public CoursesController(AppDbContext db, IMapper mapper, IMemoryCache cache, ILogger<CoursesController> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>GET /api/v1/courses?page=1&pageSize=10&search=title</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        // Use cache only when no search/pagination filters
        if (search == null && page == 1 && pageSize == 10)
        {
            if (_cache.TryGetValue(CoursesCacheKey, out List<CourseDto>? cached) && cached != null)
            {
                _logger.LogInformation("Returning courses from cache");
                return Ok(cached);
            }
        }

        var query = _db.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .Where(c => c.IsPublished)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Title.Contains(search) || c.Description.Contains(search));

        var totalCount = await query.CountAsync();

        var courses = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<CourseDto>>(courses);

        if (search == null && page == 1 && pageSize == 10)
        {
            _cache.Set(CoursesCacheKey, dtos, CacheDuration);
        }

        var result = new PagedResult<CourseDto>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Ok(result);
    }

    /// <summary>GET /api/v1/courses/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound(new { error = $"Course with id {id} not found" });

        return Ok(_mapper.Map<CourseDto>(course));
    }

    /// <summary>GET /api/v1/courses/category/{name}</summary>
    [HttpGet("category/{name}")]
    public async Task<IActionResult> GetByCategory(string name)
    {
        var courses = await _db.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .Where(c => c.Category.ToLower() == name.ToLower() && c.IsPublished)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(_mapper.Map<List<CourseDto>>(courses));
    }

    /// <summary>POST /api/v1/courses — Instructor only</summary>
    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var course = _mapper.Map<Course>(dto);
        course.InstructorId = userId;

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();

        // Invalidate cache
        _cache.Remove(CoursesCacheKey);
        _logger.LogInformation("Course created: {Title} by user {UserId}", course.Title, userId);

        var result = await _db.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .FirstAsync(c => c.Id == course.Id);

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, _mapper.Map<CourseDto>(result));
    }

    /// <summary>PUT /api/v1/courses/{id}</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto dto)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { error = $"Course with id {id} not found" });

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (course.InstructorId != userId && role != "Admin")
            return Forbid();

        _mapper.Map(dto, course);
        await _db.SaveChangesAsync();

        _cache.Remove(CoursesCacheKey);
        return NoContent();
    }

    /// <summary>DELETE /api/v1/courses/{id} — Admin only</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { error = $"Course with id {id} not found" });

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
        _cache.Remove(CoursesCacheKey);

        return NoContent();
    }

    /// <summary>POST /api/v1/courses/{id}/lessons — Instructor only</summary>
    [HttpPost("{id:int}/lessons")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> AddLesson(int id, [FromBody] CreateLessonDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { error = $"Course with id {id} not found" });

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (course.InstructorId != userId && role != "Admin")
            return Forbid();

        var lesson = _mapper.Map<Lesson>(dto);
        lesson.CourseId = id;

        _db.Lessons.Add(lesson);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id }, _mapper.Map<LessonDto>(lesson));
    }

    /// <summary>GET /api/v1/courses/{id}/lessons</summary>
    [HttpGet("{id:int}/lessons")]
    public async Task<IActionResult> GetLessons(int id)
    {
        var lessons = await _db.Lessons
            .Where(l => l.CourseId == id)
            .OrderBy(l => l.Order)
            .ToListAsync();

        return Ok(_mapper.Map<List<LessonDto>>(lessons));
    }
}
