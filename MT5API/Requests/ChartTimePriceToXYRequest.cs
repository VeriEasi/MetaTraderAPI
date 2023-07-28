using System;

namespace MT5API.Requests
{
    internal class ChartTimePriceToXYRequest : RequestBase
    {
        public override RequestType RequestType => RequestType.ChartTimePriceToXY;

        public long ChartID { get; set; } 
        public int SubWindow { get; set; }
        public DateTime? Time { get; set; }
        public double Price { get; set; }

        public int MTTime => MT5TimeConverter.ConvertToMTTime(Time);
    }
}