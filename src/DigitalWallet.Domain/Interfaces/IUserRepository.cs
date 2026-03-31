using DigitalWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken=default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string email, CancellationToken cancellationToken=default);

        Task UpdateAsync(User user, CancellationToken cancellationToken=default);
        Task DeleteAsync(User user, CancellationToken cancellationToken=default);

    }
}
