namespace Http
{
    public static class HttpDownloadFileRequestExtensions
    {
        public static IHttpDownloadFileRequest IfNonMatch(this IHttpDownloadFileRequest request, string eTag)
        {
            request.SetHeader("If-None-Match", eTag);
            return request;
        }
    }
}
