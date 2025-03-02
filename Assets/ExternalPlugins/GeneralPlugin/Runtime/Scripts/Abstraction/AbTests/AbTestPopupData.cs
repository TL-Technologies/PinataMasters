using Newtonsoft.Json;
using System;
#if UNITY_IOS
    using UnityEngine.iOS;
#endif


namespace Modules.General.Abstraction
{
    public class AbTestPopupData : IAbTestData
    {
        #region Fields

        private const string DefaultTargetIosVersion = "14.5";
        private const string DefaultCurrentIosVersion = "1.0";
        private Version targetIosVersion = new Version(DefaultTargetIosVersion);
        private Version currentIosVersion;

        #endregion



        #region Properties

        public string AttPopupTargetIosVersion
        {
            get => targetIosVersion.ToString();
            set => targetIosVersion = new Version(value);
        }


        [JsonIgnore] public bool IsShowAttPopup => CurrentIosVersion >= targetIosVersion;


        [JsonIgnore] public bool IsShowGdprPopup => CurrentIosVersion < targetIosVersion;


        private Version CurrentIosVersion
        {
            get
            {
                if (currentIosVersion == null)
                {
                    currentIosVersion =
                        #if UNITY_IOS && !UNITY_EDITOR
                            new Version(Device.systemVersion);
                        #else
                            new Version(DefaultCurrentIosVersion);
                        #endif
                }

                return currentIosVersion;
            }
        }

        #endregion
    }
}