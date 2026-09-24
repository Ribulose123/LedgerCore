using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Application.Interface
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(Guid userId, Guid roleId);
    }
}
