using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Balance { get; private set; }
        public string Currency { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }


        public User User { get; private set; }

        public ICollection<Transaction> Transactions { get; private set; } = new HashSet<Transaction>();

        public ICollection<TransferRequest> SentTransfers { get; private set; } = new HashSet<TransferRequest>();
        public ICollection<TransferRequest> ReceivedTransfers { get; private set; } = new HashSet<TransferRequest>();

        private Wallet() { }

        public Wallet(Guid userId, string currency)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Balance = 0;
            Currency = currency;
            CreatedAt = DateTime.UtcNow;
            
        }

    }
}
