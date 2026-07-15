namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public interface ITokenGenerationStrategy
    {
        string GenerateToken();
    }
}
