using System;

namespace MT4API
{
    public class MT4ConnectionEventArgs: EventArgs
    {
        public MT4ConnectionState Status { get; private set; }
        public string ConnectionMessage { get; private set; }

        public MT4ConnectionEventArgs(MT4ConnectionState status, string message)
        {
            Status = status;
            ConnectionMessage = message;
        }
    }
}
