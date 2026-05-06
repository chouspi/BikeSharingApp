using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Dtos;

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
                Rentals = u.Rentals,
            }).ToListAsync();


        }
    }
}
