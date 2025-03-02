using System.Collections.Generic;


namespace Http
{
    public interface IHttpClient
    {
        IDictionary<string, string> Headers { get; }


        IHttpGetRequest Get(string url);

        IHttpPostRequest Post(string url, string postData);

        IHttpDownloadFileRequest DownloadFile(string url, string outputPath);
    }
}
