using System.Net;
using System.Text.Json;

namespace DigitalWallet.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {

               
                await _next(context);
            }
            catch(Exception err)
            {
                _logger.LogError(err, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, err);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound, //404
                ArgumentException => HttpStatusCode.BadRequest, //400
                InvalidOperationException => HttpStatusCode.Conflict, //409
                UnauthorizedAccessException =>HttpStatusCode.Unauthorized, //401
                _ => HttpStatusCode.InternalServerError //500


            };

            context.Response.StatusCode = (int)statusCode;

            var message = statusCode == HttpStatusCode.InternalServerError ? "An unexpected error occured." : exception.Message;

            var response = new
            {
                Status = context.Response.StatusCode,
                Message = message,
                Title = statusCode.ToString()
            };

            return context.Response.WriteAsJsonAsync(response);

        }

      

        
    }
}
