using DigitalWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Domain.Entities
{
    public class TransferRequest
    {
        public Guid Id { get; private set; }
        public Guid SenderWalletId { get; private set; }
        public  Guid ReceiverWalletId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionStatus Status { get; private set; }
        public string IdempotencyKey { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public Wallet SenderWallet { get; private set; }
        public Wallet ReceiverWallet { get; private set; }

    }
}
