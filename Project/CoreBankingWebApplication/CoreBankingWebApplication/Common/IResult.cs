namespace CoreBankingWebApplication.Common;

public interface IResult<TValue, TErrorEnum>
where TErrorEnum : Enum
{
    TValue? Value { get; }           // The actual data if the operation was successful
    bool IsSuccess { get; }     // Did everything go well?
    bool IsFailure { get; }     // Was there a business logic error?
    Error<TErrorEnum>? Error { get; }       // The error code and message explaining the failure reason
}
