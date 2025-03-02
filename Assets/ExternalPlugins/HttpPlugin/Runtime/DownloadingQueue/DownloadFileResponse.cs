namespace Http
{
    public class DownloadFileResponse
    {
        public int ResponseCode { get; }


        public string Path { get; }


        public string ETag { get; }


        public DownloadFileResponse(int responseCode, string path, string eTag)
        {
            ResponseCode = responseCode;
            Path = path;
            ETag = eTag;
        }
    }
}
