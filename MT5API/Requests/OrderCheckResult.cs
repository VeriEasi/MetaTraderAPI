namespace MT5API.Requests
{
    public class OrderCheckResult
    {
        public bool RetVal { get; set; }
        public MQLTradeCheckResult TradeCheckResult { get; set; }
    }
}