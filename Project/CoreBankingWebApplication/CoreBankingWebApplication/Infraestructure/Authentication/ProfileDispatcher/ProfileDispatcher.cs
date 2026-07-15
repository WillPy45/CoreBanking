using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Infraestructure.Authentication;
using CoreBankingWebApplication.Models.Entities;

public class ProfileDispatcher : IProfileDispatcher
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IProfileSessionProvider _profileSessionProvider;

    public ProfileDispatcher(ApplicationDbContext dbContext, IProfileSessionProvider profileSessionProvider)
    {
        _dbContext = dbContext;
        _profileSessionProvider = profileSessionProvider;
    }
    public async Task<Profile?> GetProfile(ProfileSession profileSession)
    {
        ProfileSession? profileSessionProvided = _profileSessionProvider.GetSession(profileSession.SessionToken);

        if (profileSessionProvided is null)
        {
            return null;
        }

        return await _dbContext.Profiles.FindAsync(profileSession.AsociatedProfileId);
    }
}