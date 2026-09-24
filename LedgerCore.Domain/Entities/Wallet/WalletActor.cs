using LedgerCore.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Domain.Entities.Wallet
{
    public class WalletActor
    {
        public Guid UserId { get; private set; }
        public ActorRole Actor { get; private set; }

        private WalletActor() { }

        public WalletActor(Guid userId, ActorRole actor)
        {
            if(userId == Guid.Empty)
                throw new ArgumentNullException("User cannot be empty");

            UserId = userId;
            Actor = actor;
        }

        public bool isAdmin => Actor == ActorRole.Admin;
        public bool IsUser => Actor == ActorRole.User;
    }
}
