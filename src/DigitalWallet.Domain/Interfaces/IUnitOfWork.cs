using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IWalletRepository Wallets { get; }
        IUserRepository Users { get; }
        ITransactionRepository Transactions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
