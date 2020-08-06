using System;

namespace ErroHandling.Exceptions
{
    public class AsyncPlaygroundException : Exception
    {
        public AsyncPlaygroundException(string payload)
            : base($"Payload: {payload}")
        {
        }
    }
}