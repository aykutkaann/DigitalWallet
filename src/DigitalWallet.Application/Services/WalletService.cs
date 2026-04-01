using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Interfaces;

namespace DigitalWallet.Application.Services
{
    public class WalletService(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService) : IWalletService
    {
        public async Task<WalletResponse> GetMyWalletAsync(CancellationToken cancellationToken = default)
        {
            var userId = currentUserService.UserId;

            var wallet = await walletRepository.GetByUserIdAsync(userId, cancellationToken);

            if (wallet == null)
                throw new InvalidOperationException("Wallet not found.");

           
            return new WalletResponse(wallet.Id, wallet.Balance, wallet.Currency, wallet.CreatedAt);
        }

        public async Task<WalletResponse> DepositAsync(DepositRequest request, CancellationToken cancellationToken = default)
        {
            var userId = currentUserService.UserId;
            var wallet = await walletRepository.GetByUserIdAsync(userId, cancellationToken);

            if (wallet == null)
                throw new InvalidOperationException("Wallet not found.");

            
            wallet.Credit(request.Amount);

            
            var transaction = new Transaction(
                walletId: wallet.Id,
                amount: request.Amount,
                type: TransactionType.Deposit,
                status: TransactionStatus.Completed,
                description: $"Deposit of {request.Amount} {wallet.Currency}"
            );

            
            await walletRepository.UpdateAsync(wallet, cancellationToken);
            await transactionRepository.AddAsync(transaction, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new WalletResponse(wallet.Id, wallet.Balance, wallet.Currency, wallet.CreatedAt);
        }
    }
}
