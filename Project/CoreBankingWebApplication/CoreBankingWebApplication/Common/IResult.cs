namespace CoreBankingWebApplication.Common
{
    public interface IResult<T>
    {
        T? Value { get; }           // The actual data if the operation was successful
        bool IsSuccess { get; }     // Did everything go well?
        bool IsFailure { get; }     // Was there a business logic error?
        string Error { get; }       // The message explaining the failure reason
    }
}