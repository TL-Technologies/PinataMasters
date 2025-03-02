namespace Http
{
    public interface IHttpPostResponse : IHttpResponse
    {
        string ETag { get; }


        string Body { get; }
    }
}