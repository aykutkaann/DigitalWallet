using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.DTOs.Auth
{
    public sealed class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
