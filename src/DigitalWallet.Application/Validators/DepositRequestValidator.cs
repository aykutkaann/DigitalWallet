using DigitalWallet.Application.DTOs.Wallet;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.Validators
{
    public class DepositRequestValidator: AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().GreaterThan(0);
        }
    }
}
