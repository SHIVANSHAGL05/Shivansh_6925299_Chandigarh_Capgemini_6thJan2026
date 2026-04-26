using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.API.DTOs;

// ── Course DTOs ──────────────────────────────────────────────────────────────

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Level { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int LessonCount { get; set; }
    public int EnrollmentCount { get; set; }
}

public class CreateCourseDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(0, 99999.99, ErrorMessage = "Price must be between 0 and 99999.99")]
    public decimal Price { get; set; }

    [AllowedValues("Beginner", "Intermediate", "Advanced")]
    public string Level { get; set; } = "Beginner";
}

public class UpdateCourseDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public decimal? Price { get; set; }
    public string? Level { get; set; }
    public bool? IsPublished { get; set; }
}

// ── Lesson DTOs ──────────────────────────────────────────────────────────────

public class LessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
    public int CourseId { get; set; }
}

public class CreateLessonDto
{
    [Required(ErrorMessage = "Lesson title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;

    public string? VideoUrl { get; set; }

    [Range(1, 1000)]
    public int Order { get; set; } = 1;

    [Range(1, 600)]
    public int DurationMinutes { get; set; } = 10;
}

// ── Enrollment DTOs ──────────────────────────────────────────────────────────

public class EnrollDto
{
    [Required]
    public int CourseId { get; set; }
}

public class EnrollmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public double ProgressPercent { get; set; }
}

// ── User DTOs ────────────────────────────────────────────────────────────────

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ── Pagination ───────────────────────────────────────────────────────────────

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
