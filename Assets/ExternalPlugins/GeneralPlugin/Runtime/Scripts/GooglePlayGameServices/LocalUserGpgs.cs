using Modules.General.Abstraction.GooglePlayGameServices;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;


namespace Modules.General.GooglePlayGameServices
{
    internal class LocalUserGpgs : UserProfileGpgs, ILocalUserGpgs
    {
        #region Fields

        private static readonly AndroidJavaClass LocalUserClass =
            new AndroidJavaClass("com.lllibset.LLSocialGPGS.LLLocalUserGPGS");

        private const string AuthenticateMethodName = "LLLocalUserGPGSAuthenticate";
        private const string IsAuthenticatedMethodName = "LLLocalUserGPGSIsAuthenticated";
        private const string IsUnderAgeMethodName = "LLLocalUserGPGSIsUnderage";
        private const string GetCurrentPlayerJsonMethodName = "LLLocalUserGPGSGetCurrentPlayerJSON";

        #endregion



        #region ILocalUserGpgs

        public bool authenticated => LocalUserClass.CallStatic<bool>(IsAuthenticatedMethodName);
        
        
        public IUserProfile[] friends => throw new NotImplementedException();
        
        
        public bool underage => LocalUserClass.CallStatic<bool>(IsUnderAgeMethodName);
        
        
        public void Authenticate(Action<bool> callback)
        {
            AndroidJavaProxy callbackAuthenticate = HandlerAuthentication(callback);
            LocalUserClass.CallStatic(AuthenticateMethodName, callbackAuthenticate);
            
            
            AndroidJavaProxy HandlerAuthentication(Action<bool> internalCallback)
            {
                AndroidJavaProxy handler = LLAndroidJavaCallback.ProxyCallback((bool success) => 
                {
                    if (success)
                    {
                        string currentPlayerJson = LocalUserClass.CallStatic<string>(GetCurrentPlayerJsonMethodName);

                        if (!string.IsNullOrEmpty(currentPlayerJson))
                        {
                            NativeUserProfile nativeUserProfile = JsonConvert.DeserializeObject<NativeUserProfile>(currentPlayerJson);
                            userName = nativeUserProfile.displayName;
                            id = nativeUserProfile.id;
                        }
                    }

                    internalCallback?.Invoke(success);
                });

                return handler;
            }
        }

        
        public void Authenticate(Action<bool, string> callback) => throw new NotImplementedException();

        
        public void LoadFriends(Action<bool> callback) => throw new NotImplementedException();
        
        #endregion
    }
}
