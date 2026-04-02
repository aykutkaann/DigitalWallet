using DigitalWallet.Application.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Infrastructure.Messaging.Consumers
{
 
    public class NotificationConsumer :
        IConsumer<TransferCompletedEvent>,
        IConsumer<TransferFailedEvent>
    {
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(ILogger<NotificationConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<TransferCompletedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation(
                "[NOTIFICATION] Transfer {TransferId} completed. Amount: {Amount} from {Sender} to {Receiver}",
                msg.TransferRequestId, msg.Amount, msg.SenderWalletId, msg.ReceiverWalletId);

            // TODO: Send actual email/push notification here
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<TransferFailedEvent> context)
        {
            var msg = context.Message;
            _logger.LogWarning(
                "[NOTIFICATION] Transfer {TransferId} failed. Reason: {Reason}",
                msg.TransferRequestId, msg.Reason);

            // TODO: Send failure notification to sender
            return Task.CompletedTask;
        }
    }
}
