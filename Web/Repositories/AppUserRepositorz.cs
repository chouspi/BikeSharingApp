using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Dtos;
using Web.Models;

namespace Web.Repositories
{
    public class AppUserRepositorz
    {
        private AppDbContext context;

        public AppUserRepositorz(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<ApplicationUser> GetUserByIdAsync(int id)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<bool> EmailAllreadyExist(string email)
        {
            string normalizedEmail = email.Trim().ToLowerInvariant();
            return await context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task AddUser(ApplicationUser user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
        public async Task<UserProfileInfoDto?> GetProfileInfoAsync(int userId)
        {
            return await context.Users.Where(user => user.Id == userId).Select(user => new UserProfileInfoDto
                {
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    TotalRentals = user.Rentals.Count(),
                    ActiveRentals = user.Rentals.Count(rental => rental.EndedAt == null),
                    FinishedRentals = user.Rentals.Count(rental => rental.EndedAt != null)
                }).FirstOrDefaultAsync();
        }
        public async Task<List<RecentRentalDto>> GetRecentUserRentals(int userId,int count)
        {
            return await context.Rentals.Where(rental => rental.UserId == userId).OrderByDescending(Rental => Rental.StartedAt).Take(count).Select(rental => new RecentRentalDto
            {
                BikeCode = rental.Bike.Code,
                StartStation = rental.StartStation.Name,
                EndStation = rental.EndStation == null ? null : rental.EndStation.Name,
                StartedAt = rental.StartedAt,
                EndedAt = rental.EndedAt,
                DurationMinutes = rental.DurationMinutes,
                Price = rental.Price,
                Status = rental.EndedAt == null ? "Aktivni" : "Ukonceno"
            }

                ).ToListAsync();
        }
        public async Task<int> GetUserIDFromEmailAsync(string mail)
        {
            if (await EmailAllreadyExist(mail) == false)
                return -1; //chyba
            return await context.Users.Where(u => u.Email == mail).Select(user => user.Id).FirstOrDefaultAsync();
        }
    
    }
}
