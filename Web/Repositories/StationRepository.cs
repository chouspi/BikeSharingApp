using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using Web.Data;
using Web.Dtos;
using Web.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Web.Repositories
{
    public class StationRepository
    {
        private AppDbContext context;

        public StationRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Station>> GetStationsAsync()
        {
            return await context.Stations.ToListAsync();
        }
        public async Task<Station?> GetStationByIdAsync(int id)
        {
            return await context.Stations.FirstOrDefaultAsync(s => s.Id == id);
        }
        // Vrati stanice serazene v DB.
        public async Task<List<StationGridItemDto>> GetStationGridAsync(string orderBy, bool descending)
        {
            IQueryable<Station> query = context.Stations;
            // Sort prijde z odkazu v tabulce.
            orderBy = orderBy.ToLower().Trim();

            if (orderBy == "name")
            {
                if (descending)
                {
                    query = query.OrderByDescending(station => station.Name);
                }
                else
                {
                    query = query.OrderBy(station => station.Name);
                }
            }
            else if (orderBy == "address")
            {
                if (descending)
                {
                    query = query.OrderByDescending(station => station.Address);
                }
                else
                {
                    query = query.OrderBy(station => station.Address);
                }
            }
            else if (orderBy == "availablebikes")
            {
                if (descending)
                {
                    query = query.OrderByDescending(station =>
                        station.Bikes.Count(bike => bike.IsActive && bike.Status == BikeStatus.Available));
                }
                else
                {
                    query = query.OrderBy(station =>
                        station.Bikes.Count(bike => bike.IsActive && bike.Status == BikeStatus.Available));
                }
            }
            else if (orderBy == "totalbikes")
            {
                if (descending)
                {
                    query = query.OrderByDescending(station => station.Bikes.Count(bike => bike.IsActive));
                }
                else
                {
                    query = query.OrderBy(station => station.Bikes.Count(bike => bike.IsActive));
                }
            }
            else
            {
                query = query.OrderBy(station => station.Name);
            }

            return await query
                .Select(station => new StationGridItemDto
                {
                    Id = station.Id,
                    Name = station.Name,
                    Address = station.Address,
                    Latitude = station.Latitude,
                    Longitude = station.Longitude,
                    AvailableBikes = station.Bikes.Count(bike => bike.IsActive && bike.Status == BikeStatus.Available),
                    TotalBikes = station.Bikes.Count(bike => bike.IsActive)
                })
                .ToListAsync();
        }

        // Vrati detail s dostupnymi koly.
        public async Task<StationDetailDto?> GetStationDetailAsync(int id)
        {
            return await context.Stations.Where(station => station.Id == id).Select(station => new StationDetailDto
            {
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Id = station.Id,
                Name = station.Name,

                TotalBikes = station.Bikes.Count(bike => bike.IsActive),
                AvailableBikes = station.Bikes.Count(bike => bike.IsActive && bike.Status == BikeStatus.Available),

                AvailableBikesList = station.Bikes.Where(bike => bike.IsActive && bike.Status == BikeStatus.Available)
                .OrderBy(bike => bike.Code).Select(bike => new AvailableBikeDto
                {
                    Code = bike.Code,
                    Id = bike.Id,
                    Status = bike.Status.ToString()
                }).ToList()
            }).FirstOrDefaultAsync();
        }
    
        public async Task<Station> GetStationById(int id)
        {
            return await context.Stations.Where(station => station.Id == id).FirstAsync();
        }
        // Pripravi volby stanic do selectu.
        public async Task<List<SelectListItem>> GetTargetStationOptionsAsync()
        {
            return await context.Stations.OrderBy(station => station.Name).Select(station => new SelectListItem
                {
                    Value = station.Id.ToString(),
                    Text = station.Name + " (" + station.Bikes.Count(bike => bike.IsActive) + " kol)"
                }).ToListAsync();
        }
        // Hlida preplnenou stanici.
        public async Task<bool> HasMoreThanThreeBikesAsync(int stationId)
        {
            int bikeCount = await context.Bikes.CountAsync(bike => bike.IsActive && bike.CurrentStationId == stationId);

            return bikeCount > 3;
        }
        // Spocita mesicni pohyby kol.
        public async Task<List<StationStatisticDto>> GetStationStatisticsAsync()
        {
            DateTime from = DateTime.UtcNow.AddMonths(-1);

            return await context.Stations.Select(station => new StationStatisticDto
            {
                StationId = station.Id,
                StationName = station.Name,
                StartedCount = station.StartedRentals.Count(rental => rental.StartedAt >= from),
                FinishedCount = station.FinishedRentals.Count(rental => rental.EndedAt != null && rental.EndedAt >= from)
            }).ToListAsync();
        }
    }
}
