using System.Diagnostics.CodeAnalysis;

namespace CoreBankingWebApplication.Common;


public class Result<TValue, TErrorEnum> : IResult<TValue, TErrorEnum>
where TErrorEnum : Enum
{

    public TValue? Value { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailure => !IsSuccess;

    public Error<TErrorEnum>? Error { get; }

    // If sucess !!
    public Result(TValue value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsSuccess = true;
        Error = null;
    }

    // If failure
    public Result(TErrorEnum code, string message)
    {
        Value = default;
        IsSuccess = false;
        Error = new Error<TErrorEnum>(code, message);
    }
    // Methods to generate common results

    // Returns a failure Result object
    public static Result<TValue, TErrorEnum> Fail(TErrorEnum code, string message) => new Result<TValue, TErrorEnum>(code, message);


    // Returns a sucessfull Result object
    public static Result<TValue, TErrorEnum> Success(TValue value) => new Result<TValue, TErrorEnum>(value);

}
