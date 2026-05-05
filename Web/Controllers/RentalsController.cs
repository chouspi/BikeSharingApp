using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Services;

namespace Web.Controllers
{
    public class RentalsController : Controller
    {
        private RetnalService retnalService;
        public RentalsController(RetnalService retnalService)
        {
            this.retnalService = retnalService;
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bikeId, int stationId)
        {
            string? userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdText, out int userId))
            {
                return Unauthorized();
            }
            bool succes = await retnalService.CreateRentalAsync(userId, bikeId, stationId);

            if (succes)
            {
                return RedirectToAction("Profile", "Account");
            }
            TempData["ErrorMessage"] = "Kolo se nepodarilo vypujcit.";
            return RedirectToAction("Details", "Stations", new { id = stationId });
        }
    }
}
