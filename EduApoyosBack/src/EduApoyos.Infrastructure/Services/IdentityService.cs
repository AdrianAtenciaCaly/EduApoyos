using EduApoyos.Application.Interfaces;
using EduApoyos.Domain.Entities;
using EduApoyos.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduApoyos.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _context;

        public IdentityService(UserManager<IdentityUser<Guid>> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<(bool Succeeded, Guid UserId, string? Error)> CreateUserAsync(
            string email, string password, string fullName, UserRole role, CancellationToken ct = default)
        {
            if (await _userManager.FindByEmailAsync(email) is not null)
                return (false, Guid.Empty, "Email is already registered.");

            var identityUser = new IdentityUser<Guid>
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, password);
            if (!result.Succeeded)
                return (false, Guid.Empty, string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(identityUser, role.ToString());

            var domainUser = new User(fullName, email, identityUser.PasswordHash!, role);
            domainUser.SetId(identityUser.Id);
            _context.DomainUsers.Add(domainUser);
            await _context.SaveChangesAsync(ct);

            return (true, identityUser.Id, null);
        }

        public async Task<(bool Succeeded, User? User)> ValidateCredentialsAsync(
            string email, string password, CancellationToken ct = default)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser is null || !await _userManager.CheckPasswordAsync(identityUser, password))
                return (false, null);

            var user = await _context.DomainUsers.FirstOrDefaultAsync(u => u.Id == identityUser.Id, ct);
            return user is null ? (false, null) : (true, user);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
            => await _context.DomainUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
    }
}
