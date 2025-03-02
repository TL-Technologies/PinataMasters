using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Http
{
    public partial class UnityHttpClient
    {
        private class HttpGetRequest : HttpRequest, IHttpGetRequest
        {
            public HttpGetRequest(UnityHttpClient httpClient, string url)
                : base(httpClient, url)
            {
            }


            public async Task<IHttpGetResponse> SendAsync(CancellationToken cancellationToken)
            {
                string url = BuildUrl();
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    int responseCode = await httpClient.SendWebRequestAsync(request, Headers, cancellationToken);
                    var responseHeaders = httpClient.GetResponseHeaders(request);

                    return new HttpGetResponse(responseCode, responseHeaders, request.downloadHandler?.text);
                }
            }
        }
    }
}
