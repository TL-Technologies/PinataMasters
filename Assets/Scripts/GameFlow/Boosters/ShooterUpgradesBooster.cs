using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "ShooterUpgradesBooster", menuName = "ShooterUpgradesBooster")]
    public class ShooterUpgradesBooster : BaseBooster 
    {
        #region Fields

        public static readonly ResourceAsset<ShooterUpgradesBooster> asset = new ResourceAsset<ShooterUpgradesBooster>("Game/Boosters/ShooterUpgradesBooster");

        [SerializeField] float moveSpeedMultiplier = 0.0f;
        [SerializeField] float timeReloadMultiplier = 0.0f;
        [SerializeField] float bulletsImpulseMultiplier = 0.0f;

        #endregion



        #region Properties

        public float MoveSpeedMultiplier => (CurrentBoosterState == BoosterState.Active) ? (moveSpeedMultiplier) : (1.0f);


        public float TimeReloadMultiplier => (CurrentBoosterState == BoosterState.Active) ? (timeReloadMultiplier) : (1.0f);


        public float BulletsImpulseMultiplier => (CurrentBoosterState == BoosterState.Active) ? (bulletsImpulseMultiplier) : (1.0f);

        #endregion
    }
}
