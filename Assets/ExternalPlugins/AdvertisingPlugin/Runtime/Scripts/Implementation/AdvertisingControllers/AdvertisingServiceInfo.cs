using Modules.General.Abstraction;


namespace Modules.Advertising
{
    internal class AdvertisingServiceInfo<T>
    {
        public readonly IAdvertisingService advertisingService;
        public readonly T controlledElement;
        public string unitId;

        public AdvertisingServiceInfo(IAdvertisingService service, T element, string adIdentifier)
        {
            advertisingService = service;
            controlledElement = element;
            unitId = adIdentifier;
        }
    }
}
