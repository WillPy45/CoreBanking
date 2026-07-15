namespace CoreBankingWebApplication.Common;

/// <summary>
/// Defines the contract for a result object that can represent either success or failure.
/// </summary>
/// <typeparam name="TValue">The type of the value returned in a success case.</typeparam>
/// <typeparam name="TErrorEnum">The enum type representing possible error codes.</typeparam>
public interface IResult<TValue, TErrorEnum>
where TErrorEnum : Enum
{
    /// <summary>
    /// Gets the value when the result is successful; otherwise <c>null</c>.
    /// </summary>
    TValue? Value { get; }

    /// <summary>
    /// Indicates whether the result represents a success.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the result represents a failure.
    /// </summary>
    bool IsFailure { get; }

    /// <summary>
    /// Gets the error details when the result is a failure; otherwise <c>null</c>.
    /// </summary>
    Error<TErrorEnum>? Error { get; }
}