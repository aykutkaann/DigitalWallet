using DigitalWallet.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.Validators
{
    public class RegisterRequestValidator :AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().MaximumLength(255).MinimumLength(2);

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(6);

            RuleFor(x => x.Currency)
                .NotEmpty().MaximumLength(3);
        }
    }
}
