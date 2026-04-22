using System.ComponentModel.DataAnnotations;
using System.Net;
using Tekwill.Library.Dto;

namespace Tekwill.Library.Midllewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {

                await next(context);
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = (int)ConvertExceptionToHttp(ex),
                    Title = ConvertExceptionToHttp(ex).ToString(),
                    Details = ex.Message
                };
                context.Response.ContentType = "appliaction/json";
                context.Response.StatusCode = problemDetails.Status;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private static HttpStatusCode ConvertExceptionToHttp(Exception e) =>
         e switch
         {
             KeyNotFoundException => HttpStatusCode.NotFound,
             ValidationException => HttpStatusCode.BadRequest,
             UnauthorizedAccessException => HttpStatusCode.Unauthorized,
             _ => HttpStatusCode.InternalServerError
         };
    }
}

