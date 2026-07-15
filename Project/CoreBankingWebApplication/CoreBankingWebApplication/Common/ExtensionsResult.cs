namespace CoreBankingWebApplication.Common;

/// <summary>
/// Provides static helper methods to create <see cref="Result{TValue, TErrorEnum}"/> objects.
/// </summary>
public static class Result
{
    /// <summary>
    /// Creates a failure result with the specified error code and message.
    /// </summary>
    /// <typeparam name="TValue">The type of the value expected in a success case.</typeparam>
    /// <typeparam name="TErrorEnum">The enum type representing possible error codes.</typeparam>
    /// <param name="code">The error code.</param>
    /// <param name="message">A descriptive error message.</param>
    /// <returns>A failure <see cref="Result{TValue, TErrorEnum}"/> object.</returns>
    public static Result<TValue, TErrorEnum> Fail<TValue, TErrorEnum>(TErrorEnum code, string message) where TErrorEnum : Enum
    {
        return Result<TValue, TErrorEnum>.Fail(code, message);
    }

    /// <summary>
    /// Creates a success result with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned in a success case.</typeparam>
    /// <typeparam name="TErrorEnum">The enum type representing possible error codes.</typeparam>
    /// <param name="value">The value to wrap in the result.</param>
    /// <returns>A success <see cref="Result{TValue, TErrorEnum}"/> object.</returns>
    public static Result<TValue, TErrorEnum> Success<TValue, TErrorEnum>(TValue value) where TErrorEnum : Enum
    {
        return Result<TValue, TErrorEnum>.Success(value);
    }
}
