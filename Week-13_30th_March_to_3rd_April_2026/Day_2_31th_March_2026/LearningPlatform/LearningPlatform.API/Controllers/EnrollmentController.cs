using System.Security.Claims;
using AutoMapper;
using LearningPlatform.API.Data;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/v1/enroll")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<EnrollmentController> _logger;

    public EnrollmentController(AppDbContext db, IMapper mapper, ILogger<EnrollmentController> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>POST /api/v1/enroll — Student / Admin</summary>
    [HttpPost]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> Enroll([FromBody] EnrollDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var course = await _db.Courses.FindAsync(dto.CourseId);
        if (course == null)
            return NotFound(new { error = "Course not found" });

        if (!course.IsPublished)
            return BadRequest(new { error = "Cannot enroll in an unpublished course" });

        var existing = await _db.Enrollments
            .AnyAsync(e => e.UserId == userId && e.CourseId == dto.CourseId);

        if (existing)
            return Conflict(new { error = "Already enrolled in this course" });

        var enrollment = new Enrollment { UserId = userId, CourseId = dto.CourseId };
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();

        _logger.LogInformation("User {UserId} enrolled in course {CourseId}", userId, dto.CourseId);
        return Ok(new { message = "Enrolled successfully", enrollmentId = enrollment.Id });
    }

    /// <summary>GET /api/v1/enroll/my — Current user's enrollments</summary>
    [HttpGet("my")]
    public async Task<IActionResult> MyEnrollments()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enrollments = await _db.Enrollments
            .Include(e => e.Course)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        return Ok(_mapper.Map<List<EnrollmentDto>>(enrollments));
    }

    /// <summary>DELETE /api/v1/enroll/{courseId} — Unenroll</summary>
    [HttpDelete("{courseId:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> Unenroll(int courseId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null)
            return NotFound(new { error = "Enrollment not found" });

        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Unenrolled successfully" });
    }
}
