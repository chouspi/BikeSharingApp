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

        //[HttpPost("UserAdd")]
        //public async Task<ActionResult<DesktopUserDto>> CreateUser(CreateUserApiRequest request)
        //{
        //    DesktopUserDto createdUser = await usersRepository.ApiCreateUserAsync(
        //        request.User,
        //        request.HashPassword
        //    );

        //    return Ok(createdUser);
        //}
    }
}
