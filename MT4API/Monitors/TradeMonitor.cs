using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MT4API.Monitors.Triggers;

namespace MT4API.Monitors
{
    public class TradeMonitor : MT4MonitorBase
    {
        #region Fields
        private List<MT4Order> _prevOrders;
        private readonly object _locker = new object();
        #endregion

        #region Events
        //
        // Summary:
        //     Occurs when orders are opened or closed.
        public event EventHandler<AvailabilityOrdersEventArgs> AvailabilityOrdersChanged;
        #endregion

        #region ctor
        /// <summary>
        /// Constructor for initializing an instance of <see cref="TradeMonitor"/>.
        /// </summary>
        /// <param name="apiClient">The <see cref="MT4APIClient"/> which will be used to communicate with MetaTrader.</param>
        /// <param name="monitorTrigger">The custom instance of <see cref="IMonitorTrigger"/> which will be used to trigger this instance of <see cref="TradeMonitor"/>.</param>
        public TradeMonitor(MT4APIClient apiClient, IMonitorTrigger monitorTrigger) : base(apiClient, monitorTrigger) { }
        #endregion

        protected override void OnMT4Connected()
        {
            InitialCheck();
            base.OnMT4Connected();
        }
        protected override void OnStart()
        {
            if (IsMT4Connected)
                InitialCheck();
            base.OnStart();
        }

        protected override void OnTriggerRaised()
        {
            if (IsMT4Connected)
                Check();
        }
        
        private void Check()
        {
            try
            {
                CheckOrders();
            }
            catch (MT4ConnectionException)
            {
                //TODO: write error to log
            }
            catch (MT4ExecutionException)
            {
                //TODO: write error to log
            }
        }
        private void InitialCheck()
        {
            lock (_locker)
                _prevOrders = null;

            Task.Factory.StartNew(Check);
        }
        private void CheckOrders()
        {
            var openedOrders = new List<MT4Order>();
            var closedOrders = new List<MT4Order>();
            List<MT4Order> prevOrders;

            // get current orders from MetaTrader
            var tradesOrders = APIClient.GetOrders(OrderSelectSource.MODE_TRADES);

            lock (_locker)
                prevOrders = _prevOrders;

            if (prevOrders != null) //skip checking on first load orders
            {
                //check open orders
                openedOrders = tradesOrders.Where(to => prevOrders.Find(a => a.Ticket == to.Ticket) == null).ToList();

                //check closed orders
                var closeOrdersTemp = prevOrders.Where(po => tradesOrders.Find(a => a.Ticket == po.Ticket) == null).ToList();

                if (closeOrdersTemp.Count > 0)
                {
                    //get closed orders from history with actual values
                    var historyOrders = APIClient.GetOrders(OrderSelectSource.MODE_HISTORY);
                    closedOrders = closeOrdersTemp.Where(cot => historyOrders.Find(a => a.Ticket == cot.Ticket) != null).ToList();
                }
            }

            lock (_locker)
                _prevOrders = tradesOrders;

            if (openedOrders.Count > 0 || closedOrders.Count > 0)
            {
                AvailabilityOrdersChanged?.Invoke(this, new AvailabilityOrdersEventArgs(openedOrders, closedOrders));
            }
        }
    }
}
