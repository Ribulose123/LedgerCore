using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Domain.Identity
{
    public static class RoleIds
    {
        public static readonly Guid User =
            new("11111111-1111-1111-1111-111111111111");

        public static readonly Guid Admin =
            new("22222222-2222-2222-2222-222222222222");
    }
}
