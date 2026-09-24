using LedgerCore.Application.Enum;
using LedgerCore.Application.Interface;
using LedgerCore.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace Ledger.Persistence.Repository
{
    public class PasswordHasher:IPasswordHasher
    {
        private readonly PasswordHasher<User> _hasher = new ();

        public string Hash(string plainText)
        {
            var user = (User)RuntimeHelpers.GetUninitializedObject(typeof(User));
            return _hasher.HashPassword(user,plainText);
        }

       public PasswordVerificationOutcome Verify(string plainText, string hash)
        {
            var user = (User)RuntimeHelpers.GetUninitializedObject(typeof(User));
            var result = _hasher.VerifyHashedPassword(user, hash, plainText);

            return result switch
            {
                PasswordVerificationResult.Success =>
                    PasswordVerificationOutcome.Success,

                PasswordVerificationResult.SuccessRehashNeeded =>
                    PasswordVerificationOutcome.SuccessRehashNeeded,

                _ =>
                  PasswordVerificationOutcome.Failed,
            };
        }
    }
}
