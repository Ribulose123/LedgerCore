using LedgerCore.Application.Dtos.UsersDto;
using LedgerCore.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LedgerCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]

        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = await _userService.CreateUser(request.Name, request.Email, request.Password);

            return Ok(UserResponse.FromDomain(user));
        }
    }
}
