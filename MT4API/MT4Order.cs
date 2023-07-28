using System;

namespace MT4API
{
    public class MT4Order
    {
        public int Ticket { get; set; }
        public string Symbol { get; set; }
        public TradeOperation Operation { get; set; }
        public double OpenPrice { get; set; }
        public double ClosePrice { get; set; }
        public double Lots { get; set; }
        public int MTOpenTime { get; set; }
        public int MTCloseTime { get; set; }
        public double Profit { get; set; }
        public string Comment { get; set; }
        public double Commission { get; set; }
        public int MagicNumber { get; set; }
        public double Swap { get; set; }
        public int MTExpiration { get; set; }
        public double TakeProfit { get; set; }
        public double StopLoss { get; set; }

        public DateTime OpenTime
        {
            get { return MT4APITimeConverter.ConvertFromMTTime(MTOpenTime); }
        }

        public DateTime CloseTime
        {
            get { return MT4APITimeConverter.ConvertFromMTTime(MTCloseTime); }
        }

        public DateTime Expiration
        {
            get { return MT4APITimeConverter.ConvertFromMTTime(MTExpiration); }
        }
    }
}