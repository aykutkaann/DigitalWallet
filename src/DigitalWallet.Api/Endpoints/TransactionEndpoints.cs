using DigitalWallet.Application.Services;

namespace DigitalWallet.Api.Endpoints
{
    public static class TransactionEndpoints 
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/transactions").WithTags("Transactions").RequireAuthorization();

            group.MapGet("/", async (int page= 1, int pageSize=10, ITransactionService service= default, CancellationToken ct= default) =>
            {

                var transaction = await service.GetMyTransactionsAsync(page , pageSize, ct);

                return Results.Ok(transaction);
            }).WithName("GetMyTransactions");
        }
    }
}
