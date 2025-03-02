using Modules.General.Abstraction;


namespace Modules.InAppPurchase
{
    internal class PurchaseAnalyticsDummy : IPurchaseAnalytics
    {
        public string AnalyticsUserId => "1530908707775-3432697";


        public string AnalyticsAppId => "";
        

        public void LogPurchase(
            string productId,
            string currencyCode,
            string price,
            string transactionId,
            string androidPurchaseDataJson = null,
            string androidPurchaseSignature = null,
            string androidPublicKey = null) { }
    }
}
