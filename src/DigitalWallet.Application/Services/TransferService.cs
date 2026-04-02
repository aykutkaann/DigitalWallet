using DigitalWallet.Application.DTOs.Transfer;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Application.Events;
using MassTransit;

namespace DigitalWallet.Application.Services
{
    public class TransferService(
        ICurrentUserService currentUserService,
        IWalletRepository walletRepository,
        IUserRepository userRepository,
        ITransferRequestRepository transferRequestRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint) : ITransferService
    {
        public async Task<TransferResponse> CreateTransferAsync(CreateTransferRequest request, CancellationToken ct)
        {
            var senderId = currentUserService.UserId;
            var senderWallet = await walletRepository.GetByUserIdAsync(senderId, ct)
                ?? throw new InvalidOperationException("Sender wallet not found.");

            var receiverUser = await userRepository.GetByEmailAsync(request.ReceiverEmail, ct)
                ?? throw new InvalidOperationException("Receiver not found.");

            var receiverWallet = await walletRepository.GetByUserIdAsync(receiverUser.Id, ct)
                ?? throw new InvalidOperationException("Receiver wallet not found.");

            if (senderWallet.Id == receiverWallet.Id)
                throw new InvalidOperationException("You cannot send money to yourself.");

            if (request.Amount <= 0)
                throw new ArgumentException("Transfer amount must be positive.");

            if (senderWallet.Balance < request.Amount)
                throw new InvalidOperationException("Insufficient balance for this transfer.");

            var existing = await transferRequestRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, ct);
            if (existing != null)
                return new TransferResponse(existing.Id, existing.Status.ToString(), existing.Amount, request.ReceiverEmail, existing.CreatedAt);

            var transfer = new TransferRequest(
                senderWalletId: senderWallet.Id,
                receiverWalletId: receiverWallet.Id,
                amount: request.Amount,
                idempotencyKey: request.IdempotencyKey
            );

            await transferRequestRepository.AddAsync(transfer, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await publishEndpoint.Publish(new TransferRequestedEvent
            {
                TransferRequestId = transfer.Id,
                SenderWalletId = senderWallet.Id,
                ReceiverWalletId = receiverWallet.Id,
                Amount = request.Amount,
                RequestedAt = transfer.CreatedAt
            }, ct);

            return new TransferResponse(transfer.Id, transfer.Status.ToString(), transfer.Amount, request.ReceiverEmail, transfer.CreatedAt);
        }

        public async Task<TransferResponse> GetTransferByIdAsync(Guid id, CancellationToken ct)
        {
            var transfer = await transferRequestRepository.GetByIdAsync(id, ct)
                ?? throw new InvalidOperationException("Transfer not found.");

            var receiverWallet = await walletRepository.GetByIdAsync(transfer.ReceiverWalletId, ct);
            var receiverUser = receiverWallet != null
                ? await userRepository.GetByIdAsync(receiverWallet.UserId, ct)
                : null;

            return new TransferResponse(
                transfer.Id,
                transfer.Status.ToString(),
                transfer.Amount,
                receiverUser?.Email ?? "Unknown",
                transfer.CreatedAt
            );
        }
    }
}
