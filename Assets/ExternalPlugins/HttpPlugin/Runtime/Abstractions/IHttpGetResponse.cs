namespace Http
{
    public interface IHttpGetResponse : IHttpResponse
    {
        string ETag { get; }


        string Body { get; }
    }
}