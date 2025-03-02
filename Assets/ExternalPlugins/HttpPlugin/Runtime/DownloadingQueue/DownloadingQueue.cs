using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Http
{
    public class DownloadingQueue
    {
        private readonly IHttpClient httpClient;
        private Queue<DownloadFileRequest> queue = new Queue<DownloadFileRequest>();
        private CancellationToken executionCancellationToken;


        public bool InProgress { get; private set; }


        public DownloadingQueue(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        public void Enqueue(DownloadFileRequest request)
        {
            queue.Enqueue(request);

            if (!InProgress)
                _ = ExecuteAsync(executionCancellationToken);
        }


        public void SetExecutionCancellationToken(CancellationToken cancellationToken)
        {
            executionCancellationToken = cancellationToken;
        }


        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            InProgress = true;

            while (queue.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                // process next request
                var request = queue.Dequeue();

                try
                {
                    var httpResponse = await httpClient.DownloadFile(request.Url, request.Path)
                        .IfNonMatch(request.ETag)
                        .AddUniformRepeater()
                        .SendAsync(cancellationToken);

                    DownloadFileResponse response;
                    switch (httpResponse.ResponseCode)
                    {
                    case HttpResponseCode.Ok:
                        response = new DownloadFileResponse(httpResponse.ResponseCode, httpResponse.OutputPath,
                                                            httpResponse.ETag);
                        break;

                    case HttpResponseCode.NotModified:
                        response = new DownloadFileResponse(httpResponse.ResponseCode, null, httpResponse.ETag);
                        break;

                    default:
                        response = new DownloadFileResponse(httpResponse.ResponseCode, null, null);
                        break;
                    }

                    request.TrySetResult(response);
                }
                catch (TaskCanceledException)
                {
                    request.TrySetCanceled();
                }
                catch (Exception e)
                {
                    request.TrySetException(e);
                }
            }

            // cancel all deferred requests
            while (queue.Count > 0)
            {
                queue.Dequeue().TrySetCanceled();
            }

            InProgress = false;
        }
    }
}
