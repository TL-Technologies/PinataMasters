using Spine;
using Spine.Unity;
using UnityEngine;


namespace PinataMasters
{
    public class ShooterLegs : MonoBehaviour
    {
        #region Variables

        public static event System.Action OnFlip = delegate { };

        [SerializeField]
        private float border = 0f;
        [SerializeField]
        private Shooter shooter = null;

        [Header("Animation")]
        [SerializeField]
        private SkeletonAnimation legs = null;
        [SerializeField] [SpineAnimation(dataField: "legs")]
        private string WALK_FORWARD = null;
        [SerializeField] [SpineAnimation(dataField: "legs")]
        private string WALK_BACK = null;

        private float speed;
        private float direction;
        private bool atLeft;

        #endregion



        #region Properties

        public Transform Target { get; set; }

        public static float Offset { get; private set; } = 0f;

        public float Period { get { return (border * 4f) / PlayerConfig.GetSpeed(Player.SpeedLevel); } }

        #endregion



        #region Unity lifecycle

        private void Start()
        {
            Player.OnChangeShooterSpeed += ChangeSpeed;

            speed = PlayerConfig.GetSpeed(Player.SpeedLevel);
            direction = 1f;
            atLeft = true;

            legs.AnimationName = WALK_FORWARD;
            SetLegsSkin();
        }


        private void Update()
        {
            if (!shooter.IsMovingDisabled)
            {
                Moving();
                TryFlip();
            }
        }


        private void OnDestroy()
        {
            Player.OnChangeShooterSpeed -= ChangeSpeed;
        }

        #endregion



        #region Public methods

        public Vector3 GetTargetPosition()
        {
            return Target != null ? Target.position : Vector3.up;
        }


        public void SetDemoLegsSkin(int skin)
        {
            string animationName = legs.AnimationName;
            legs.skeletonDataAsset = Skins.GetLegsSkeletonAsset(skin);
            legs.Initialize(true);
            legs.AnimationState.SetAnimation(0, animationName, true);
        }


        public void SetLegsSkin()
        {
            string animationName = string.IsNullOrEmpty(legs.AnimationName) ? (WALK_FORWARD) : (legs.AnimationName);
            legs.skeletonDataAsset = Skins.GetLegsSkeletonAsset((shooter.IsShadow) ? (shooter.ShadowInfo.skin) : (Player.CurrentSkin));
            legs.Initialize(true);
            ShooterShadowsConfig.SetAnimationShader(legs, shooter.IsShadow);
            legs.AnimationState.SetAnimation(0, animationName, true);
        }

        #endregion



        #region Private methods

        private void TryFlip()
        {
            if ((atLeft && GetTargetPosition().x < transform.position.x) ||
                (!atLeft && GetTargetPosition().x > transform.position.x))
            {
                transform.localScale = new Vector3(atLeft ? -1f : 1f, 1f, 1f);
                legs.AnimationState.SetAnimation(0, legs.AnimationName == WALK_FORWARD ? WALK_BACK : WALK_FORWARD, true);
                atLeft = !atLeft;
                OnFlip();
            }

        }


        private void Moving()
        {
            float multipliedSpeed = speed * ShooterUpgradesBooster.asset.Value.MoveSpeedMultiplier;  
            float x = transform.localPosition.x + direction * multipliedSpeed * Time.deltaTime;

            if (x > border || x < -border)
            {
                direction *= -1f;
                legs.AnimationState.SetAnimation(0, legs.AnimationName == WALK_FORWARD ? WALK_BACK : WALK_FORWARD, true);
            }

            transform.Translate(direction * multipliedSpeed * Time.deltaTime * Vector3.right);

            Offset = transform.localPosition.x / border;
        }


        private void ChangeSpeed()
        {
            speed = PlayerConfig.GetSpeed(Player.SpeedLevel);
        }

        #endregion
    }
}
