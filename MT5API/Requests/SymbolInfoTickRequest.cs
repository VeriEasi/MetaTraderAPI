namespace MT5API.Requests
{
    internal class SymbolInfoTickRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.SymbolInfoTick;

        public string SymbolName { get; set; }
    }
}
