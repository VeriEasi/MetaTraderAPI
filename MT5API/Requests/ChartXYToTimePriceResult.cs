using System;

namespace MT5API.Requests
{
    internal class ChartXYToTimePriceResult
    {
        public bool RetVal { get; set; }
        public int SubWindow { get; set; }
        public DateTime? Time => MT5TimeConverter.ConvertFromMTTime(MTTime);
        public double Price { get; set; }

        public int MTTime { get; set; }
    }
}