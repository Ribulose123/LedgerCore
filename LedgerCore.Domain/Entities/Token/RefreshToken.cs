using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Domain.Entities.Token
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId {  get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpireAt {  get; private set; }
        public Guid? ReplacedByTokenId { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        private RefreshToken() { }

        public RefreshToken(Guid userId, string tokenHash, TimeSpan lifeTime)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.",
                    nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(tokenHash))
            {
                throw new ArgumentException ("token cannot be empty.",
                    nameof(tokenHash));
            }

            if(lifeTime <= TimeSpan.Zero)
            {
                throw new ArgumentException("Token lifetime must be greater than zero.",
                    nameof(lifeTime));
            }
            Id = Guid.NewGuid();
            UserId = userId;
            TokenHash = tokenHash;
            CreatedAt = DateTime.UtcNow; 
            ExpireAt = CreatedAt.Add(lifeTime);
        }

        public void ReplaceToken (Guid newTokenId)
        {
            if (newTokenId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Replacement token ID cannot be empty.",
                    nameof(newTokenId));
            }

            if (IsExpired)
            {
                throw new InvalidOperationException(
                    "An expired refresh token cannot be replaced.");
            }

            if (RevokedAt.HasValue)
                throw new InvalidOperationException(
                    "A revoked refresh token cannot be replaced.");

            if (ReplacedByTokenId.HasValue)
            {
                throw new InvalidOperationException(
                    "Refresh token has already been replaced.");
            }

            ReplacedByTokenId = newTokenId;
        }

        public void RevokeToken()
        {
            if (RevokedAt.HasValue)
                throw new InvalidOperationException(
                    "Refresh token has already been revoked.");

            if (ReplacedByTokenId.HasValue)
                throw new InvalidOperationException(
                    "Refresh token has already been revoked.");

            RevokedAt = DateTime.UtcNow;
            
        }

        public bool IsExpired => DateTime.UtcNow >= ExpireAt;
        public bool IsActive =>  !IsExpired && ReplacedByTokenId == null && RevokedAt == null;
    }
}
