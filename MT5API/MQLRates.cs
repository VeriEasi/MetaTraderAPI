using System;

namespace MT5API
{
    public class MQLRates
    {
        public MQLRates(DateTime time, double open, double high, double low, double close, long tickVolume, int spread, long realVolume)
        {
            MTTime = MT5TimeConverter.ConvertToMTTime(time);
            Open = open;
            High = high;
            Low = low;
            Close = close;
            TickVolume = tickVolume;
            Spread = spread;
            RealVolume = realVolume;
        }

        internal MQLRates(long time, double open, double high, double low, double close, long tickVolume, int spread, long realVolume)
        {
            MTTime = time;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            TickVolume = tickVolume;
            Spread = spread;
            RealVolume = realVolume;
        }

        public MQLRates()
        {
        }

        public DateTime Time => MT5TimeConverter.ConvertFromMtTime(MTTime); // Period start time              
        public long MTTime { get; set; }         // Period start time (original MT time)
        public double Open { get; set; }         // Open price
        public double High { get; set; }         // The highest price of the period
        public double Low { get; set; }          // The lowest price of the period
        public double Close { get; set; }        // Close price
        public long TickVolume { get; set; }     // Tick volume
        public int Spread { get; set; }          // Spread
        public long RealVolume { get; set; }     // Trade volume
    }
}
