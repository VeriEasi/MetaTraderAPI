using System;

namespace MT4API
{
    public class MT4ConnectionException: Exception        
    {
        public MT4ConnectionException()
            : this(null, null)
        {            
        }

        public MT4ConnectionException(string message)
            : this(message, null)
        {            
        }

        public MT4ConnectionException(string message, Exception exception)
            : base(message, exception)
        {
        }

    }
}