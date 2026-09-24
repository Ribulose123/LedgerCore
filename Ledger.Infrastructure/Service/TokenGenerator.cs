

using LedgerCore.Application.Interface;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace Ledger.Persistence.Service
{
    public class TokenGenerator:ITokenGenerator
    {
        public string GenerateRawToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);

            return WebEncoders.Base64UrlEncode(bytes);
        }
    }
}
