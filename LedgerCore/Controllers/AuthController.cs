using LedgerCore.Application.Dtos.AuthDto;
using LedgerCore.Application.Exceptions;
using LedgerCore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LedgerCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService; 
        public AuthController(AuthService authService) { _authService = authService; }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _authService.RefreshAsync(request.RefreshToken);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
    RefreshTokenRequest request)
        {
            await _authService.LogoutAsync(
                request.RefreshToken);

            return NoContent();
        }
    }
}
