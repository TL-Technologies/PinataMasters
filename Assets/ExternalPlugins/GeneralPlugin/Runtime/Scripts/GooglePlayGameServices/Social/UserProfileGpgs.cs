using Modules.General.Abstraction.GooglePlayGameServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;


namespace Modules.General.GooglePlayGameServices
{
    public class UserProfileGpgs : IUserProfileGpgs
    {
        #region IUserProfileGpgs

        public string userName { get; protected set; }


        public string id { get; protected set; }


        public bool isFriend => false;


        public UserState state => UserState.Online;


        public Texture2D image => null;

        #endregion



        #region Class lifecycle

        public UserProfileGpgs() { }
        
        
        public UserProfileGpgs(NativeUserProfile nativeUserProfile)
        {
            userName = nativeUserProfile.displayName;
            id = nativeUserProfile.id;
        }

        #endregion
    }
}
