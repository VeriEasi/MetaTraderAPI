using System;
using MT4API.Events;

namespace MT4API
{
    public class ChartEventArgs : EventArgs
    {
        internal ChartEventArgs(int expertHandle, MT4ChartEvent chartEvent)
        {
            ExpertHandle = expertHandle;
            ChartID = chartEvent.ChartID;
            EventID = chartEvent.EventID;
            LParam = chartEvent.LParam;
            DParam = chartEvent.DParam;
            SParam = chartEvent.SParam;
        }

        public int ExpertHandle { get; }
        public long ChartID { get; }
        public int EventID { get; }
        public long LParam { get; }
        public double DParam { get; }
        public string SParam { get; }
    }
}