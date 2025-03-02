using System.Threading;
using System.Threading.Tasks;


namespace Http
{
    public interface IHttpOperation<T> where T : IHttpResponse
    {
        Task<T> SendAsync(CancellationToken cancellationToken);
    }
}