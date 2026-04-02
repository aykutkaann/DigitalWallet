namespace DigitalWallet.Application.Events
{
    public record TransferRequestedEvent
    {
        public Guid TransferRequestId { get; init; }
        public Guid SenderWalletId { get; init; }
        public Guid ReceiverWalletId { get; init; }
        public decimal Amount { get; init; }
        public DateTime RequestedAt { get; init; }
    }
}
