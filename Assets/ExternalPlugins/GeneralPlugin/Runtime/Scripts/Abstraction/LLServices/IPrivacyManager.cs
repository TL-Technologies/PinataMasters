using System;


namespace Modules.General.Abstraction
{
    public interface IPrivacyManager
    {
        bool IsPrivacyButtonAvailable { get; }
        bool WasPersonalDataDeleted { get; set; }
        void GetTermsAndPolicyURI(Action<bool, string> callback);
    }
}
