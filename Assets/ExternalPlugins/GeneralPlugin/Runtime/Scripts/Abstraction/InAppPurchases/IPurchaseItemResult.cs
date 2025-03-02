namespace Modules.General.Abstraction.InAppPurchase
{
    public interface IPurchaseItemResult
    {
        PurchaseItemResultCode ResultCode { get; }
        string Message { get; }
        IStoreItem StoreItem { get; }
        string TransactionId { get; }
        PurchaseTransactionState TransactionState { get; }
        PurchaseValidationState ValidationState { get; }

        bool IsSucceeded { get; }
        bool IsValidated { get; }
    }
}
