using Modules.General.Abstraction.GooglePlayGameServices;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;


namespace Modules.General.GooglePlayGameServices
{
    internal class LocalUserGpgsDummy : ILocalUserGpgs
    {
        public string userName => string.Empty;


        public string id => string.Empty;


        public bool isFriend => false;


        public UserState state => UserState.Offline;


        public Texture2D image => null;


        public IUserProfile[] friends => null;


        public bool authenticated => false;


        public bool underage => false;
        
        
        public void Authenticate(Action<bool> callback) => callback?.Invoke(false);


        public void Authenticate(Action<bool, string> callback) => callback?.Invoke(false, string.Empty);


        public void LoadFriends(Action<bool> callback) => callback?.Invoke(false);


    }
}
