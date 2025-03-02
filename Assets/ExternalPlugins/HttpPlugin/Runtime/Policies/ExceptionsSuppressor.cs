using System;
using System.Threading;
using System.Threading.Tasks;


namespace CrossPromo.Http
{
    //public abstract class ExceptionsSuppressor<T> : IHttpPolicy<T> where T : IHttpResponse
    //{
    //    private readonly IHttpOperation<T> operation;


    //    public bool SuppressCancellation { get; set; } = false;


    //    public bool SuppressExceptions { get; set; } = true;


    //    public ExceptionsSuppressor(IHttpOperation<T> operation)
    //    {
    //        this.operation = operation;
    //    }


    //    public async Task<T> SendAsync(CancellationToken cancellationToken)
    //    {
    //        T response = default;

    //        try
    //        {
    //            response = await operation.SendAsync(cancellationToken);
    //        }
    //        catch (TaskCanceledException e)
    //        {
    //            if (!SuppressCancellation)
    //                throw;

    //            response = default; // new CancelledResponse()
    //        }
    //        catch (Exception e)
    //        {
    //            if (!SuppressExceptions)
    //                throw;

    //            response = default; // new FailedResponse()
    //        }

    //        return response;
    //    }
    //}
}
