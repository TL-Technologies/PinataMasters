namespace Http
{
    public interface IHttpDownloadFileResponse : IHttpResponse
    {
        string ETag { get; }


        string OutputPath { get; }
    }
}