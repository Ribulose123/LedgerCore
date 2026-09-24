using LedgerCore.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Persistence.Service
{
    public class TokenHasher:ITokenHasher
    {
        public string Hash (string value)
        {
            var bytes = Encoding.UTF8.GetBytes (value);
            var hash = SHA256.HashData (bytes);

            return Convert.ToHexString (hash);
        }
    }
}
