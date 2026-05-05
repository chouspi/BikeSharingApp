using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class RentalsController : Controller
    {
        private RetnalService retnalService;
        public RentalsController(RetnalService retnalService)
        {
            this.retnalService = retnalService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int bikeId, int stationId)
        {
            CreateRentalViewModel? model = await retnalService.GetCreateRentalFormAsync(bikeId, stationId);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Kolo neni dostupne pro zapujceni.";
                return RedirectToAction("Details", "Stations", new { id = stationId });
            }

            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRentalViewModel model)
        {
            if (!model.AgreeToTerms)
            {
                ModelState.AddModelError(nameof(model.AgreeToTerms), "Musis souhlasit s podminkami.");
            }

            if (model.EstimatedTargetStationId == null)
            {
                ModelState.AddModelError(nameof(model.EstimatedTargetStationId), "Vyber odhadovanou cilovou stanici.");
            }
            else
            {
                bool targetStationIsValid = await retnalService.TargetStationIsValidAsync(model.EstimatedTargetStationId.Value);

                if (!targetStationIsValid)
                {
                    ModelState.AddModelError(
                        nameof(model.EstimatedTargetStationId),
                        "Tato cilova stanice ma vice nez 3 kola. Vyber jinou stanici."
                    );
                }
            }

            if (!ModelState.IsValid)
            {
                CreateRentalViewModel? reloadedModel = await retnalService.GetCreateRentalFormAsync(model.BikeId, model.StationId);

                if (reloadedModel == null)
                {
                    return RedirectToAction("Details", "Stations", new { id = model.StationId });
                }

                reloadedModel.EstimatedTargetStationId = model.EstimatedTargetStationId;
                reloadedModel.AgreeToTerms = model.AgreeToTerms;

                return View(reloadedModel);
            }

            string? userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdText, out int userId))
            {
                return Unauthorized();
            }

            bool success = await retnalService.CreateRentalAsync(userId, model.BikeId, model.StationId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Kolo se nepodarilo zapujcit.";
                return RedirectToAction("Details", "Stations", new { id = model.StationId });
            }

            TempData["SuccessMessage"] = "Kolo bylo uspesne zapujceno.";
            return RedirectToAction("Profile", "Account");
        }
    }
}
