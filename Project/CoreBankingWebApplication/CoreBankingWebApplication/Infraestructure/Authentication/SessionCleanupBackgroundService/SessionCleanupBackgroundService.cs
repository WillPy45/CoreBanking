using CoreBankingWebApplication.Infraestructure.Authentication;

public class SessionCleanupBackgroundService : BackgroundService
{
    private readonly IProfileSessionProvider _sessionProvider;
    public SessionCleanupBackgroundService(IProfileSessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            _sessionProvider.CleanExpiredSessions();

        }

    }
}
