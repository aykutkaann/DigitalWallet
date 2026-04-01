using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Infrastructure.Persistence.Repositories
{
    public class WalletRepository :IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Wallets.FindAsync(id, cancellationToken);
        }

        public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

            
        }

        public async Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default)
        {
            await _context.Wallets.AddAsync(wallet, cancellationToken);
        }

        public  Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default)
        {
            _context.Wallets.Update(wallet);

            return Task.CompletedTask;

            
        }
    }
}
