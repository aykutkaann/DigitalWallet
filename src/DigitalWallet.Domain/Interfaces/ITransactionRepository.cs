using DigitalWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Domain.Interfaces
{
    public interface ITransactionRepository
    {

        Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Transaction>> GetByWalletIdAsync(Guid walletId,int page,int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(Transaction transaction, CancellationToken cancellationToken=default);
    }
}
