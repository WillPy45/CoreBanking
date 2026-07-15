using System.Net;

namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public class SessionResponseDto
    {
        public required string SessionToken { get; set; }
        public required int AsociatedProfileId { get; set; }
        public required DateTime SessionStartUtc { get; set; }
        public required DateTime SessionEndUtc { get; set; }
    }
}

