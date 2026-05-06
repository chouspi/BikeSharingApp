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
        // Nacte formular zapujceni kola.
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
        // Zkontroluje a ulozi zapujceni.
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
        // Nacte formular vraceni kola.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Return(int rentalId)
        {
            string? userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdText, out int userId))
            {
                return Unauthorized();
            }

            ReturnRentalViewModel? model = await retnalService.GetReturnRentalFormAsync(rentalId, userId);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Vypujcka nejde vratit.";
                return RedirectToAction("Profile", "Account");
            }

            return View(model);
        }
        // Ukonci vypujcku a vrati kolo.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(ReturnRentalViewModel model)
        {
            if (!model.ConfirmReturn)
            {
                ModelState.AddModelError(nameof(model.ConfirmReturn), "Musis potvrdit vraceni.");
            }

            if (model.EndStationId == null)
            {
                ModelState.AddModelError(nameof(model.EndStationId), "Vyber stanici.");
            }

            string? userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdText, out int userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                ReturnRentalViewModel? reloadedModel = await retnalService.GetReturnRentalFormAsync(model.RentalId, userId);

                if (reloadedModel == null)
                {
                    return RedirectToAction("Profile", "Account");
                }

                reloadedModel.EndStationId = model.EndStationId;
                reloadedModel.Note = model.Note;
                reloadedModel.ConfirmReturn = model.ConfirmReturn;

                return View(reloadedModel);
            }

            bool success = await retnalService.ReturnRentalAsync(model.RentalId, userId, model.EndStationId.Value);

            if (!success)
            {
                TempData["ErrorMessage"] = "Kolo se nepodarilo vratit.";
                return RedirectToAction("Profile", "Account");
            }

            TempData["SuccessMessage"] = "Kolo bylo vraceno.";
            return RedirectToAction("Profile", "Account");
        }
    }
}
