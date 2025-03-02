namespace Modules.Advertising
{
    public class AdAvailabilityErrorType
    {
        public const string NoAdAvailable = "NoAdAvailable"; //service (e.g. Mopub) answered with "no ad available"
        public const string AdIsShowing = "AdIsShowing";
        public const string NoAdServiceAvailable = "NoAdServiceAvailable";
        public const string NoInternet = "NoInternet";
    }
}