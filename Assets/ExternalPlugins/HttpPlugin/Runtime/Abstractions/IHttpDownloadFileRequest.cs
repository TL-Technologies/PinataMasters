namespace Http
{
    public interface IHttpDownloadFileRequest : IHttpRequest<IHttpDownloadFileResponse>
    {
        string OutputPath { get; }
    }
}
