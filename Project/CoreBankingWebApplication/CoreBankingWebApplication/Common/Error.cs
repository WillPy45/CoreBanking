namespace CoreBankingWebApplication.Common
{
    public class Error<TErrorEnum>
    {
        public TErrorEnum Code { get; }
        public string Message { get; }

        public Error(TErrorEnum code, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}