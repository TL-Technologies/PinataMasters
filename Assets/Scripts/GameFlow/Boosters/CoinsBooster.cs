using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "CoinsBooster", menuName = "CoinsBooster")]
    public class CoinsBooster : BaseBooster 
    {
        #region Fields

        public static readonly ResourceAsset<CoinsBooster> asset = new ResourceAsset<CoinsBooster>("Game/Boosters/CoinsBooster");

        #endregion



        #region Properties

        public uint CoinsMultiplier => (CurrentBoosterState == BoosterState.Active) ? (2u) : (1u);

        #endregion
    }
}
