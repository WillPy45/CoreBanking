using System.Diagnostics.CodeAnalysis;

namespace CoreBankingWebApplication.Common
  {

    public class Result<TValue, TErrorEnum> : IResult<TValue, TErrorEnum>

    {
        
        public TValue? Value {get;}

        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsSuccess {get;}

        [MemberNotNullWhen(true, nameof(Error))]
        public bool IsFailure => !IsSuccess;
    
        public Error<TErrorEnum>? Error {get; }

        // If sucess !!
        public Result(TValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsSuccess = true;
            Error = null;
        }

        // If failure
        public Result(Error<TErrorEnum> error)
        {
            Value = default;
            IsSuccess = false;
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }
        // Methods to generate common results

        // Returns a failure Result object
        public static Result<TValue, TErrorEnum> Fail(Error<TErrorEnum> error) => new Result<TValue, TErrorEnum>(error);


        // Returns a sucessfull Result object
        public static Result<TValue, TErrorEnum> Success(TValue value) => new Result<TValue, TErrorEnum>(value);
    
    }
}