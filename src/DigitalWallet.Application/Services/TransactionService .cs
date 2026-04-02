using DigitalWallet.Application.DTOs.Transaction;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.Services
{
    public class TransactionService(
        ICurrentUserService currentUserService,
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository
        ) : ITransactionService
    {

        public async Task<List<TransactionResponse>> GetMyTransactionsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var userId = currentUserService.UserId;

            var wallet = await walletRepository.GetByUserIdAsync(userId, cancellationToken);

            if (wallet == null)
                throw new InvalidOperationException("Wallet not found.");

            var transactions = await transactionRepository.GetByWalletIdAsync(wallet.Id, page, pageSize, cancellationToken);


            return transactions.Select(t => new TransactionResponse(
                    t.Id,
                    t.Amount,
                    t.Type.ToString(),
                    t.Status.ToString(),
                    t.Description,
                    t.ReferenceId,
                    t.CreatedAt
                )).ToList();





        }


    }
}
