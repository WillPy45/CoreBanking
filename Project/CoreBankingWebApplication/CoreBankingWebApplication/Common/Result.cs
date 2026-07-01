using System.Diagnostics.CodeAnalysis;

namespace CoreBankingWebApplication.Common
  {

    public class Result<T> : IResult<T>
    {
        
        public T? Value {get;}

        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsSuccess {get;}

        [MemberNotNullWhen(true, nameof(Error))]
        public bool IsFailure => !IsSuccess;
    
        public Error? Error {get; }

        // If sucess !!
        public Result(T value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsSuccess = true;
            Error = null;
        }

        // If failure
        public Result(Error error)
        {
            Value = default;
            IsSuccess = false;
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }
        // Methods to generate common results

        // Returns a failure Result object
        public static Result<T> Fail(Error error) => new Result<T>(error);


        // Returns a sucessfull Result object
        public static Result<T> Success(T value) => new Result<T>(value);
    
    }
}