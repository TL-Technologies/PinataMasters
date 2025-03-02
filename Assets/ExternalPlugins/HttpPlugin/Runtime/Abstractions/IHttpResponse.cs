using System.Collections.Generic;


namespace Http
{
    public interface IHttpResponse
    {
        int ResponseCode { get; }


        IReadOnlyDictionary<string, string> Headers { get; }


        // isCanceled, isNetworkError
        // Exception
    }
}