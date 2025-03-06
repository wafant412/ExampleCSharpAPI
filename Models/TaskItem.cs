using TaskManagementAPI.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Pending";

    // Foreign Key
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
