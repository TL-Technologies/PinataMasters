using Modules.General.Abstraction;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.Advertising
{
    [Serializable]
    public abstract class AdModuleSettings
    {
        #region Nested types

        [Serializable]
        public class AdCloseStatusHotKey
        {
            public KeyCode hotkey;
            public AdActionResultType closeResultType;
        }

        #endregion



        # region Fields

        [SerializeField] private int firstLoadDelay = 1;
        [SerializeField] private int reloadDelay = 2;

        [SerializeField] private List<AdCloseStatusHotKey> hotKeys =
            new List<AdCloseStatusHotKey>
            {
                new AdCloseStatusHotKey
                {
                    hotkey = KeyCode.Escape,
                    closeResultType = AdActionResultType.Success
                }
            };

        #endregion



        #region Properties

        public abstract AdModule ModuleType { get; }


        public int FirstLoadDelay
        {
            get => firstLoadDelay;
            set => firstLoadDelay = value;
        }


        public int ReloadDelay
        {
            get => reloadDelay;
            set => reloadDelay = value;
        }


        public List<AdCloseStatusHotKey> HotKeys
        {
            get => hotKeys;
            set => hotKeys = value;
        }

        #endregion
    }
}