using System;

namespace MT5API
{
    public class MQLTick
    {
        public MQLTick(DateTime time, double bid, double ask, double last, ulong volume)
        {
            MTTime = MT5TimeConverter.ConvertToMTTime(time);
            Bid = bid;
            Ask = ask;
            Last = last;
            Volume = volume;
        }

        public MQLTick()
        {
        }

        public long MTTime { get; set; }          // Time of the last prices update
        public double Bid { get; set; }           // Current Bid price
        public double Ask { get; set; }           // Current Ask price
        public double Last { get; set; }          // Price of the last deal (Last)
        public ulong Volume { get; set; }         // Volume for the current Last price
        public double VolumeReal { get; set; }   // Volume for the current Last price with greater accuracy 

        public DateTime Time => MT5TimeConverter.ConvertFromMtTime(MTTime);
    }
}
