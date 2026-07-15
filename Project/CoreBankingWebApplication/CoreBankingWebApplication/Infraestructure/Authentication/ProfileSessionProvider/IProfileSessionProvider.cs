using System.Net;

namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public interface IProfileSessionProvider
    {
        void AddSession(ProfileSession profileSession);
        bool DeleteSession(ProfileSession profileSession);
        bool DeleteSession(string sessionTokenr);
        void CleanExpiredSessions();
        ProfileSession? GetSession(string sessionToken);
    }
}
