using CoreBankingWebApplication.Infraestructure.Authentication;
using CoreBankingWebApplication.Infraestructure.Authentication.SessionUsageRiskManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CoreBankingWebApplication.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class SessionUsageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISessionUsageRiskManager _sessionUsageRiskManager;
        private readonly IProfileSessionProvider _sessionProvider;
        
        private const string SessionTokenHeaderName = "Session-Token";


        public SessionUsageMiddleware(RequestDelegate next, ISessionUsageRiskManager sessionUsageRiskManager, IProfileSessionProvider sessionProvider)
        {
            _next = next;
            _sessionUsageRiskManager = sessionUsageRiskManager;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // Retrieves the session token provided
            if (!httpContext.Request.Headers.TryGetValue(SessionTokenHeaderName, out var extractedSessionToken))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Session token was not provided");
                return;
            }
            string extractedToken = extractedSessionToken!;

            // Generates the request source identifier
            var sourceIdentifierGenerator = new HttpContextSourceIdentifier(httpContext);
            string sourceIdentifier = sourceIdentifierGenerator.Generate();

            // Retrieves the session 
            ProfileSession? session = _sessionProvider.GetSession(extractedToken);

            if (session is null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsync("Session for session token not found");
                return;
            }

            var sessionUsageManagerResult = _sessionUsageRiskManager.HandleRisk(session, sourceIdentifier);
            if (sessionUsageManagerResult.IsFailure) 
            {
                switch (sessionUsageManagerResult.Error.Code)
                {
                    case SessionUsageError.SourceSuspicious:
                        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await httpContext.Response.WriteAsync("Invalid source session");
                        return;
                    default:
                        throw new Exception("Unhandled");

                }
            }

            //
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SessionUsageMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionUsageMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionUsageMiddleware>();
        }
    }
}
