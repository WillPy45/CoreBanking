using CoreBankingWebApplication.Models.Entities;

namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public interface IProfileDispatcher
    {
        Task<Profile?> GetProfile(ProfileSession profileSession);
    }
}
