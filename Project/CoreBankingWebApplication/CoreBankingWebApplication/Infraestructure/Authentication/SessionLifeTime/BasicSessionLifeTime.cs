namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    public class BasicSessionLifeTime : ISessionLifeTime
    {
        public TimeSpan Duration { 
            get => TimeSpan.FromMinutes(10);
            set => throw new NotImplementedException();
        }
    }
}
