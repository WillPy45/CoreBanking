using System.Diagnostics.CodeAnalysis;

namespace CoreBankingWebApplication.Common
{
    public class Result<TErrorEnum> where TErrorEnum : Enum
    {
        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; }

        /// <inheritdoc/>   
        [MemberNotNullWhen(true, nameof(Error))]
        public bool IsFailure => !IsSuccess;

        public Error<TErrorEnum>? Error { get; }

        public Result()
        {
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
            IsSuccess = false;
            Error = new Error<TErrorEnum>(code, message);
        }

        // Methods to generate common results

        /// <summary>
        /// Creates a failure result with the specified error code and message.
        /// </summary>
        public static Result<TErrorEnum> Fail(TErrorEnum code, string message) => new Result<TErrorEnum>(code, message);


        public static Result<TErrorEnum> Success() => new Result<TErrorEnum>();

    }
}
