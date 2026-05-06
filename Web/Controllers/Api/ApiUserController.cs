using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Api.ApiRepositories;
using Web.Dtos;
using Web.Repositories;

namespace Web.Controllers.Api
{
    [ApiController]
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class ApiUsersController : ControllerBase
    {
        private ApiUserRepository usersRepository;

        public ApiUsersController(ApiUserRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        [HttpGet("AllUsers")]
        public async Task<ActionResult<List<DesktopUserDto>>> GetAllUsers()
        {
            List<DesktopUserDto> users = await usersRepository.ApiGetAllUsersAsync();

            return Ok(users);
        }

        [HttpPost("UserAdd")]
        public async Task<ActionResult<DesktopUserDto>> CreateUser(CreateUserApiRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.User.FirstName))
            {
                return BadRequest("Jmeno je povinne.");
            }

            if (string.IsNullOrWhiteSpace(request.User.LastName))
            {
                return BadRequest("Prijmeni je povinne.");
            }

            if (string.IsNullOrWhiteSpace(request.User.Email))
            {
                return BadRequest("Email je povinny.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Heslo je povinne.");
            }

            bool emailExists = await usersRepository.ApiEmailAllreadyExist(request.User.Email);

            if (emailExists)
            {
                return BadRequest("Uzivatel s timto emailem uz existuje.");
            }

            DesktopUserDto createdUser = await usersRepository.ApiCreateUserAsync(request.User, request.Password);

            return Ok(createdUser);
        }

        [HttpPut("UserEdit/{id}")]
        public async Task<ActionResult<DesktopUserDto>> EditUser(int id, DesktopUserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                return BadRequest("Jmeno je povinne.");
            }

            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                return BadRequest("Prijmeni je povinne.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Email je povinny.");
            }

            bool emailExists = await usersRepository.ApiEmailUsedByOtherUser(user.Email, id);

            if (emailExists)
            {
                return BadRequest("Uzivatel s timto emailem uz existuje.");
            }

            DesktopUserDto? updatedUser = await usersRepository.ApiUpdateUserAsync(id, user);

            if (updatedUser == null)
            {
                return NotFound("Uzivatel nenalezen.");
            }

            return Ok(updatedUser);
        }

        [HttpDelete("UserDelete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            string result = await usersRepository.ApiDeleteUserAsync(id);

            if (result == "notfound")
            {
                return NotFound("Uzivatel nenalezen.");
            }

            if (result == "rentals")
            {
                return BadRequest("Uzivatel ma vypujcky, nejde smazat.");
            }

            return Ok();
        }
    }
}
