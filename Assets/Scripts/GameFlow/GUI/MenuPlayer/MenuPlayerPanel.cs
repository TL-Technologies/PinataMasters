using UnityEngine;


namespace PinataMasters
{
    public class MenuPlayerPanel : MonoBehaviour
    {

        public virtual bool NeedShowAlert()
        {
            return false;
        }


        public virtual void SetTutorialState() { }
    }
}
