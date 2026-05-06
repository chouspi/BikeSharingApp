using Microsoft.AspNetCore.Mvc;
using Web.Dtos;
using Web.Services;

namespace Web.Controllers
{
    public class StatisticsController : Controller
    {
        private StationsService stationsService;
        public StatisticsController(StationsService stationsService)
        {
            this.stationsService = stationsService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<StationStatisticDto> statistics = await stationsService.GetStationStatisticsAsync();

            return View(statistics);
        }
    }
}
