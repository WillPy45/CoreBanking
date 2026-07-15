using System.Security.Cryptography;
using System.Text;

namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    /// <summary>
    /// Provides a source identifier based on the current HTTP context.
    /// This implementation generates a unique identifier by hashing
    /// the remote IP address of the client making the request.
    /// </summary>
    public class HttpContextSourceIdentifier : ISourceIdentifier
    {
        private readonly HttpContext _httpContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextSourceIdentifier"/> class.
        /// </summary>
        /// <param name="context">The HTTP context associated with the current request.</param>
        public HttpContextSourceIdentifier(HttpContext context)
        {
            _httpContext = context;
        }

        /// <summary>
        /// Generates a unique identifier string for the current request source.
        /// The identifier is created by hashing the remote IP address using SHA256
        /// and encoding the result in Base64.
        /// </summary>
        /// <returns>A Base64-encoded SHA256 hash representing the client source.</returns>
        public string Generate()
        {
            // Retrieve the remote IP address of the client
            var remoteIp = _httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Convert the IP address to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(remoteIp);

            // Compute SHA256 hash
            byte[] hashed = SHA256.HashData(bytes);

            // Return Base64 string
            return Convert.ToBase64String(hashed);
        }
    }
}
