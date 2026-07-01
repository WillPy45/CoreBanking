  namespace CoreBankingWebApplication.Common
  {

    public class Result<T> : IResult<T>
    {
        
        public T? Value {get;}

        public bool IsSuccess {get;}

        public bool IsFailure {get; } 
    
        public Error? Error {get; }


        public Result(T? value, bool isSuccess, bool isFailure, Error? error)
        {
            Value = value;
            IsSuccess = isSuccess;
            IsFailure = isFailure;
            Error = error;
        }

        // Methods to generate common results

        // Returns a failure Result object
        public static Result<T> Fail(Error error)
        {
            return new Result<T>(
                value: default,
                isSuccess: false,
                isFailure: true,
                error: error
            );
               
        }

        // Returns a sucessfull Result object
        public static Result<T> Success(T value)
        {
            return new Result<T>(
                value: value,
                isSuccess: true,
                isFailure: false,
                error: null
            );
        }
    }
}