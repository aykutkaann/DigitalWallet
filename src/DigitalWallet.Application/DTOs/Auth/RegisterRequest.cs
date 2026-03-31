using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.DTOs.Auth
{
    public sealed class RegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Currency { get; set; }
    }
}
