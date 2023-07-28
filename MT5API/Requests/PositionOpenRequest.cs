namespace MT5API.Requests
{
    internal class PositionOpenRequest: RequestBase
    {
        public override RequestType RequestType => RequestType.PositionOpen;

        public string Symbol { get; set; }
        public EnumOrderType OrderType { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public double SL { get; set; }
        public double TP { get; set; }
        public string Comment { get; set; }
    }
}