using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Web.Data;
using Web.Models;

namespace Web.Repositories;

public class BikeRepository
{
    private  AppDbContext context;

    public BikeRepository(AppDbContext context)
    {
        this.context = context;
    }


    public async Task<bool> ChangeBikeStateToRentedAsync(int bikeId, int stationId, Rental rental)
    {
        Bike? bike = await context.Bikes.FirstOrDefaultAsync(bike =>
                bike.Id == bikeId &&
                bike.CurrentStationId == stationId &&
                bike.IsActive
                );

        if (bike == null)
        {
            return false;
        }

        if (bike.Status != BikeStatus.Available)
        {
            return false;
        }

        bike.Status = BikeStatus.Rented;
        bike.CurrentStationId = null;

        BikeStatusHistory history = new BikeStatusHistory
        {
            BikeId = bikeId,
            NewStatus = BikeStatus.Rented,
            StationId = stationId,
            Rental = rental,
            ChangedAt = DateTime.UtcNow
        };

        context.BikeStatusHistory.Add(history);

        return true;
    }
    public async Task<List<Bike>> GetAllAsync()
    {
        return await context.Bikes.Include(bike => bike.CurrentStation).Where(bike => bike.IsActive).ToListAsync();
    }

    public async Task<List<Bike>> GetAvailableByStationAsync(int stationId)
    {
        return await context.Bikes.Where(bike => bike.IsActive && bike.CurrentStationId == stationId && bike.Status == BikeStatus.Available)
            .OrderBy(bike => bike.Code).ToListAsync();
    }

    public async Task<Bike?> GetByIdAsync(int id)
    {
        return await context.Bikes.Include(bike => bike.CurrentStation).FirstOrDefaultAsync(bike => bike.Id == id && bike.IsActive);
    }

    public async Task<Bike?> GetByCodeAsync(string code)
    {
        return await context.Bikes
            .Include(bike => bike.CurrentStation)
            .FirstOrDefaultAsync(bike => bike.Code == code && bike.IsActive);
    }

    public async Task<bool> CodeExistsAsync(string code, int? ignoredBikeId = null)
    {
        return await context.Bikes.AnyAsync(bike =>
            bike.Code == code && (!ignoredBikeId.HasValue || bike.Id != ignoredBikeId.Value));
    }

    public async Task AddAsync(Bike bike)
    {
        await context.Bikes.AddAsync(bike);
    }

    public void Update(Bike bike)
    {
        context.Bikes.Update(bike);
    }

    public void Remove(Bike bike)
    {
        context.Bikes.Remove(bike);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
