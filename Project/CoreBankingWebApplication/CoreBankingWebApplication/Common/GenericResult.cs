using System.Diagnostics.CodeAnalysis;

namespace CoreBankingWebApplication.Common;

/// <summary>
/// Represents the outcome of an operation, which can be either success with a value or failure with an error.
/// </summary>
/// <typeparam name="TValue">The type of the value returned in a success case.</typeparam>
/// <typeparam name="TErrorEnum">The enum type representing possible error codes.</typeparam>
public class Result<TValue, TErrorEnum> : IResult<TValue, TErrorEnum>
where TErrorEnum : Enum
{
    /// <inheritdoc/>
    public TValue? Value { get; }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <inheritdoc/>   
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc/>
    public Error<TErrorEnum>? Error { get; }


    /// <summary>
    /// Initializes a new successful result with the specified value.
    /// </summary>
    /// <param name="value">The value to wrap in the result.</param>
    public Result(TValue value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsSuccess = true;
        Error = null;
    }

    /// <summary>
    /// Initializes a new failure result with the specified error code and message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">A descriptive error message.</param>
    public Result(TErrorEnum code, string message)
    {
        Value = default;
        IsSuccess = false;
        Error = new Error<TErrorEnum>(code, message);
    }
    // Methods to generate common results


    /// <summary>
    /// Creates a failure result with the specified error code and message.
    /// </summary>
    public static Result<TValue, TErrorEnum> Fail(TErrorEnum code, string message) => new Result<TValue, TErrorEnum>(code, message);


    /// <summary>
    /// Creates a success result with the specified value.
    /// </summary>
    public static Result<TValue, TErrorEnum> Success(TValue value) => new Result<TValue, TErrorEnum>(value);

}
