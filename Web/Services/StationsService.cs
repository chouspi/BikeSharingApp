using System.Globalization;
using Web.Dtos;
using Web.Repositories;

namespace Web.Services
{
    public class StationsService
    {
        private StationRepository repository;
        public StationsService(StationRepository repository)
        {
            this.repository = repository;
        }
        // Nacte grid stanic.
        public async Task<List<StationGridItemDto>> GetStationGridAsync(string sortBy,bool descending)
        {
            return await repository.GetStationGridAsync(sortBy, descending);
        }
        // Nacte detail stanice.
        public async Task<StationDetailDto?> GetStationDetailAsync(int id)
        {
            return await repository.GetStationDetailAsync(id);
        }
        // Nacte statistiku stanic.
        public async Task<List<StationStatisticDto>> GetStationStatisticsAsync()
        {
            return await repository.GetStationStatisticsAsync();
        }
    }
}
