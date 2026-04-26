namespace LearningPlatform.API.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Level { get; set; } = "Beginner"; // Beginner | Intermediate | Advanced
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK - Instructor (Many-to-One)
    public int InstructorId { get; set; }
    public User Instructor { get; set; } = null!;

    // Navigation
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
