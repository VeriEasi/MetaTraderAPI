using System;

namespace MT4API
{
    public class MT4Session
    {
        public string Symbol { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public uint Index { get; set; }
        public int MTFromTime { get; set; }
        public DateTime From => MT4APITimeConverter.ConvertFromMTTime(MTFromTime);
        public int MTToTime { get; set; }
        public DateTime To => MT4APITimeConverter.ConvertFromMTTime(MTToTime);
        public bool HasData { get; set; }
        public SessionType Type { get; set; }
    }

    public enum SessionType
    {
        Quote,
        Trade
    }
}
