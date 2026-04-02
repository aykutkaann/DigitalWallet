using DigitalWallet.Domain.Enums;

namespace DigitalWallet.Domain.Entities
{
    public class TransferRequest
    {
        public Guid Id { get; private set; }
        public Guid SenderWalletId { get; private set; }
        public Guid ReceiverWalletId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionStatus Status { get; private set; }
        public string IdempotencyKey { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public Wallet SenderWallet { get; private set; }
        public Wallet ReceiverWallet { get; private set; }

        private TransferRequest() { }

        public TransferRequest(Guid senderWalletId, Guid receiverWalletId, decimal amount, string idempotencyKey)
        {
            Id = Guid.NewGuid();
            SenderWalletId = senderWalletId;
            ReceiverWalletId = receiverWalletId;
            Amount = amount;
            IdempotencyKey = idempotencyKey;
            Status = TransactionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkCompleted()
        {
            Status = TransactionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkFailed()
        {
            Status = TransactionStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
