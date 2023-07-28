using System;

namespace MT4API
{
    public class MQLRates
    {
        public int MTTime { get; set; }
        public DateTime Time => MT4APITimeConverter.ConvertFromMTTime(MTTime);
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long TickVolume { get; set; }
        public int Spread { get; set; }
        public long RealVolume { get; set; }
    }
}
