using Modules.General.InitializationQueue;


namespace Modules.General.Abstraction
{
    public interface IHuaweiServices : IInitializableService
    {
        string GetAdvertisingIdentifier();
    }
}