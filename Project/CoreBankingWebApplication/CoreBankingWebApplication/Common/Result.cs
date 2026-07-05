using System.Diagnostics.CodeAnalysis;

namespace CoreBankingWebApplication.Common;

public static class Result
{
    public static Result<TValue, TErrorEnum> Fail<TValue, TErrorEnum>(TErrorEnum code, string message) where TErrorEnum : Enum
    {
        return Result<TValue, TErrorEnum>.Fail(code, message);
    }

    public static Result<TValue, TErrorEnum> Success<TValue, TErrorEnum>(TValue value) where TErrorEnum : Enum
    {
        return Result<TValue, TErrorEnum>.Success(value);
    }
}
