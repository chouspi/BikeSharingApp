using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Web.Data;
using Web.Models;

namespace Web.Repositories
{
    internal class RentalRepository
    {
        AppDbContext context;
        public RentalRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> CreateRental(Rental rental)
        {
            context.Rentals.Add(rental);
            int succes = await context.SaveChangesAsync();
            if(succes >= 1)
                return true;
            else 
                return false;
        }
        public async Task<List<Rental>> GetAllUserRentals(int userId)
        {
            return await context.Rentals.Where(rental => rental.UserId == userId).ToListAsync();
        }
    }
}