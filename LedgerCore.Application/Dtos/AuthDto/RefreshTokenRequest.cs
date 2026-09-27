using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Application.Dtos.AuthDto
{
    public record RefreshTokenRequest
    {
        public string RefreshToken { get; init; } = string.Empty;
    }
}
