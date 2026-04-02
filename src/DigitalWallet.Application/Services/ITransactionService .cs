using DigitalWallet.Application.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> GetMyTransactionsAsync(int page , int pageSize , CancellationToken cancellationToken =default);
    }
}
