namespace MT4API.Requests
{
    internal class SymbolInfoDoubleRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.SymbolInfoDouble;

        public string SymbolName { get; set; }
        public int PropID { get; set; }
    }
}