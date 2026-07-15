using System.Net;

namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public class TrackedTrySessions
    {
        public required TrackedTrySession TrackedSession { get; set; }
        public int Attempts { get; set; }
        public TimeSpan LastRestrictionTimeSpan { get; set; }
        public DateTime StartRestrictionUtc { get; set; }
        public DateTime EndRestrictionUtc { get; set; }

    }

    public record TrackedTrySession(int ProfileId, string SourceIdentifier);

}
