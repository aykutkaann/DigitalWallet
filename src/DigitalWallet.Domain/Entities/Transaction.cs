using DigitalWallet.Domain.Enums;

namespace DigitalWallet.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid WalletId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }
        public Guid? ReferenceId { get; private set; }
        public TransferRequest? Request { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Wallet Wallet { get; private set; }

        private Transaction() { }

   
        public Transaction(
            Guid walletId,
            decimal amount,
            TransactionType type,
            TransactionStatus status,
            string? description = null,
            Guid? referenceId = null)
        {
            Id = Guid.NewGuid();
            WalletId = walletId;
            Amount = amount;
            Type = type;               
            Status = status;            
            Description = description;
            ReferenceId = referenceId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
