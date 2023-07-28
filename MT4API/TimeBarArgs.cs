using System;

namespace MT4API
{
    public class TimeBarArgs: EventArgs
    {
        internal TimeBarArgs(int expertHandle, MT4TimeBar timeBar)
            : this(timeBar)
        {
            ExpertHandle = expertHandle;
        }

        public TimeBarArgs(MT4TimeBar timeBar)
        {
            TimeBar = timeBar;
        }

        public int ExpertHandle { get; }
        public MT4TimeBar TimeBar { get; }
    }
}
