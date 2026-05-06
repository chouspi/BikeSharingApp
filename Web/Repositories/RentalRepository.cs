using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Web.Data;
using Web.Models;
using Web.ViewModels;

namespace Web.Repositories
{
    public class RentalRepository
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
        public async Task<Rental?> GetRentalForReturnAsync(int rentalId,int userId)
        {
            return await context.Rentals.Include(rental => rental.Bike).Include(rental => rental.StartStation).Where(rental => rental.Id == rentalId && rental.UserId == userId).FirstOrDefaultAsync();
        }
        public async Task<bool> ReturnRentalAsync(int rentalId,int userId,int stationId)
        {
            Rental? rental = await context.Rentals.Include(rental => rental.Bike).Where(rental => rental.Id == rentalId && rental.UserId == userId).FirstOrDefaultAsync();

            if (rental == null)
                return false;

            if (rental.EndedAt != null)
                return false;

            Station? station = await context.Stations.Where(station => station.Id == stationId && station.IsActive).FirstOrDefaultAsync();

            if (station == null)
                return false;

            if (rental.Bike.Status != BikeStatus.Rented)
                return false;

            DateTime endTime = DateTime.UtcNow;
            int duration = (int)Math.Ceiling((endTime - rental.StartedAt).TotalMinutes);

            if (duration < 1)
                duration = 1;

            rental.EndedAt = endTime;
            rental.EndStationId = stationId;
            rental.DurationMinutes = duration;
            rental.Price = duration * 2;

            rental.Bike.Status = BikeStatus.Available;
            rental.Bike.CurrentStationId = stationId;

            BikeStatusHistory history = new BikeStatusHistory
            {
                BikeId = rental.BikeId,
                NewStatus = BikeStatus.Available,
                StationId = stationId,
                Rental = rental,
                ChangedAt = endTime
            };

            context.BikeStatusHistory.Add(history);

            int succes = await context.SaveChangesAsync();

            if (succes >= 1)
                return true;
            else
                return false;
        }
    }
}
