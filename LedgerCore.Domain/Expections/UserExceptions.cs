

namespace LedgerCore.Domain.Expections
{
    public class DuplicateEmailException :Exception
    {
        public DuplicateEmailException(string email):base($"A user with the email '{email}' already exists.") { }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid id):base($"No user with this '{id}' exist."){}
    }
}
