using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.DTOs.Transfer
{
    public record TransferResponse(
        Guid TransferRequestId,
        string Status,
        decimal Amount,
        string ReceiverEmail,
        DateTime CreatedAt);
        
        
}
