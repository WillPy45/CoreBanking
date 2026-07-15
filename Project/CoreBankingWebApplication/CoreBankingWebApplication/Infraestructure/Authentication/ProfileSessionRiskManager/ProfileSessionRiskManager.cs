using CoreBankingWebApplication.Infraestructure.Authentication;
using System.Collections.Concurrent;

public class ProfileSessionRiskManager : IProfileSessionRiskManager
{
    private readonly ConcurrentDictionary<TrackedTrySession, TrackedTrySessions> _trackedTrySessionsDict = new ConcurrentDictionary<TrackedTrySession, TrackedTrySessions>();

    public (bool IsTriable, TrackedTrySessions? Details) CheckIfTriable(TrackedTrySession trackedTrySession)
    {
        DateTime dateTimeNow = DateTime.UtcNow;

        if (!_trackedTrySessionsDict.TryGetValue(trackedTrySession, out var trackedSessions))
        {
            return (true, null);
        }

        if (dateTimeNow < trackedSessions.EndRestrictionUtc)
        {
            return (false, trackedSessions);
        }

        return (true, trackedSessions);
    }

    public TrackedTrySessions SubscribeToFailAttempts(TrackedTrySession trackedTrySession)
    {
        TimeSpan firstRestrictionTime = TimeSpan.FromMinutes(1);
        DateTime dateTimeUtcNow = DateTime.UtcNow;

        _trackedTrySessionsDict.TryGetValue(trackedTrySession, out var trackedSessions);

        // Primer intento y fallo
        if (trackedSessions is null)
        {

            trackedSessions = new TrackedTrySessions()
            {
                TrackedSession = trackedTrySession,
                Attempts = 1,
                LastRestrictionTimeSpan = firstRestrictionTime,
                StartRestrictionUtc = dateTimeUtcNow,
                EndRestrictionUtc = dateTimeUtcNow + firstRestrictionTime,
            };

            _trackedTrySessionsDict[trackedSessions.TrackedSession] = trackedSessions;
            return trackedSessions;

        }

        // Restricción activa
        if (dateTimeUtcNow < trackedSessions.EndRestrictionUtc)
        {
            return trackedSessions;
        }

        // Fallo posterior e incremento del bloqueo
        TimeSpan restrictionTime = trackedSessions.LastRestrictionTimeSpan * 2;
        trackedSessions.Attempts++;
        trackedSessions.LastRestrictionTimeSpan = restrictionTime;
        trackedSessions.StartRestrictionUtc = dateTimeUtcNow;
        trackedSessions.EndRestrictionUtc = dateTimeUtcNow + restrictionTime;

        return trackedSessions;

    }

    public void UnSubscribeFromFailAttempt(TrackedTrySession trackedTrySession)
    {
        _trackedTrySessionsDict.TryRemove(trackedTrySession, out _);
    }
}