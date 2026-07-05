namespace CoreBankingWebApplication.Services;

public enum CurrencyExchangeServiceError
{
    SameBankAccounts,
    NegativeOrZeroAmount,
    SenderBankAccountNotFound,
    SameCurrencyType,
    ReceiverBankAccountNotFound,
    NotEnoughBalance,
    HasNotReserveForAcceptTheCurrencies,
    HasNotReserveForDepositTheCurrencies,
    ConversionFailed,
    HasNotEnoughBalanceInCurrencyReserve,




}

