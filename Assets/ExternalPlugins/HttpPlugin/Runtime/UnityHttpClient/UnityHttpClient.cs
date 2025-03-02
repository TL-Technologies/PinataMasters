using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace Http
{
    public partial class UnityHttpClient : IHttpClient
    {
        #region Properties

        public string Endpoint { get; }


        public IDictionary<string, string> Headers { get; }

        #endregion



        #region Public methods

        public UnityHttpClient(string endpoint)
        {
            Endpoint = endpoint.TrimEnd('/');
            Headers = new Dictionary<string, string>();
        }


        public IHttpGetRequest Get(string url)
        {
            var completeUrl = GetCompleteUrl(url);
            var request = new HttpGetRequest(this, completeUrl.url);

            return request;
        }


        public IHttpPostRequest Post(string url, string postData)
        {
            var completeUrl = GetCompleteUrl(url);
            var request = new HttpPostRequest(this, completeUrl.url, postData);

            return request;
        }


        public IHttpDownloadFileRequest DownloadFile(string url, string outputPath)
        {
            var completeUrl = GetCompleteUrl(url);
            var request = new HttpDownloadFileRequest(this, completeUrl.url, outputPath);

            return request;
        }

        #endregion



        #region Private methods

        private (string url, bool isRelative) GetCompleteUrl(string url)
        {
            if (url[0] == '/')
            {
                return (Endpoint + url, true);
            }

            return (url, false);
        }


        private async Task<int> SendWebRequestAsync(
            UnityWebRequest request,
            IDictionary<string, string> headers,
            CancellationToken cancellationToken)
        {
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            var requestAsyncOperation = request.SendWebRequest();
            try
            {
                while (!requestAsyncOperation.isDone)
                {
                    await Task.Delay(100, cancellationToken);
                }
            }
            catch
            {
                request.Abort();
                throw;
            }

            if (request.isNetworkError /*|| request.isHttpError*/)
            {
                Debug.LogError("Network exception");
            }

            return (int)request.responseCode;
        }


        private IReadOnlyDictionary<string, string> GetResponseHeaders(UnityWebRequest request)
        {
            var headers = request.GetResponseHeaders();

            if (headers == null)
            {
                return new Dictionary<string, string>();
            }

            // need to remove double quotes
            if (headers.TryGetValue("ETag", out string eTag))
            {
                headers["ETag"] = eTag.Trim('\"');
            }

            return headers;
        }

        #endregion
    }
}