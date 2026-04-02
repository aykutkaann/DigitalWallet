namespace DigitalWallet.Application.Events
{
    public record TransferCompletedEvent
    {
        public Guid TransferRequestId { get; init; }
        public Guid SenderWalletId { get; init; }
        public Guid ReceiverWalletId { get; init; }
        public decimal Amount { get; init; }
        public DateTime CompletedAt { get; init; }
    }
}
