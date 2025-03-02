using Modules.General.HelperClasses;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PromoFetchUnits")]
public class PromoFetchUnits : ScriptableSingleton<PromoFetchUnits>
{
    public List<LLPromoFetcherUnit> promoUnits;
}

