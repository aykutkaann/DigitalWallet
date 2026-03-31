using DigitalWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
