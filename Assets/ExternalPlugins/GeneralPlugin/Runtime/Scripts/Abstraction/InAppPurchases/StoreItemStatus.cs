namespace Modules.General.Abstraction.InAppPurchase
{
    public enum StoreItemStatus
    {
        None = 0,

        /// <summary>
        /// The store item is ready and actual.
        /// </summary>
        Actual,

        /// <summary>
        /// Data of the store item is outdated since last request.
        /// </summary>
        Outdated,

        /// <summary>
        /// The store item marked as invalid by platform-native API. 
        /// </summary>
        Invalid,
    }
}
