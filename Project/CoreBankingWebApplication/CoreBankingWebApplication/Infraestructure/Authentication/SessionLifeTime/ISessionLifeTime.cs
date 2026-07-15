namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public interface ISessionLifeTime
    {
        TimeSpan Duration { get; set; }
    }

}
