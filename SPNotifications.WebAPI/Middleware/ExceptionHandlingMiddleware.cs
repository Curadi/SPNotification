using System.Net;
using System.Text.Json;
using SPNotifications.Domain.Exceptions;

namespace SPNotifications.WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteProblemDetails(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (BadRequestException ex)
            {
                await WriteProblemDetails(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                await WriteProblemDetails(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Erro interno do servidor"
                );
            }
        }

        private static async Task WriteProblemDetails(
            HttpContext context,
            HttpStatusCode status,
            string message)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/problem+json";

            var response = new
            {
                title = message,
                status = (int)status
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
