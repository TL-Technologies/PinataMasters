using System.Threading;
using System.Threading.Tasks;

namespace Http
{
    public static class HttpOperationExtensions
    {
        public static Task<T> SendAsync<T>(this IHttpOperation<T> operation) where T : IHttpResponse
        {
            return operation.SendAsync(CancellationToken.None);
        }
    }
}
