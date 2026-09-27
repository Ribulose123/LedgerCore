

namespace LedgerCore.Application.Dtos.AuthDto
{
    public record AuthResult(
        string AccessToken,
        string RefreshToken,
        string Name,
        string Email);
}
