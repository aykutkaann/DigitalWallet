using DigitalWallet.Api.Endpoints;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Application.Services;
using DigitalWallet.Application.Settings;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Infrastructure.Persistence;
using DigitalWallet.Infrastructure.Persistence.Repositories;
using DigitalWallet.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));



builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

//Repositrories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Endpoinst
app.MapGet("/health", () => "API is running");

app.MapAuthEndpoints();



app.Run();

