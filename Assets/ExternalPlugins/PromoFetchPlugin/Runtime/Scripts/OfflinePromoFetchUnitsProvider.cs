using Modules.General.HelperClasses;
using System.Collections.Generic;


public class OfflinePromoFetchUnitsProvider : IPromoFetchUnitsProvider
{
    public List<LLPromoFetcherUnit> GetUnits()
    {
        return PromoFetchUnits.Instance.promoUnits;
    }
    
    
    public LLPromoFetcherUnit GetUnvisitedPromo(LLPromoPlacementType placement)
    {
        LLPromoFetcherUnit result = TryGetUnvisitedPromo();

        if (result.promoURL.IsNullOrEmpty())
        {
            MarkPromosAsUnvisited(placement);
            result = TryGetUnvisitedPromo();
        }

        return result;
        
        LLPromoFetcherUnit TryGetUnvisitedPromo()
        {
            foreach (LLPromoFetcherUnit promo in GetUnits())
            {
                if (promo.placement == placement && !WasLinkVisited(promo.promoURL))
                {
                    return promo;
                }
            }
        
            return new LLPromoFetcherUnit();
        }
    }

    
    public void MarkPromosAsUnvisited(LLPromoPlacementType placement)
    {
        foreach (LLPromoFetcherUnit promo in GetUnits())
        {
            if (promo.placement == placement)
            {
                SetLinkVisitedStatus(promo.promoURL, false);
            }
        } 
    }
        
    
    public bool WasLinkVisited(string link)
    {
        return CustomPlayerPrefs.GetBool(link, false);
    }

    
    public void SetLinkVisitedStatus(string link, bool wasVisited)
    {
        CustomPlayerPrefs.SetBool(link, wasVisited);
    }
}
