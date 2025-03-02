using PinataMasters;


public class CustomAdPlacementType
{
    //rewarded
    public const string LevelResult = "LevelResult";
    public const string OfflineReward = "OfflineReward";
    public const string IngameReward = "IngameReward";
    public const string MiniBank = "MiniBank";
    
    //offers
    public const string FreeCoinsBonus = "FreeCoinsBonus";
    public const string VideoCoinsBonus = "VideoCoinsBonus";
    public const string X2CoinsBonus = "X2CoinsBonus";
    public const string GemsBonus = "GemsBonus";
    public const string SpeedBonus = "SpeedBonus";
    
    
    public static string GetIngameOfferPlacement(IngameOfferType offerType)
    {
        string result = string.Empty;

        switch (offerType)
        {
            case IngameOfferType.FreeCoins:
            {
                result = FreeCoinsBonus;

                break;
            }

            case IngameOfferType.CoinBonus:
            {
                result = VideoCoinsBonus;

                break;
            }

            case IngameOfferType.GemsBonus:
            {
                result = GemsBonus;

                break;
            }

            case IngameOfferType.SpeedBonus:
            {
                result = SpeedBonus;

                break;
            }
            case IngameOfferType.X2CoinBonus:
            {
                result = X2CoinsBonus;

                break;
            }
        }

        return result;
    }
}
