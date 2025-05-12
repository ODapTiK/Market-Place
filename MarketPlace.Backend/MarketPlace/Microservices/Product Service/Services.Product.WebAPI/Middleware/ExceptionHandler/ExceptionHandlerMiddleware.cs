using FluentValidation;
using System.Net;
using System.Text.Json;

namespace ProductService
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _Next;

        public ExceptionHandlerMiddleware(RequestDelegate next) => _Next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _Next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (ex)
            {
                case ValidationException validationExeption:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(validationExeption.Errors);
                    break;
                case EntityNotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case LackOfRightException:
                    code = HttpStatusCode.Forbidden;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (result == string.Empty)
            {
                result = JsonSerializer.Serialize(new { error = ex.Message });
            }

            return context.Response.WriteAsync(result);
        }
    }
}
