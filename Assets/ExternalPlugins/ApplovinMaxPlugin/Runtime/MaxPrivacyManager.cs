using Modules.General;
using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.HelperClasses;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Modules.Max
{
    [InitQueueService(-100, typeof(IPrivacyManager))]
    public class MaxPrivacyManager : MonoBehaviour, IPrivacyManager, IInitializableService
    {
        #region Fields

        private const string SetUserConsentMethodName = "SetUserConsent";
        private const string WasPrivacyPopupsShownKey = "was_max_privacy_shown";

        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject eventSystem;
        private Action gdprCallback;
        
        #endregion



        #region Properties

        public bool WasPrivacyPopupsShown
        {
            get => CustomPlayerPrefs.GetBool(WasPrivacyPopupsShownKey, false);
            set => CustomPlayerPrefs.SetBool(WasPrivacyPopupsShownKey, value);
        }

        public bool IsPrivacyButtonAvailable => true;
        
        public bool WasPersonalDataDeleted { get; set; }
        
        public void GetTermsAndPolicyURI(Action<bool, string> callback)
        {
            callback(true, LLMaxSettings.Instance.TermsUrl);
        }

        #endregion



        #region Methods

        public void InitializeService(IServiceContainer container, Action onCompleteCallback, Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            LLMaxManager.Initialize(this, (isSuccess) =>
            {
                if (isSuccess)
                {
                    SetUserConsent(MaxSdk.HasUserConsent() || !MaxSdk.IsUserConsentSet());
                    
                    // Azure requirement
                    Scheduler.Instance.CallMethodWithDelay(Scheduler.Instance, () =>
                    {
                        onCompleteCallback?.Invoke();
                    }, 2.0f);
                }
                else
                {
                    onErrorCallback?.Invoke(this, InitializationStatus.Failed);
                }
            });
        }


        public void ShowGdpr(Action callback)
        {
            gdprCallback = callback;
            var eventSystemObject = FindObjectOfType<EventSystem>();
            canvas.SetActive(true);
            eventSystem.SetActive(eventSystemObject == null || !eventSystemObject.gameObject.activeInHierarchy);
        }


        public void ApplyGdpr()
        {
            WasPrivacyPopupsShown = true;
            canvas.SetActive(false);
            eventSystem.SetActive(false);
            gdprCallback?.Invoke();
        }


        public void OpenPrivacy()
        {
            Application.OpenURL(LLMaxSettings.Instance.PrivacyUrl);
        }


        public void OpenTerms()
        {
            Application.OpenURL(LLMaxSettings.Instance.TermsUrl);
        }
        
        
        private void SetUserConsent(bool isConsentAvailable)
        {
            object[] parameters = {isConsentAvailable};

            foreach (string className in LLMaxSettings.Instance.ConsentApiClassesNamesIncludingAssemblies)
            {
                Type classType = Type.GetType(className);
            
                if (classType != null)
                {
                    MethodInfo currentMethod = classType.GetMethod(SetUserConsentMethodName);
            
                    if (currentMethod != null)
                    {
                        currentMethod.Invoke(classType, parameters);
                    }
                }
            }
        }
        
        #endregion
    }
}
