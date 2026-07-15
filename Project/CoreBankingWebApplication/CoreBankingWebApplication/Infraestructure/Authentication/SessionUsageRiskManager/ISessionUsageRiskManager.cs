using CoreBankingWebApplication.Common;

namespace CoreBankingWebApplication.Infraestructure.Authentication.SessionUsageRiskManager
{
    public interface ISessionUsageRiskManager
    {
        Result<SessionUsageError> HandleRisk(ProfileSession session, string sourceIdentifier);
        
    }

    public enum SessionUsageError
    {
        SourceSuspicious
    }

}
