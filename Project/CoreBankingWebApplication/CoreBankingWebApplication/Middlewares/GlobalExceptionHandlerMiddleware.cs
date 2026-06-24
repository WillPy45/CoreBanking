using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CoreBankingWebApplication.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            IWebHostEnvironment webHostEnvironment,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) {



                ExceptionDetails? exceptionDetailsInstance = new ExceptionDetails(ex);
                ExceptionDetails? exceptionDetails = _webHostEnvironment.IsDevelopment() == true 
                    ? exceptionDetailsInstance
                    : null;

                //Registramos la excepcion
                _logger.LogCritical(ex, "CRITICAL ERROR");
                _logger.LogWarning($"ExceptionMessage: {exceptionDetails.Message} | HResult: {exceptionDetails.HResult}");

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/json";


                

                GlobalExceptionResponse exceptionResponse = new GlobalExceptionResponse("Something went wrong", exceptionDetails);

                await httpContext.Response.WriteAsJsonAsync<GlobalExceptionResponse>(exceptionResponse);
                
            }

            return;


        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }

    public class GlobalExceptionResponse
    {
        public string Message { get; set; }
        public ExceptionDetails? ExceptionDetails { get; set; }
        public GlobalExceptionResponse(string message, ExceptionDetails? exceptionDetails)
        {
            Message = message;
            ExceptionDetails = exceptionDetails;
        }
    }

    public class ExceptionDetails
    {
        public string Message { get; set; }
        public int HResult { get; set; }
        public string? StackTrace { get; set; }
        public string? HelpLink {get; set;}
        public ExceptionDetails(Exception exception)
        {

            Message = exception.Message;
            HResult = exception.HResult;
            StackTrace = exception.StackTrace;
            HelpLink = exception.HelpLink;
        }
    }


}
