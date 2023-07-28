using System;
using System.Collections.Generic;

namespace MT4API.Monitors
{
    public class AvailabilityOrdersEventArgs : EventArgs
    {
        public AvailabilityOrdersEventArgs(List<MT4Order> opened, List<MT4Order> closed)
        {
            Opened = opened;
            Closed = closed;
        }
        /// <summary>
        /// Contains all newly opened orders since the last time the monitor checked the open orders.
        /// </summary>
        public List<MT4Order> Opened { get; private set; }
        /// <summary>
        /// Contains all newly closed orders since the last time the monitor checked the open orders.
        /// </summary>
        public List<MT4Order> Closed { get; private set; }
    }
}
