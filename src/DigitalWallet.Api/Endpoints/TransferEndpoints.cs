using DigitalWallet.Api.Filters;
using DigitalWallet.Application.DTOs.Transfer;
using DigitalWallet.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.Api.Endpoints
{
    public static class TransferEndpoints
    {
        public static void MapTransferEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/transfers").WithTags("Transfers").RequireAuthorization();

            group.MapPost("/", async (
                [FromBody] CreateTransferRequest request,
                ITransferService service,
                CancellationToken ct) =>
            {
                try
                {
                    var result = await service.CreateTransferAsync(request, ct);
                    return Results.Created($"/api/transfers/{result.TransferRequestId}", result);
                }
                catch (InvalidOperationException err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }
                catch (ArgumentException err)
                {
                    return Results.BadRequest(new { message = err.Message });
                }
            }).WithName("CreateTransfer")
              .AddEndpointFilter<ValidationFilter<CreateTransferRequest>>();

            group.MapGet("/{id:guid}", async (Guid id, ITransferService service, CancellationToken ct) =>
            {
                try
                {
                    var result = await service.GetTransferByIdAsync(id, ct);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException err)
                {
                    return Results.NotFound(new { message = err.Message });
                }
            }).WithName("GetTransferById");
        }
    }
}
