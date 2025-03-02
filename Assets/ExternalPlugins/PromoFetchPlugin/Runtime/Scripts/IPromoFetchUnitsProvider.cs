using System.Collections.Generic;


public interface IPromoFetchUnitsProvider
{
    List<LLPromoFetcherUnit> GetUnits();
    LLPromoFetcherUnit GetUnvisitedPromo(LLPromoPlacementType placement);
    void MarkPromosAsUnvisited(LLPromoPlacementType placement);
    bool WasLinkVisited(string link);
    void SetLinkVisitedStatus(string link, bool wasVisited);
}
