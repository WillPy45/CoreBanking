namespace CoreBankingWebApplication.Models
{
    // Data transfer object for represents the final result of an exchange operation
    // This DTO will to project data and returns by a service
    public class CurrencyExchangeDetailsResponseDto
    {
        public int CurrencyExchangeId { get; set; } // The record pk

        public int SenderBankAccountId { get; set; } 

        public int ReceiverBankAccountId {get; set; } 

        public string? FromCurrencyCode { get; set; } // f.e: PYG 
        public decimal SourceAmount {get; set; } // f.e: PYG 200000

        public string? ToCurrencyCode {get; set;} // f.e: USD
        public decimal BankUnitPrice { get; set; } 

        public decimal GrossAmount { get; set; } // Before discounts
        public decimal BankDiscountInPercentage { get; set; }
        public decimal BankDiscount {get; set;}
        public decimal NetAmount {get; set; } // After discounts

        public decimal Amount { get; set; }

        public DateTime CreateAt { get; set; }

    }
}