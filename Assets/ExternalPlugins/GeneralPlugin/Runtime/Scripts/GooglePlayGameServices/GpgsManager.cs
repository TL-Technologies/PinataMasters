using Modules.General.Abstraction.GooglePlayGameServices;
using Modules.Hive;


namespace Modules.General.GooglePlayGameServices
{
    public class GpgsManager
    {
        private static GpgsManager instance;
        private ISocialGpgs socialGpgs;
        private ILocalUserGpgs localUserGpgs;
        private ISavedGamesGpgs savedGamesGpgs;
        

        private static GpgsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GpgsManager();
                    
                    if (PlatformInfo.AndroidTarget == AndroidTarget.GooglePlay)
                    {
                        IGpgsSettings gpgsSettings = LLGPGSSettings.DoesInstanceExist ? LLGPGSSettings.Instance : null;
    
                        instance.localUserGpgs = new LocalUserGpgs();
                        instance.socialGpgs = new SocialGpgs(instance.localUserGpgs, gpgsSettings);
                        instance.savedGamesGpgs = new SavedGamesGpgs(instance.socialGpgs);
                    }
                    else
                    {
                        instance.localUserGpgs = new LocalUserGpgsDummy();
                        instance.socialGpgs = new SocialGpgsDummy();
                        instance.savedGamesGpgs = new SavedGamesGpgsDummy();
                    }
                }
                
                return instance;
            }
        }
        
        
        public static bool IsGpgsAvailable =>
            PlatformInfo.AndroidTarget == AndroidTarget.GooglePlay &&
            LLGPGSSettings.DoesInstanceExist &&
            LLGPGSSettings.Instance.IsGpgsEnabled;
        
        
        public static ISocialGpgs SocialGpgs => Instance.socialGpgs;
        public static ILocalUserGpgs LocalUserGpgs => Instance.localUserGpgs;
        public static ISavedGamesGpgs SavedGamesGpgs => Instance.savedGamesGpgs;
    }
}
