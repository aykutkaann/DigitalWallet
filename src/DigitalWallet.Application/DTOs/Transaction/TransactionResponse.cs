using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.DTOs.Transaction
{
    public record TransactionResponse(
        Guid Id,
        decimal Amount,
        string Type,
        string Status,
        string? Description,
        Guid? ReferenceId,
        DateTime CreatedAt);
}
