namespace Http
{
    public static class InternetReachabilityWaiterExtensions
    {
        public static InternetReachabilityWaiter<T> AddInternetReachabilityWaiter<T>(this IHttpOperation<T> operation)
            where T : IHttpResponse
        {
            return new InternetReachabilityWaiter<T>(operation);
        }
    }
}
