using Ledger.Persistence.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Tests.Infrastructure
{
    public class PasswordHasherTests
    {
        [Fact]

        public void Hash_ProducesHash_CanBeVerified()
        {
            var passwordHashed = new PasswordHasher();
            var plainPassword = "SomeTestPassword123";

            var hash = passwordHashed.Hash(plainPassword);

            var rawHash = new PasswordHasher<object>();

            var result = rawHash.VerifyHashedPassword(new object(), hash, plainPassword);

            Assert.Equal(PasswordVerificationResult.Success, result);
        }
    }
}
