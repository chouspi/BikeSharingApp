using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;
using System.Security.Cryptography;
using Web.Data;
using Web.Models;
using Web.Repositories;
using Web.Services;

namespace Web.Controllers;

public class AccountController : Controller
{
    private AccountService accountService;
    private PasswordHasher<ApplicationUser> passwordHasher = new();
    public AccountController(AccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var email = model.Email.Trim().ToLower(); 
        bool emailExists = await accountService.EmailExist(email);
        if(!emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Uzivatel s timto emailem neexistuje.");
            return View(model);
        }
        ApplicationUser user = await accountService.LoginAsync(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError("", "Neplatny email nebo heslo.");
            return View(model);
        }
        else
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));

            ClaimsIdentity identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["SuccessMessage"] = "Prihlaseni probehlo uspesne.";
            return RedirectToAction("Profile","Account");
        }
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var email = model.Email.Trim().ToLower();
        bool emailExists = await accountService.EmailExist(email);

        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Uzivatel s timto emailem uz existuje.");
            return View(model);
        }

        ApplicationUser user = new ApplicationUser
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = email
        };

        user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
        await accountService.AddUser(user);

        TempData["SuccessMessage"] = "Registrace probehla uspesne.";

        return RedirectToAction(nameof(Register));
    }

    [Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        return View();
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ProfileData()
    {
        string? userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdText, out int userId))
        {
            return Unauthorized();
        }

        var userInfo = await accountService.GetProfileInfoAsync(userId);

        if (userInfo == null)
        {
            return NotFound();
        }

        var recentRentals = await accountService.GetRecentRentalDtos(userId, 5);

        return Json(new
        {
            userInfo,
            recentRentals
        });
    }

}
