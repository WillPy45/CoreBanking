namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public class HttpRequestBasedTokenGeneration : ITokenGenerationStrategy
    {
        private readonly HttpRequest _request;
        private readonly IProfileSessionProvider _profileSessionProvider;
        public HttpRequestBasedTokenGeneration(HttpRequest httpRequest, IProfileSessionProvider profileSessionProvider) 
        {
            _request = httpRequest;
            _profileSessionProvider = profileSessionProvider;
        }

        string ITokenGenerationStrategy.GenerateToken()
        {
            

            throw new NotImplementedException();
        }
    }
}
