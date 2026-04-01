using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Infrastructure.Persistence.Repositories
{
    public class UserRepository :IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FindAsync(id, cancellationToken);

        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }


        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
             await _context.Users.AddAsync(user, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            
            return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);


        }

        public  Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
               _context.Users.Update(user);

            return Task.CompletedTask;
        }

        public  Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Remove(user);

            return Task.CompletedTask;
        }
    }
}
