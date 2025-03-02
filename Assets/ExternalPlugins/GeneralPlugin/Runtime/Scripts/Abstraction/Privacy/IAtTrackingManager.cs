using System;


namespace Modules.General.Abstraction
{
    public interface IAtTrackingManager
    {
        UserAuthorizationStatus AuthorizationStatus { get; }

        void Initialize(Action onCompleteCallback);
    }
}
