using System;

namespace MT4API
{
    public class MT4ExecutionException: Exception
    {
        public MT4ExecutionException(MT4ErrorCode errorCode, string message)
            :base(message)
        {
            ErrorCode = errorCode;
        }

        public MT4ErrorCode ErrorCode { get; private set; }
    }
}