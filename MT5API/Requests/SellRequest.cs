namespace MT5API.Requests
{
    internal class SellRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.Sell;

        public double Volume { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double SL { get; set; }
        public double TP { get; set; }
        public string Comment { get; set; }
    }
}
