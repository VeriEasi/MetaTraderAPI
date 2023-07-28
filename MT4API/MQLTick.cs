using System;

namespace MT4API
{
    public class MQLTick
    {
        public int MTTime { get; set; }
        public DateTime Time => MT4APITimeConverter.ConvertFromMTTime(MTTime);
        public double Bid { get; set; }
        public double Ask { get; set; }
        public double Last { get; set; }
        public ulong Volume { get; set; }
    }
}