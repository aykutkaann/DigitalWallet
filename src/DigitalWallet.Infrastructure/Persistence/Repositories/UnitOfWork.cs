using DigitalWallet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IUserRepository _userRepo;
        private IWalletRepository _walletRepo;
        private ITransactionRepository _transactionRepo;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _userRepo ??= new UserRepository(_context);

        public IWalletRepository Wallets => _walletRepo ??= new WalletRepository(_context);
        public ITransactionRepository Transactions => _transactionRepo ??= new TransactionRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);

            
        }
    }
}
