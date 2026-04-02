using DigitalWallet.Domain.Entities;
using DigitalWallet.Application.Events;
using DigitalWallet.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DigitalWallet.Infrastructure.Messaging.Consumers
{
    public class AuditLogConsumer :
        IConsumer<TransferRequestedEvent>,
        IConsumer<TransferCompletedEvent>,
        IConsumer<TransferFailedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuditLogConsumer> _logger;

        public AuditLogConsumer(AppDbContext context, ILogger<AuditLogConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransferRequestedEvent> context)
        {
            await SaveLog("TransferRequested", context.Message.TransferRequestId, context.Message);
        }

        public async Task Consume(ConsumeContext<TransferCompletedEvent> context)
        {
            await SaveLog("TransferCompleted", context.Message.TransferRequestId, context.Message);
        }

        public async Task Consume(ConsumeContext<TransferFailedEvent> context)
        {
            await SaveLog("TransferFailed", context.Message.TransferRequestId, context.Message);
        }

        private async Task SaveLog(string eventType, Guid entityId, object payload)
        {
            var auditLog = new AuditLog(
                eventType: eventType,
                entityId: entityId,
                payload: JsonSerializer.Serialize(payload)
            );

            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[AUDIT] {EventType} for entity {EntityId}", eventType, entityId);
        }
    }
}
