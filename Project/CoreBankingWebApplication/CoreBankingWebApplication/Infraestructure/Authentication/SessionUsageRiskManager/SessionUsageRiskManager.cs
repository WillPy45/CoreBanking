using CoreBankingWebApplication.Common;

namespace CoreBankingWebApplication.Infraestructure.Authentication.SessionUsageRiskManager
{
    public class SessionUsageRiskManager : ISessionUsageRiskManager
    {

        public Result<SessionUsageError> HandleRisk(ProfileSession session, string usageSourceIdentifier)
        {
            if (usageSourceIdentifier != session.SourceIdentifier)
            {
                return Result<SessionUsageError>.Fail(SessionUsageError.SourceSuspicious, "Source suspicious");
            } 

            return Result<SessionUsageError>.Success();

        }
    }
}
