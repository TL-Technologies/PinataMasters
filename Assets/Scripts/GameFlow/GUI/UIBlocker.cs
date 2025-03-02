using System;


namespace PinataMasters
{
    public class UIBlocker : UIUnit<UnitResult>
    {
        #region Fields

        public static readonly ResourceGameObject<UIBlocker> Prefab = new ResourceGameObject<UIBlocker>("Game/GUI/DialogUIBlocker");

        #endregion



        #region Public Methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            Showed();
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            Hided();
        }

        #endregion
    }
}