using System.Threading;
using System.Threading.Tasks;


namespace Http
{
    public sealed class InternetReachabilityWaiter<T> : IHttpPolicy<T> where T : IHttpResponse
    {
        private readonly IHttpOperation<T> operation;


        public InternetReachabilityWaiter(IHttpOperation<T> operation)
        {
            this.operation = operation;
        }


        public async Task<T> SendAsync(CancellationToken cancellationToken)
        {
            //await operation.HttpClient.WaitForInternetReachableAsync(cancellationToken);
            await TaskExtensions.WaitForInternetReachableAsync(cancellationToken);
            return await operation.SendAsync(cancellationToken);
        }
    }
}
