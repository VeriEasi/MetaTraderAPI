using System;

namespace MT4API
{
    public class MT4TimeBar
    {
        public string Symbol { get; set; }
        public int MTOpenTime { get; set; }
        public int MTCloseTime { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public DateTime OpenTime => MT4APITimeConverter.ConvertFromMTTime(MTOpenTime);
        public DateTime CloseTime => MT4APITimeConverter.ConvertFromMTTime(MTCloseTime);
    }
}
