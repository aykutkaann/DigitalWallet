using DigitalWallet.Application.DTOs.Auth;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Application.Services
{
    public class AuthService(
            IUserRepository userRepository,
            IWalletRepository walletRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUnitOfWork unitOfWork) : IAuthService
    {

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            if (await userRepository.ExistsAsync(request.Email, cancellationToken))
                throw new InvalidOperationException("Email already exist");



            var passwordHash = passwordHasher.Hash(request.Password);


            var user = new User(
                request.FullName,
                request.Email,
                passwordHash,
                UserRole.User);

            var wallet = new Wallet(user.Id, request.Currency);


            await userRepository.AddAsync(user);

            await walletRepository.AddAsync(wallet);

            await unitOfWork.SaveChangesAsync(cancellationToken);


            var token = jwtTokenGenerator.GenerateToken(user);

            return new AuthResponse(token, user.Email, user.FullName, wallet.Currency);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            var wallet = await walletRepository.GetByUserIdAsync(user.Id,cancellationToken);

            var token = jwtTokenGenerator.GenerateToken(user);
            return new AuthResponse(token, user.Email, user.FullName, wallet.Currency);
        }

    }
}
