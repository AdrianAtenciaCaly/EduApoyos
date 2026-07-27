using EduApoyos.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduApoyos.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> DomainUsers => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();
        public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.FullName).HasMaxLength(150).IsRequired();
                e.Property(x => x.Email).HasMaxLength(150).IsRequired();
                e.Property(x => x.PasswordHash).IsRequired();
            });

            builder.Entity<Student>(e =>
            {
                e.ToTable("Students");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.DocumentNumber).IsUnique();
                e.Property(x => x.DocumentNumber).HasMaxLength(20).IsRequired();
                e.Property(x => x.DocumentType).HasMaxLength(10).IsRequired();
                e.Property(x => x.AcademicProgram).HasMaxLength(100).IsRequired();
                e.HasOne(x => x.User).WithOne(x => x.Student)
                    .HasForeignKey<Student>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SupportRequest>(e =>
            {
                e.ToTable("SupportRequests");
                e.HasKey(x => x.Id);
                e.Property(x => x.RequestedAmount).HasPrecision(18, 2);
                e.Property(x => x.Description).HasMaxLength(500).IsRequired();
                e.HasOne(x => x.Student).WithMany(x => x.SupportRequests)
                    .HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Advisor).WithMany()
                    .HasForeignKey(x => x.AdvisorId).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => new { x.Status, x.UpdatedAt })
                    .HasDatabaseName("IX_SupportRequests_Status_UpdatedAt");
            });

            builder.Entity<StatusHistory>(e =>
            {
                e.ToTable("StatusHistories");
                e.HasKey(x => x.Id);
                e.Property(x => x.Observation).HasMaxLength(500);
                e.HasOne(x => x.SupportRequest).WithMany(x => x.StatusHistories)
                    .HasForeignKey(x => x.SupportRequestId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.User).WithMany()
                    .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
