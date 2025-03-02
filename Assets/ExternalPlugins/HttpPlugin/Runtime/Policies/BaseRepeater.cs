using System.Threading;
using System.Threading.Tasks;


namespace Http
{
    public abstract class BaseRepeater<T> : IHttpPolicy<T> where T : IHttpResponse
    {
        private readonly IHttpOperation<T> operation;


        protected abstract int GetNextDelayTime();


        protected abstract void Reset();


        public BaseRepeater(IHttpOperation<T> operation)
        {
            this.operation = operation;
        }


        public async Task<T> SendAsync(CancellationToken cancellationToken)
        {
            Reset();

            while (true)
            {
                var response = await operation.SendAsync(cancellationToken);
                if (response.ResponseCode < 400)
                    return response;

                int delay = GetNextDelayTime();
                if (delay < 0)
                    return response;

                UnityEngine.Debug.Log(
                    $"[{nameof(BaseRepeater<T>)}] Retry to perform operation in {delay} milliseconds.");
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
