using System;

namespace MTAPIService
{
    public class MTCommandExecuteEventArgs: EventArgs
    {
        public MTCommand Command { get; private set; }
        public MTResponse Response { get; private set; }

        public MTCommandExecuteEventArgs(MTCommand command, MTResponse response)
        {
            Command = command;
            Response = response;
        }
    }
}
