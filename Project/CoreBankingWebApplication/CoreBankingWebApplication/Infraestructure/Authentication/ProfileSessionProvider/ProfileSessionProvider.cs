
using CoreBankingWebApplication.Infraestructure.Authentication;
using System.Collections.Concurrent;

public class ProfileSessionProvider : IProfileSessionProvider
{

    private readonly ConcurrentDictionary<string, ProfileSession> _profileSessions = new();
    public void AddSession(ProfileSession profileSession)
    {
        _profileSessions[profileSession.SessionToken] = profileSession;
    }

    public void CleanExpiredSessions()
    {
        DateTime dateTimeNow = DateTime.UtcNow;

        var expiredSessions = _profileSessions
            .Where(s => s.Value.SessionEndUtc < dateTimeNow)
            .Select(s => s.Key)
            .ToList();

        foreach (var key in expiredSessions)
        {
            _profileSessions.TryRemove(key, out _);
        }
    }


    public bool DeleteSession(ProfileSession profileSession)
    {
        return _profileSessions.Remove(profileSession.SessionToken, out var value);
    }

    public bool DeleteSession(string sessionToken)
    {
        return _profileSessions.Remove(sessionToken, out var value);
    }
    public ProfileSession? GetSession(string sessionToken)
    {
        _profileSessions.TryGetValue(sessionToken, out var session);

        if (session is null)
        {
            return null;
        }

        if (DateTime.UtcNow > session.SessionEndUtc)
        {
            DeleteSession(session);
            return null;
        }

        return session;
    }


}