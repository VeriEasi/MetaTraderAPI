namespace MT5API.Requests
{
    internal class ChartXYToTimePriceRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.ChartXYToTimePrice;

        public long ChartID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}