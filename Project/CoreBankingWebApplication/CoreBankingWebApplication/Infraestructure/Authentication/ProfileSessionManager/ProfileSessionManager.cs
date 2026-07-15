using CoreBankingWebApplication.Common;
using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Infraestructure.Authentication;
using CoreBankingWebApplication.Infraestructure.PasswordHashers;
using CoreBankingWebApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CoreBankingWebApplication.Infraestructure.Authentication;

using CreateProfileSessionResult = Result<ProfileSession, SessionCreateError>;

public class ProfileSessionManager : IProfileSessionManager
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionLifeTime _sessionLifeTime;
    private readonly ITokenGenerationStrategy _tokenGenerationStrategy;
    private readonly IProfileSessionRiskManager _profileSessionRiskManager;
    private readonly ISourceIdentifier _sourceIdentifier; 

    public ProfileSessionManager(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, ISessionLifeTime sessionLife, ITokenGenerationStrategy tokenGenerationStrategy, IProfileSessionRiskManager profileSessionRiskManager, ISourceIdentifier sourceIdentifier)
    {

        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _sessionLifeTime = sessionLife;
        _tokenGenerationStrategy = tokenGenerationStrategy;
        _profileSessionRiskManager = profileSessionRiskManager;
        _sourceIdentifier = sourceIdentifier;
    }
    public async Task<Result<ProfileSession, SessionCreateError>> CreateSession(LogToProfileRequest logToProfileRequest)
    {
        // Asociates with a Profile if exists
        string userName = logToProfileRequest.UserName;
        string password = logToProfileRequest.Password;
        string hashedPassword = _passwordHasher.HashPassword(password);
        Profile? profile = await _dbContext.Profiles.FirstOrDefaultAsync(e => e.UserName == userName);

        if (profile is null)
        {
            return CreateProfileSessionResult.Fail(SessionCreateError.ProfileNotFound, "Profile not found, session cant associate with a profile");
        }

        var trackedTrySession = new TrackedTrySession(profile.Id, _sourceIdentifier.Generate());
        var (isTriable, trackedTrySessions) = _profileSessionRiskManager.CheckIfTriable(trackedTrySession);

        if (!isTriable)
        {
            return CreateProfileSessionResult.Fail(SessionCreateError.SessionRiskManagerRestriction, $"Session restricted for this connection until {trackedTrySessions!.EndRestrictionUtc}");
        }


        if (profile.PasswordHash != hashedPassword)
        {
            trackedTrySessions = _profileSessionRiskManager.SubscribeToFailAttempts(trackedTrySession);

            return CreateProfileSessionResult.Fail(SessionCreateError.InvalidPassword, $"Invalid password (lock until {trackedTrySessions.EndRestrictionUtc}");
        }

        //Si pasa hasta aquí es una sesión aprobada

        _profileSessionRiskManager.UnSubscribeFromFailAttempt(trackedTrySession);

        DateTime dateTimeNow = DateTime.UtcNow;
        ProfileSession profileSession = new ProfileSession()
        {
            SessionToken = _tokenGenerationStrategy.GenerateToken(),
            SourceIdentifier = trackedTrySession.SourceIdentifier,
            AsociatedProfileId = profile.Id,
            SessionStartUtc = dateTimeNow,
            SessionEndUtc = dateTimeNow + _sessionLifeTime.Duration,
        };

        return CreateProfileSessionResult.Success(profileSession);

    }
}