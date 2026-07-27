using EduApoyos.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EduApoyos.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            await context.Database.MigrateAsync();

            foreach (var roleName in new[] { "Advisor", "Student" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }

            await EnsureUserAsync(userManager, context,
                "advisor@eduapoyos.com", "Advisor123*", "Carlos Advisor", UserRole.Advisor, null);

            await EnsureUserAsync(userManager, context,
                "student@eduapoyos.com", "Student123*", "Ana Student", UserRole.Student,
                ("1234567890", "CC", "Systems Engineering", 6));
        }

        private static async Task EnsureUserAsync(
            UserManager<IdentityUser<Guid>> userManager,
            ApplicationDbContext context,
            string email, string password, string fullName, UserRole role,
            (string Doc, string DocType, string Program, int Semester)? studentData)
        {
            if (await userManager.FindByEmailAsync(email) is not null) return;

            var identityUser = new IdentityUser<Guid>
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(identityUser, password);
            if (!result.Succeeded) return;

            await userManager.AddToRoleAsync(identityUser, role.ToString());

            var domainUser = new User(fullName, email, identityUser.PasswordHash!, role);
            domainUser.SetId(identityUser.Id);
            context.DomainUsers.Add(domainUser);

            if (studentData is not null)
            {
                context.Students.Add(new Student(
                    identityUser.Id,
                    studentData.Value.Doc,
                    studentData.Value.DocType,
                    studentData.Value.Program,
                    studentData.Value.Semester));
            }

            await context.SaveChangesAsync();
        }
    }
}
