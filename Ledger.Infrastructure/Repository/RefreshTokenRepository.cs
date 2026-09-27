using Ledger.Persistence.Context;
using LedgerCore.Domain.Entities.Token;
using LedgerCore.Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Persistence.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string hashToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hashToken);
        }
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
