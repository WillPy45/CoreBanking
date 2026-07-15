using CoreBankingWebApplication.Common;
using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Infraestructure.PasswordHashers;
using CoreBankingWebApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net;

namespace CoreBankingWebApplication.Infraestructure.Authentication
{
   
    public interface IProfileSessionManager
    {
        Task<Result<ProfileSession, SessionCreateError>> CreateSession(LogToProfileRequest logToProfileRequest);

    }

}
