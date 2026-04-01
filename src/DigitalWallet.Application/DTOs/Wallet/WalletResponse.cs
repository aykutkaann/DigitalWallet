using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.DTOs.Wallet
{


    public record WalletResponse(Guid Id, decimal Balance, string Currency, DateTime CreatedAt);
}
