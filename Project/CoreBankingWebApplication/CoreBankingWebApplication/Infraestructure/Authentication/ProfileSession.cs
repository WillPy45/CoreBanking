using System.Net;

namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public class ProfileSession
    {
        public required string SessionToken { get; set; }
        public required string SourceIdentifier { get; set; }
        public required int AsociatedProfileId { get; set; }
        public required DateTime SessionStartUtc { get; set; }
        public required DateTime SessionEndUtc { get; set; }
    }
}
