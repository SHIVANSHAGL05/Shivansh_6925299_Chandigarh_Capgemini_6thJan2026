namespace LearningPlatform.API.Models;

public class Enrollment
{
    public int Id { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active"; // Active | Completed | Dropped
    public double ProgressPercent { get; set; } = 0;

    // FK - Many-to-Many (User ↔ Course)
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
