using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Application.Events;
using DigitalWallet.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Infrastructure.Messaging.Consumers
{
   
    public class TransferProcessorConsumer : IConsumer<TransferRequestedEvent>
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<TransferProcessorConsumer> _logger;

        public TransferProcessorConsumer(
            AppDbContext context,
            IPublishEndpoint publishEndpoint,
            ILogger<TransferProcessorConsumer> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransferRequestedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation("Processing transfer {TransferId}", msg.TransferRequestId);

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var transfer = await _context.TransferRequests.FindAsync(msg.TransferRequestId);
                if (transfer == null || transfer.Status != TransactionStatus.Pending)
                    return;

                var senderWallet = await _context.Wallets.FindAsync(msg.SenderWalletId);
                var receiverWallet = await _context.Wallets.FindAsync(msg.ReceiverWalletId);

                if (senderWallet == null || receiverWallet == null)
                    throw new Exception("Wallet not found.");

                senderWallet.Debit(msg.Amount);
                receiverWallet.Credit(msg.Amount);

                var senderTransaction = new Transaction(
                    walletId: senderWallet.Id,
                    amount: msg.Amount,
                    type: TransactionType.TransferOut,
                    status: TransactionStatus.Completed,
                    description: $"Transfer to wallet {receiverWallet.Id}",
                    referenceId: transfer.Id
                );

                var receiverTransaction = new Transaction(
                    walletId: receiverWallet.Id,
                    amount: msg.Amount,
                    type: TransactionType.TransferIn,
                    status: TransactionStatus.Completed,
                    description: $"Transfer from wallet {senderWallet.Id}",
                    referenceId: transfer.Id
                );

                transfer.MarkCompleted();

                await _context.Transactions.AddAsync(senderTransaction);
                await _context.Transactions.AddAsync(receiverTransaction);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                await _publishEndpoint.Publish(new TransferCompletedEvent
                {
                    TransferRequestId = transfer.Id,
                    SenderWalletId = senderWallet.Id,
                    ReceiverWalletId = receiverWallet.Id,
                    Amount = msg.Amount,
                    CompletedAt = DateTime.UtcNow
                });

                _logger.LogInformation("Transfer {TransferId} completed successfully", msg.TransferRequestId);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, "Transfer {TransferId} failed", msg.TransferRequestId);

                var transfer = await _context.TransferRequests.FindAsync(msg.TransferRequestId);
                if (transfer != null)
                {
                    transfer.MarkFailed();
                    await _context.SaveChangesAsync();
                }

                await _publishEndpoint.Publish(new TransferFailedEvent
                {
                    TransferRequestId = msg.TransferRequestId,
                    SenderWalletId = msg.SenderWalletId,
                    ReceiverWalletId = msg.ReceiverWalletId,
                    Amount = msg.Amount,
                    Reason = ex.Message,
                    FailedAt = DateTime.UtcNow
                });
            }
        }
    }
}
