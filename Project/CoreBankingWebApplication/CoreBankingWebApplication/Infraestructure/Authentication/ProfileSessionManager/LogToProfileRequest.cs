namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public class LogToProfileRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
