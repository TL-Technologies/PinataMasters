using System;


namespace Modules.General.Abstraction
{
    public interface ICrossPromoTracker
    {
        string CampaignName { get; }

        void Initialize();

        void TrackImpression(ICrossPromoAdData data, string placement, string version);

        void TrackClickAndOpenStore(ICrossPromoAdData data, string placement, string version);

        void TrackAdShow(string eventName, string status, ICrossPromoAdData data, string isWatched = null);

        event Action OnConversionDataReceived;
    }
}