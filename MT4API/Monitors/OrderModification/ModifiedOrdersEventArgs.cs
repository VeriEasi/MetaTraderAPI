using System;
using System.Collections.Generic;

namespace MT4API.Monitors
{
    public class ModifiedOrdersEventArgs : EventArgs
    {
        /// <summary>
        /// Returns a list of all modified orders
        /// </summary>
        public List<MT4ModifiedOrder> ModifiedOrders { get; }
        public ModifiedOrdersEventArgs(List<MT4ModifiedOrder> modifiedOrders)
        {
            ModifiedOrders = modifiedOrders;
        }
    }
}
