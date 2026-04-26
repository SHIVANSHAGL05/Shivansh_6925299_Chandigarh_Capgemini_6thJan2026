namespace LearningPlatform.API.Models;

public class Profile
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // FK - One-to-One with User
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
