using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Application.Exceptions
{
    public class InvalidRefreshTokenException:Exception
    {
        public InvalidRefreshTokenException():base("Invalid refresh token.")
        {
            
        }
    }
}
