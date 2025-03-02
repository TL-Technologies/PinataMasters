using UnityEngine;


namespace Modules.AppsFlyer
{
    public class TestAppsFlyer : MonoBehaviour
    {
        #region GUI

        void OnGUI()
        {
            float startWidth = 0.1f;
            float startHeight = 0.05f;
            float stepHeight = 0.08f;
            float buttonWidth = 0.8f;
            float buttonHeight = 0.06f;

            GUI.skin.button.fontSize = (int) (0.035f * Screen.height);

            Rect inofRect = new Rect(startWidth * Screen.width, (startHeight + stepHeight * 2) * Screen.height,
                buttonWidth * Screen.width, buttonHeight * Screen.height);
            GUI.Button(inofRect, "APP ID = " + LLAppsFlyerSettings.AppID());

            Rect keyRect = new Rect(startWidth * Screen.width, (startHeight + stepHeight * 3) * Screen.height,
                buttonWidth * Screen.width, buttonHeight * Screen.height);
            GUI.Button(keyRect, "DEV KEY = " + LLAppsFlyerSettings.DevKey());

            Rect setUserConsentRect = new Rect(startWidth * Screen.width,
                (startHeight + stepHeight * 4) * Screen.height, buttonWidth * Screen.width,
                buttonHeight * Screen.height);
            if (GUI.Button(setUserConsentRect, "Set user consent - True "))
            {
                LLAppsFlyerManager.SetUserConsent(true);
            }
            
            Rect initializeBannerRect = new Rect(startWidth * Screen.width,
                (startHeight + stepHeight * 5) * Screen.height, buttonWidth * Screen.width,
                buttonHeight * Screen.height);
            if (GUI.Button(initializeBannerRect, "Initialize "))
            {
                LLAppsFlyerManager.Initialize();
            }


            Rect eventBannerRect = new Rect(startWidth * Screen.width, (startHeight + stepHeight * 7) * Screen.height,
                buttonWidth * Screen.width, buttonHeight * Screen.height);
            if (GUI.Button(eventBannerRect, "Send Event = TEST"))
            {
                LLAppsFlyerManager.LogEvent("TEST", "080");
            }

            Rect purchaseBannerRect = new Rect(startWidth * Screen.width,
                (startHeight + stepHeight * 8) * Screen.height, buttonWidth * Screen.width,
                buttonHeight * Screen.height);
            if (GUI.Button(purchaseBannerRect, "Send Purchase = 0.66$"))
            {
                var test = new System.Collections.Generic.Dictionary<string, string>();
                test.Add(LLAppsFlyerEvents.CONTENT_ID, "com.some.id");
                test.Add(LLAppsFlyerEvents.CURRENCY, "USD");
                test.Add(LLAppsFlyerEvents.REVENUE, "0.99");
                test.Add(LLAppsFlyerEvents.QUANTITY, "1");
                LLAppsFlyerManager.LogRichEvent(LLAppsFlyerEvents.PURCHASE, test);
            }
        }

        #endregion
    }
}
