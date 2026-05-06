using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Dtos;
using Web.Models;

namespace Web.Controllers.Api.ApiRepositories
{
    public class ApiUserRepository
    {
        private AppDbContext context;

        public ApiUserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<DesktopUserDto>> ApiGetAllUsersAsync()
        {
            return await context.Users.Select(u => new DesktopUserDto
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CreatedAt = u.CreatedAt,
                Id = u.Id,
            }).ToListAsync();


        }
        public async Task<bool> ApiEmailAllreadyExist(string email)
        {
            string normalizedEmail = email.Trim().ToLower();
            return await context.Users.AnyAsync(u => u.Email == normalizedEmail);
        }

        public async Task<DesktopUserDto> ApiCreateUserAsync(DesktopUserDto user,string password)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                FirstName = user.FirstName.Trim(),
                LastName = user.LastName.Trim(),
                Email = user.Email.Trim().ToLower(),
                CreatedAt = DateTime.UtcNow
            };

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, password);

            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            user.Id = newUser.Id;
            user.CreatedAt = newUser.CreatedAt;

            return user;
        }

        public async Task<bool> ApiEmailUsedByOtherUser(string email, int id)
        {
            string normalizedEmail = email.Trim().ToLower();
            return await context.Users.AnyAsync(u => u.Email == normalizedEmail && u.Id != id);
        }

        public async Task<DesktopUserDto?> ApiUpdateUserAsync(int id, DesktopUserDto user)
        {
            ApplicationUser? oldUser = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (oldUser == null)
            {
                return null;
            }

            oldUser.FirstName = user.FirstName.Trim();
            oldUser.LastName = user.LastName.Trim();
            oldUser.Email = user.Email.Trim().ToLower();

            await context.SaveChangesAsync();

            user.Id = oldUser.Id;
            user.Email = oldUser.Email;
            user.CreatedAt = oldUser.CreatedAt;

            return user;
        }

        public async Task<string> ApiDeleteUserAsync(int id)
        {
            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return "notfound";
            }

            bool hasRentals = await context.Rentals.AnyAsync(r => r.UserId == id);

            if (hasRentals)
            {
                return "rentals";
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return "ok";
        }
    }
}
