using UnityEngine;
using Sirenix.OdinInspector;


namespace PinataMasters
{
    public class InteractionReporter : SerializedMonoBehaviour
    {
        #region Fields

        [SerializeField] IInteractionHandler interactionHandler;

        #endregion



        #region Private methods

        public void OnMouseDown()
        {
            interactionHandler.OnMouseDown();
        }

        #endregion
    }
}
