public enum LLPromo
{
    PriorityPromo = 0,
}


public enum LLPromoType
{
    Like    		= 0,
    Comment 		= 1,
	Subscription 	= 2,
}


public enum LLPromoVisitType
{
    Opened  		= 0,
    Skipped 		= 1
}


public enum LLPromoPlacementType
{
    PeriodicalShow = 0,
    ElementOpen = 1
}


public struct LLPromoFetcherUnit
{
    public string promoURL;
    public LLPromoType promoType;
    public LLPromoPlacementType placement;
};


public sealed class LLPromoFetchManager
{
    #region Variables
    
    public const string PLACEMENT_PERIODICAL_SHOW = "periodical_show";
    public const string PLACEMENT_ELEMENT_OPEN = "element_open";
    
    private static IPromoFetchUnitsProvider unitsProvider;
    private static LLPromoFetcherUnit activePromo;

    #endregion



    #region Public methods

	public static void Initialize(IPromoFetchUnitsProvider provider = null)
    {
        if (provider != null)
        {
            unitsProvider = provider;
        }
        else
        {
            unitsProvider = new OfflinePromoFetchUnitsProvider();
        }
    }


    public static LLPromoFetcherUnit PromoFetcherActiveUnit(string placement = PLACEMENT_PERIODICAL_SHOW, LLPromo promo = LLPromo.PriorityPromo)
    {
        LLPromoPlacementType placementType = GetPlacementType(placement);
        activePromo = unitsProvider.GetUnvisitedPromo(placementType);
        return activePromo;
    }
    
    
    public static void VisitActiveUnit(LLPromoVisitType visitType, string placement = PLACEMENT_PERIODICAL_SHOW, LLPromo promo = LLPromo.PriorityPromo)
    {
        if (visitType == LLPromoVisitType.Opened)
        {
            unitsProvider.SetLinkVisitedStatus(activePromo.promoURL, true);
        }
    }
    
    #endregion
    
    
    
    #region Private methods
    
    private static LLPromoPlacementType GetPlacementType(string placement)
    {
        switch (placement)
        {
            case PLACEMENT_ELEMENT_OPEN:
                return LLPromoPlacementType.ElementOpen;
            case PLACEMENT_PERIODICAL_SHOW:
                return LLPromoPlacementType.PeriodicalShow;
            default:
                CustomDebug.LogWarning($"Wrong placement name {placement}");
                return LLPromoPlacementType.PeriodicalShow;
        }
    }
    
    #endregion
}
