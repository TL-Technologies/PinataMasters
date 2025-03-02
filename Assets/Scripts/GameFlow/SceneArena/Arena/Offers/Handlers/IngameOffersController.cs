using UnityEngine;


namespace PinataMasters
{
    public class IngameOffersController : MonoBehaviour
    {
        #region Fields

        public static readonly ResourceGameObject<IngameOffersController> Prefab = new ResourceGameObject<IngameOffersController>("Game/Game/Offers/_Common/IngameOffersController");

        [SerializeField] IngameOffersSettings offersSettings;

        IngameOfferHandler activeOfferHandler;

        int offersUpdateBlockers;
        int offersTouchesBlockers;

        #endregion



        #region Properties

        public bool IsOfferUpdateAvailable => (offersUpdateBlockers == 0);


        public bool IsOfferTouchesAvailable => (offersTouchesBlockers == 0);

        #endregion



        #region Unity lifecycle

        void Update()
        {
            if (IsOfferUpdateAvailable)
            {
                activeOfferHandler?.CustomUpdate(Time.unscaledDeltaTime);
            }
            else if (ABTest.InGameAbTestData.boosterUseRealTime)
            {
                activeOfferHandler?.CustomUpdate(Time.unscaledDeltaTime, true);
            }
        }

        #endregion



        #region Public methods

        public void Configure()
        {
            if (Player.Level >= offersSettings.LevelForUnlockOffers || Player.PresigeLevel > 0)
            {
                if (activeOfferHandler == null)
                {
                    activeOfferHandler = new IngameOfferHandler(this, offersSettings);
                }
                else
                {
                    activeOfferHandler.ConfigureHandler(this, offersSettings);
                }
            }
            else
            {
                activeOfferHandler = null;
            }
        }


        public void Block(IngameOffersBlocker.BlockingType blockType)
        {
            if ((blockType & IngameOffersBlocker.BlockingType.Touches) == IngameOffersBlocker.BlockingType.Touches)
            {
                offersTouchesBlockers++;
            }

            if ((blockType & IngameOffersBlocker.BlockingType.Update) == IngameOffersBlocker.BlockingType.Update)
            {
                offersUpdateBlockers++;
            }
        }


        public void Unblock(IngameOffersBlocker.BlockingType blockType)
        {
            if ((blockType & IngameOffersBlocker.BlockingType.Touches) == IngameOffersBlocker.BlockingType.Touches)
            {
                offersTouchesBlockers = Mathf.Max(0, offersTouchesBlockers - 1);
            }

            if ((blockType & IngameOffersBlocker.BlockingType.Update) == IngameOffersBlocker.BlockingType.Update)
            {
                offersUpdateBlockers = Mathf.Max(0, offersUpdateBlockers - 1);
            }
        }

        #endregion
    }
}
