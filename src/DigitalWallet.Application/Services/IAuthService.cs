using DigitalWallet.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    }
}
