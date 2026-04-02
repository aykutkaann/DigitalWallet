namespace DigitalWallet.Application.Events
{
    public record TransferFailedEvent
    {
        public Guid TransferRequestId { get; init; }
        public Guid SenderWalletId { get; init; }
        public Guid ReceiverWalletId { get; init; }
        public decimal Amount { get; init; }
        public string Reason { get; init; }
        public DateTime FailedAt { get; init; }
    }
}
