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
        public async Task<List<StationGridItemDto>> GetStationGridAsync(string sortBy,bool descending)
        {
            return await repository.GetStationGridAsync(sortBy, descending);
        }
        public async Task<StationDetailDto?> GetStationDetailAsync(int id)
        {
            return await repository.GetStationDetailAsync(id);
        }
    }
}
