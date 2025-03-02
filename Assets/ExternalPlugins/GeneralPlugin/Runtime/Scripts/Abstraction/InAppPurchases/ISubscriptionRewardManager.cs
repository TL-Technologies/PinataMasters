namespace Modules.General.Abstraction.InAppPurchase
{
    public interface ISubscriptionRewardManager
    { 
        bool IsRewardPopupAvailable { get; }
        
        int SubscriptionDaysForReward { get; }

        void ClaimReward();
    }
}
