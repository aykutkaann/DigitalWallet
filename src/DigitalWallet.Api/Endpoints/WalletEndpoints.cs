using DigitalWallet.Api.Filters;
using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.DTOs.Auth;
using DigitalWallet.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.Api.Endpoints
{
    public static class WalletEndpoints
    {

        public static void MapWalletEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/wallets").WithTags("Wallets").RequireAuthorization();


            group.MapGet("/me", async (IWalletService walletService,  CancellationToken ct) =>
            {
                var wallet = await walletService.GetMyWalletAsync(ct);

                return Results.Ok(wallet);

            }).WithName("GetMyWallet");

            group.MapPost("/deposit", async (
                [FromBody]  DepositRequest request, IWalletService walletService, CancellationToken ct ) =>
            {
                try
                {
                    var response = await walletService.DepositAsync(request, ct);
                    return Results.Ok(response);
                }
                catch (ArgumentException err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }
                catch (InvalidOperationException err)
                {
                    return Results.NotFound(new { message = err.Message });
                }

            }).WithName("Deposit").AddEndpointFilter<ValidationFilter<DepositRequest>>();
        }
    }
}
