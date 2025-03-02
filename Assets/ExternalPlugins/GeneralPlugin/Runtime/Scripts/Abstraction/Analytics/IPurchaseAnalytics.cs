namespace Modules.General.Abstraction
{
    public interface IPurchaseAnalytics
    {
        string AnalyticsUserId { get; }
        
        
        string AnalyticsAppId { get; }
        
        
        void LogPurchase(
            string productId,
            string currencyCode,
            string price,
            string transactionId,
            string androidPurchaseDataJson = null,
            string androidPurchaseSignature = null,
            string androidPublicKey = null);
    }
}
