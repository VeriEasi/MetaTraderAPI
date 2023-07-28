using System;

namespace MT5API
{
    public class MT5BookEventArgs : EventArgs
    {
        public int ExpertHandle { get; set; }
        public string Symbol { get; set; }      //Symbol of OnBookEvent event.
    }
}