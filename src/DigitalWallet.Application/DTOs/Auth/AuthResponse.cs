using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.DTOs.Auth
{
    public  record AuthResponse(
       string Token,
       string Email,
       string FullName,
       string Currency);
}
