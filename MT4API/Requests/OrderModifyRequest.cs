namespace MT4API.Requests
{
    internal class OrderModifyRequest: RequestBase
    {
        public int Ticket { get; set; }
        public double Price { get; set; }
        public double StopLoss { get; set; }
        public double TakeProfit { get; set; }
        public int Expiration { get; set; }
        public int? ArrowColor { get; set; }

        public override RequestType RequestType => RequestType.OrderModify;
    }
}