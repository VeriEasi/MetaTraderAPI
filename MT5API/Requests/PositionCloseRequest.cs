namespace MT5API.Requests
{
    internal class PositionCloseRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.PositionClose;

        public ulong Ticket { get; set; }
        public ulong Deviation { get; set; }
    }
}