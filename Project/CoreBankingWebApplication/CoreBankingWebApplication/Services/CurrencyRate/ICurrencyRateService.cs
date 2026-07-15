using CoreBankingWebApplication.Common;

namespace CoreBankingWebApplication.Services;

public interface ICurrencyRateService
{
    Task<Result<decimal, CurrencyRateServiceError>> GetConversionRateAsync(string fromCurrencyCode, string toCurrencyCode);

}

