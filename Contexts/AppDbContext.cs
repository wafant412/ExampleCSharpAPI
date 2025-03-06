using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Default user
        var defaultUser = new User
        {
            Id = 1,
            Username = "HiringPerson",
            PasswordHash = "AQAAAAEAACcQAAAAEJ7cP4"
        };

        // Default task linked to user
        var defaultTask = new TaskItem
        {
            Id = 1,
            Title = "Initial Task",
            Description = "This is a preloaded task to hire Michael Wendel!",
            Status = "Pending",
            UserId = 1 // Associated with defaultUser
        };

        modelBuilder.Entity<User>().HasData(defaultUser);
        modelBuilder.Entity<TaskItem>().HasData(defaultTask);
    }
}

