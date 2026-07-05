namespace CoreBankingWebApplication.Services;

public enum TransferBalanceBetweenAccountError
{
    SameBankAccount,
    NegativeOrZeroAmount,
    SenderBankAccountNotFound,
    ReceiverBankccountNotFound,
    CurrencyDiffers,
    NotEnoughBalance


}

