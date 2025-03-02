using System;


namespace PinataMasters
{
    public class UIGDPRBack : UIUnit<UnitResult>
    {
        public static readonly ResourceGameObject<UIGDPRBack> Prefab = new ResourceGameObject<UIGDPRBack>("Game/GUI/PanelGDPRBack");

        #region Public methods

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
