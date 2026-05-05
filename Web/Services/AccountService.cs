using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Dtos;
using Web.Models;
using Web.Repositories;

namespace Web.Services;

public class AccountService
{
    private readonly AppDbContext context;
    private readonly PasswordHasher<ApplicationUser> passwordHasher = new();
    private AppUserRepositorz userRepository;

    public AccountService(AppUserRepositorz userRepository)
    {
        this .userRepository = userRepository;
    }

    public async Task<ApplicationUser?> LoginAsync(string email, string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        ApplicationUser? user = await GetUser(email);

        if (user == null)
        {
            return null;
        }

        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return user;
    }
    public async Task<bool> EmailExist(string email)
    {
        return await userRepository.EmailAllreadyExist(email);
    }
    public async Task AddUser(ApplicationUser user)
    {
         await userRepository.AddUser(user);
    }

    public async Task<UserProfileInfoDto?> GetProfileInfoAsync(int userId)
    {
        return await userRepository.GetProfileInfoAsync(userId);
    }
    public async Task<List<RecentRentalDto>> GetRecentRentalDtos(int userId,int count)
    {
        return await userRepository.GetRecentUserRentals(userId, count);
    }
    public async Task<ApplicationUser> GetUser(int userId)
    {
        return await userRepository.GetUserByIdAsync(userId);
    }
    public async Task<ApplicationUser> GetUser(string email)
    {
        int id = await userRepository.GetUserIDFromEmailAsync(email);
        if (id == -1)
            return null; // chyba
        return await userRepository.GetUserByIdAsync(id);
    }

}