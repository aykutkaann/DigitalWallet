using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Persistence.Repositories
{
    public class TransferRequestRepository : ITransferRequestRepository
    {
        private readonly AppDbContext _context;

        public TransferRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TransferRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.TransferRequests.FindAsync([id], cancellationToken);
        }

        public async Task<TransferRequest?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
        {
            return await _context.TransferRequests
                .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey, cancellationToken);
        }

        public async Task AddAsync(TransferRequest transferRequest, CancellationToken cancellationToken = default)
        {
            await _context.TransferRequests.AddAsync(transferRequest, cancellationToken);
        }

        public Task UpdateAsync(TransferRequest transferRequest, CancellationToken cancellationToken = default)
        {
            _context.TransferRequests.Update(transferRequest);
            return Task.CompletedTask;
        }
    }
}
