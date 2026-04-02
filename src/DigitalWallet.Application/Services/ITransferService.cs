using DigitalWallet.Application.DTOs.Transfer;

namespace DigitalWallet.Application.Services
{
    public interface ITransferService
    {
        Task<TransferResponse> CreateTransferAsync(CreateTransferRequest request, CancellationToken ct = default);
        Task<TransferResponse> GetTransferByIdAsync(Guid id, CancellationToken ct = default);
    }
}
