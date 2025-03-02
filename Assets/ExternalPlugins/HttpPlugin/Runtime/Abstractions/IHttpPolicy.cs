namespace Http
{
    public interface IHttpPolicy<T> : IHttpOperation<T> where T : IHttpResponse
    {
        //Task<T> SendAsync(CancellationToken cancellationToken);
    }
}