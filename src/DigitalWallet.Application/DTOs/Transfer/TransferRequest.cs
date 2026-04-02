namespace DigitalWallet.Application.DTOs.Transfer
{
    
    public record CreateTransferRequest(
        string ReceiverEmail,
        decimal Amount,
        string IdempotencyKey);
}
