using System;
using System.Threading;
using System.Threading.Tasks;


namespace Http
{
    public class DownloadFileRequest
    {
        private readonly TaskCompletionSource<DownloadFileResponse> taskCompletionSource;


        public string Url { get; }


        public string Path { get; }


        public string ETag { get; }


        public CancellationToken CancellationToken { get; }


        public Task<DownloadFileResponse> Task => taskCompletionSource.Task;


        public DownloadFileRequest(string url, string path, string eTag, CancellationToken cancellationToken)
        {
            Url = url;
            Path = path;
            ETag = eTag;
            CancellationToken = cancellationToken;
            this.taskCompletionSource = new TaskCompletionSource<DownloadFileResponse>();

            if (cancellationToken != CancellationToken.None)
                cancellationToken.Register(TrySetCanceled);
        }


        internal void TrySetResult(DownloadFileResponse response)
        {
            taskCompletionSource.TrySetResult(response);
        }


        internal void TrySetCanceled()
        {
            taskCompletionSource.TrySetCanceled();
        }


        internal void TrySetException(Exception exception)
        {
            taskCompletionSource.TrySetException(exception);
        }
    }
}
