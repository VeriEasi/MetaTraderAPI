namespace MT5API.Requests
{
    internal class SymbolInfoStringRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.SymbolInfoString;

        public string SymbolName { get; set; }
        public EnumSymbolInfoString PropID { get; set; }
    }
}