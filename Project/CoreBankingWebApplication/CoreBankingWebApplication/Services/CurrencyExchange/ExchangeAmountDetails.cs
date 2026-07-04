namespace CoreBankingWebApplication.Services
{
    public class ExchangeAmountDetails
    {
        public string FromCurrencyCode { get;} // f.e: PYG 
        public decimal SourceAmount {get; } // f.e: PYG 200000

        public string ToCurrencyCode {get;} // f.e: USD
        public decimal BankUnitPrice { get; } 

        public decimal GrossAmount { get; } // Before discounts
        public decimal BankDiscountInPercentage { get; }
        public decimal BankDiscount {get; }
        public decimal NetAmount {get;} // After discounts

        public ExchangeAmountDetails(string fromCurrencyCode, decimal sourceAmount, string toCurrencyCode, decimal rate, decimal bankDiscountInPercentage)
        {
            FromCurrencyCode = fromCurrencyCode;
            SourceAmount = sourceAmount;
            ToCurrencyCode = toCurrencyCode;

            BankUnitPrice = rate != 0 ? 1 / rate : 0;
            GrossAmount = sourceAmount * rate;

            BankDiscount = GrossAmount * bankDiscountInPercentage;
            NetAmount = GrossAmount - BankDiscount;
        }

    }
}
