using DigitalWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Domain.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default);

        Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default);
    }
}
