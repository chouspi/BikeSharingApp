using Microsoft.AspNetCore.Mvc;
using Web.Dtos;
using Web.Repositories;
using Web.Services;
namespace Web.Controllers
{
    public class StationsController : Controller
    {
        private StationsService stationService;
        public StationsController(StationsService service)
        {
            this.stationService = service;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? sortBy, bool descending = false)
        {
            var stations = await stationService.GetStationGridAsync(sortBy ?? "name", descending);

            return View(stations);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            StationDetailDto? station = await stationService.GetStationDetailAsync(id);

            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

    }
}
