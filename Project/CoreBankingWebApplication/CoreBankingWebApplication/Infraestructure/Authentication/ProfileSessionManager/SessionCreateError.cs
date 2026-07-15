namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public enum SessionCreateError
    {
        ProfileNotFound,
        InvalidPassword,
        SessionRiskManagerRestriction
    }
}
