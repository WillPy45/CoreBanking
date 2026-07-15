namespace CoreBankingWebApplication.Common;

/// <summary>
/// Represents an error result with a code and descriptive message.
/// </summary>
/// <typeparam name="TErrorEnum">
/// The enum type that defines the possible error codes for the operation.
/// </typeparam
public class Error<TErrorEnum>
where TErrorEnum : Enum
{

    /// <summary>
    /// Gets the error code that identifies the type of failure.
    /// </summary>
    public TErrorEnum Code { get; }


    /// <summary>
    /// Gets the descriptive message associated with the error.
    /// </summary>
    public string Message { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="Error{TErrorEnum}"/> class.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">A descriptive error message.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="code"/> or <paramref name="message"/> is null.
    /// </exception>
    public Error(TErrorEnum code, string message)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
