using DigitalWallet.Domain.Entities;

namespace DigitalWallet.Domain.Interfaces
{
    public interface ITransferRequestRepository
    {
        Task<TransferRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TransferRequest?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
        Task AddAsync(TransferRequest transferRequest, CancellationToken cancellationToken = default);
        Task UpdateAsync(TransferRequest transferRequest, CancellationToken cancellationToken = default);
    }
}
