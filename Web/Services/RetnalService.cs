using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Web.Models;
using Web.Repositories;
using Web.ViewModels;

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
            this.bikeRepository = bikeRepository;
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

            bool bikeChanged = await bikeRepository.ChangeBikeStateToRentedAsync(bikeId, stationId, rental);

            if (!bikeChanged)
            {
                return false;
            }

            return await rentalRepository.CreateRental(rental);

        }
        public async Task<CreateRentalViewModel?> GetCreateRentalFormAsync(int bikeId, int stationId)
        {
            Bike? bike = await bikeRepository.GetByIdAsync(bikeId);

            if (bike == null)
            {
                return null;
            }

            if (bike.CurrentStationId != stationId)
            {
                return null;
            }

            if (bike.Status != BikeStatus.Available)
            {
                return null;
            }

            List<SelectListItem> targetStations = await stationRepository.GetTargetStationOptionsAsync();

            CreateRentalViewModel model = new CreateRentalViewModel
            {
                BikeId = bike.Id,
                StationId = stationId,
                BikeCode = bike.Code,
                StationName = bike.CurrentStation == null ? "" : bike.CurrentStation.Name,
                TargetStationOptions = targetStations
            };

            return model;
        }

        public async Task<bool> TargetStationIsValidAsync(int stationId)
        {
            bool hasMoreThanThreeBikes = await stationRepository.HasMoreThanThreeBikesAsync(stationId);

            return !hasMoreThanThreeBikes;
        }
        public async Task<ReturnRentalViewModel?> GetReturnRentalFormAsync(int rentalId,int userId)
        {
            Rental? rental = await rentalRepository.GetRentalForReturnAsync(rentalId, userId);

            if (rental == null)
                return null;

            if (rental.EndedAt != null)
                return null;

            List<SelectListItem> stations = await stationRepository.GetTargetStationOptionsAsync();

            ReturnRentalViewModel model = new ReturnRentalViewModel
            {
                RentalId = rental.Id,
                BikeCode = rental.Bike.Code,
                StartStationName = rental.StartStation.Name,
                StartedAt = rental.StartedAt,
                StationOptions = stations
            };

            return model;
        }
        public async Task<bool> ReturnRentalAsync(int rentalId,int userId,int stationId)
        {
            return await rentalRepository.ReturnRentalAsync(rentalId, userId, stationId);
        }
    }
}
