using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Models;
using Web.Repositories;

namespace Web.Services
{
    public class RetnalService
    {
        private RentalRepository rentalRepository;
        private StationRepository stationRepository;
        private BikeRepository bikeRepository;
        public RetnalService(RentalRepository rentalRepository, StationRepository stationRepository, BikeRepository bikeRepository)
        {
            this.rentalRepository = rentalRepository;
            this.stationRepository = stationRepository;
        }
        public async Task<bool> CreateRentalAsync(int userId, int bikeId, int stationId)
        {
            Rental rental = new Rental
            {
                UserId = userId,
                BikeId = bikeId,
                StartStationId = stationId,
                StartedAt = DateTime.UtcNow
            };

            await rentalRepository.CreateRental(rental);

            bool bikeChanged = await bikeRepository.ChangeBikeStateToRentedAsync(bikeId, stationId, rental);

            if (!bikeChanged)
            {
                return false;
            }

            return true;
        }
    }
}