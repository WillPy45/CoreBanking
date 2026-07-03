namespace CoreBankingWebApplication.Services
{
    public enum CurrencyExchangeServiceError
    {
        SameBankAccounts,
        NegativeOrZeroAmount,
        SenderBankAccountNotFound,
        receiverBankAccountNotFound,
        NotEnoughBalance,
        HasNotReserveForAcceptTheCurrencies,
        HasNotReserveForDepositTheCurrencies,
        ConversionFailed,
        HasNotEnoughBalanceInCurrencyReserve,
        



    }
}
