using System;

namespace MT5API
{
    public class MT5ConnectionEventArgs: EventArgs
    {
        public MT5ConnectionState Status { get; }
        public string ConnectionMessage { get; }

        public MT5ConnectionEventArgs(MT5ConnectionState status, string message)
        {
            Status = status;
            ConnectionMessage = message;
        }
    }
}
