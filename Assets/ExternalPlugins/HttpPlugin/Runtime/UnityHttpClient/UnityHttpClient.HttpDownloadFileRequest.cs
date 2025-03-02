using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Http
{
    public partial class UnityHttpClient
    {
        private class HttpDownloadFileRequest : HttpRequest, IHttpDownloadFileRequest
        {
            public string OutputPath { get; }


            public HttpDownloadFileRequest(UnityHttpClient httpClient, string url, string outputPath)
                : base(httpClient, url)
            {
                OutputPath = outputPath;
            }


            public async Task<IHttpDownloadFileResponse> SendAsync(CancellationToken cancellationToken)
            {
                string url = BuildUrl();
                using (UnityWebRequest request = new UnityWebRequest(url))
                {
                    request.downloadHandler = new DownloadHandlerFile(OutputPath);

                    try
                    {
                        int responseCode = await httpClient.SendWebRequestAsync(request, Headers, cancellationToken);
                        var responseHeaders = httpClient.GetResponseHeaders(request);

                        if (responseCode < 0)
                        {
                            File.Delete(OutputPath);
                            return new HttpDownloadFileResponse(responseCode, responseHeaders, OutputPath);
                        }

                        // remove zero-length temp file (unity creates it to use as a stream buffer)
                        if (responseCode == HttpResponseCode.NotModified || responseCode > HttpResponseCode.BadRequest)
                            File.Delete(OutputPath);

                        return new HttpDownloadFileResponse(responseCode, responseHeaders, OutputPath);
                    }
                    catch
                    {
                        File.Delete(OutputPath);
                        throw;
                    }
                }
            }
        }
    }
}
