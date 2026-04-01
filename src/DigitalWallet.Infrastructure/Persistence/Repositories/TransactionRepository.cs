using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository :ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions.FindAsync(id, cancellationToken);

        }

        public async Task<List<Transaction>> GetByWalletIdAsync(Guid walletId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId).OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
             await _context.Transactions.AddAsync(transaction, cancellationToken);
        }

    }
}
