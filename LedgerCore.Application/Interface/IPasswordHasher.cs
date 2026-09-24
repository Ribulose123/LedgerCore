

using LedgerCore.Application.Enum;

namespace LedgerCore.Application.Interface
{
    public interface IPasswordHasher
    {
        string Hash(string plainText);
       PasswordVerificationOutcome Verify(string plainText, string hash);
    }
}
