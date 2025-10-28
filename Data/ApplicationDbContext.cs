using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Entities;
namespace InternshipManagement.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Supervisor> Supervisors { get; set; }
    public DbSet<Internship> Internships { get; set; }
    public DbSet<InternshipTask> Tasks { get; set; } 
    public DbSet<WeeklyReport> WeeklyReports { get; set; }
    public DbSet<Evaluation> Evaluations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === User Configuration ===
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Role);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Role).IsRequired();
        });

        // === Student Configuration ===
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.StudentId).IsUnique();
            entity.HasOne(s => s.User)
                  .WithOne(u => u.Student)
                  .HasForeignKey<Student>(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // === Supervisor Configuration ===
        modelBuilder.Entity<Supervisor>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.Type);
            entity.HasOne(s => s.User)
                  .WithOne(u => u.Supervisor)
                  .HasForeignKey<Supervisor>(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // === Internship Configuration ===
        modelBuilder.Entity<Internship>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.Status);
            entity.HasIndex(i => i.StudentId);

            entity.HasOne(i => i.Student)
                  .WithMany(s => s.Internships)
                  .HasForeignKey(i => i.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.SiteSupervisor)
                  .WithMany(s => s.SiteInternships)
                  .HasForeignKey(i => i.SiteSupervisorId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(i => i.AcademicSupervisor)
                  .WithMany(s => s.AcademicInternships)
                  .HasForeignKey(i => i.AcademicSupervisorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // === Task Configuration ===
           modelBuilder.Entity<InternshipTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.DueDate);
            entity.HasIndex(t => t.InternshipId);

            entity.HasOne(t => t.Internship)
                  .WithMany(i => i.Tasks) // يشير إلى Internship.Tasks
                  .HasForeignKey(t => t.InternshipId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Creator)
                  .WithMany(u => u.CreatedTasks) // يشير إلى User.CreatedTasks
                  .HasForeignKey(t => t.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // === WeeklyReport Configuration ===
        modelBuilder.Entity<WeeklyReport>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.HasIndex(w => w.Status);
            entity.HasIndex(w => new { w.InternshipId, w.WeekNumber }).IsUnique();

            entity.HasOne(w => w.Internship)
                  .WithMany(i => i.WeeklyReports)
                  .HasForeignKey(w => w.InternshipId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // === Evaluation Configuration ===
        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.InternshipId);

            entity.HasOne(e => e.Internship)
                  .WithMany(i => i.Evaluations)
                  .HasForeignKey(e => e.InternshipId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Evaluator)
                  .WithMany(u => u.Evaluations)
                  .HasForeignKey(e => e.EvaluatorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // === Message Configuration ===
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.ReceiverId);
            entity.HasIndex(m => m.IsRead);

            entity.HasOne(m => m.Sender)
                  .WithMany(u => u.SentMessages)
                  .HasForeignKey(m => m.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Receiver)
                  .WithMany(u => u.ReceivedMessages)
                  .HasForeignKey(m => m.ReceiverId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // === ActivityLog Configuration ===
        modelBuilder.Entity<Evaluation>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.HasIndex(e => e.InternshipId);
    entity.HasIndex(e => e.EvaluatorId);

    entity.HasOne(e => e.Internship)
          .WithMany(i => i.Evaluations)
          .HasForeignKey(e => e.InternshipId)
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Evaluator)
          .WithMany()
          .HasForeignKey(e => e.EvaluatorId)
          .OnDelete(DeleteBehavior.Restrict);
});
    }
}