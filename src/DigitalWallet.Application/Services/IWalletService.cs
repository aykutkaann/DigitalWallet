using DigitalWallet.Application.DTOs.Wallet;

namespace DigitalWallet.Application.Services
{
    public interface IWalletService
    {
        Task<WalletResponse> GetMyWalletAsync(CancellationToken cancellationToken = default);

        Task<WalletResponse> DepositAsync(DepositRequest request, CancellationToken cancellationToken = default);
    }
}
