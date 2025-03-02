namespace Http
{
    public interface IHttpPostRequest : IHttpRequest<IHttpPostResponse>
    {
        string PostData { get; set; }
    }
}