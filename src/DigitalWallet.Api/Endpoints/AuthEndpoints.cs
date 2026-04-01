using DigitalWallet.Application.DTOs.Auth;
using DigitalWallet.Application.Services;

namespace DigitalWallet.Api.Endpoints
{
    public static class AuthEndpoints
    {

        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/auth").WithTags("Auths");

            group.MapPost("/register", async(RegisterRequest request, IAuthService service, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await service.RegisterAsync(request, cancellationToken);
                    return Results.Created($"/api/auth/user{request.Email}", result);
                }
                catch (InvalidOperationException err)
                {
                    return Results.Conflict(new { message = err.Message });
                }
                catch (Exception err)
                {
                    return Results.BadRequest(new {message = err.Message});
                }

            });

            group.MapPost("/login", async (LoginRequest request, IAuthService service, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await service.LoginAsync(request, cancellationToken);
                    return Results.Ok(result);
                }catch(UnauthorizedAccessException err)
                {
                    return Results.Unauthorized();
                }

            });
        }
    }
}
