using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;


namespace Http
{
    public partial class UnityHttpClient
    {
        private class HttpPostRequest : HttpRequest, IHttpPostRequest
        {
            public string PostData { get; set; }


            public HttpPostRequest(UnityHttpClient httpClient, string url, string postData) :
                base(httpClient, url)
            {
                PostData = postData;
            }


            public HttpPostRequest(UnityHttpClient httpClient, string url) :
                this(httpClient, url, "") { }


            public async Task<IHttpPostResponse> SendAsync(CancellationToken cancellationToken)
            {
                string url = BuildUrl();
                using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, PostData))
                {
                    var data = Encoding.UTF8.GetBytes(PostData);
                    request.uploadHandler = new UploadHandlerRaw(data);

                    request.uploadHandler.contentType = Headers.ContainsKey("Content-Type") ? 
                        Headers["Content-Type"] : 
                        "application/json";

                    int responseCode = await httpClient.SendWebRequestAsync(request, Headers, cancellationToken);
                    var responseHeaders = httpClient.GetResponseHeaders(request);

                    return new HttpPostResponse(responseCode, responseHeaders, request.downloadHandler?.text);
                }
            }
        }
    }
}
