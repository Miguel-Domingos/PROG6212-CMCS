using Microsoft.EntityFrameworkCore;
using PROG6212_CMCS.Server.Models;
using BCrypt.Net;

namespace PROG6212_CMCS.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }
        public DbSet<ClaimApproval> ClaimApprovals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- ROLES ----
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", AccessLevel = 1 },
                new Role { RoleId = 2, RoleName = "AcademicManager", AccessLevel = 2 },
                new Role { RoleId = 3, RoleName = "ProgrammeCoordinator", AccessLevel = 3 },
                new Role { RoleId = 4, RoleName = "Lecturer", AccessLevel = 4 }
            );

            // ---- USERS (seed) ----
            var adminHash = BCrypt.Net.BCrypt.HashPassword("admin123");
            var managerHash = BCrypt.Net.BCrypt.HashPassword("manager123");
            var coordinatorHash = BCrypt.Net.BCrypt.HashPassword("coordinator123");
            var lecturerHash = BCrypt.Net.BCrypt.HashPassword("lecturer123");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Name = "System Admin",
                    Email = "admin@cmcs.com",
                    PasswordHash = adminHash,
                    RoleId = 1
                },
                new User
                {
                    UserId = 2,
                    Name = "Academic Manager",
                    Email = "manager@cmcs.com",
                    PasswordHash = managerHash,
                    RoleId = 2
                },
                new User
                {
                    UserId = 3,
                    Name = "Programme Coordinator",
                    Email = "coordinator@cmcs.com",
                    PasswordHash = coordinatorHash,
                    RoleId = 3
                },
                new User
                {
                    UserId = 4,
                    Name = "Lecturer",
                    Email = "lecturer@cmcs.com",
                    PasswordHash = lecturerHash,
                    RoleId = 4
                }
            );

            // ---- RELACIONAMENTOS ----
            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.User)
                .WithOne(u => u.LecturerProfile)
                .HasForeignKey<Lecturer>(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Lecturer)
                .WithMany(l => l.Claims)
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SupportingDocument>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClaimApproval>()
                .HasOne(a => a.Claim)
                .WithMany(c => c.Approvals)
                .HasForeignKey(a => a.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClaimApproval>()
                .HasOne(a => a.Approver)
                .WithMany()
                .HasForeignKey(a => a.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
