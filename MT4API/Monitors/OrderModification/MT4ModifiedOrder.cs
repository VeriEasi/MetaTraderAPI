using System;

namespace MT4API.Monitors
{
    public class MT4ModifiedOrder
    {
        /// <summary>
        /// The order in its old state (before the changes)
        /// </summary>
        public MT4Order OldOrder { get; }
        /// <summary>
        /// The order in its new state (after the changes)
        /// </summary>
        public MT4Order NewOrder { get; }
        /// <summary>
        /// The changes found by this instance
        /// </summary>
        public OrderModifiedTypes ModifyType { get; private set; }
        /// <summary>
        /// Initializes an instance and compare the order in its old and new state
        /// </summary>
        /// <param name="oldOrder">The order in its old state (before the changes)</param>
        /// <param name="newOrder">The order in its new state (after the changes)</param>
        public MT4ModifiedOrder(MT4Order oldOrder, MT4Order newOrder)
        {
            if (oldOrder != null && newOrder != null && oldOrder.Ticket != newOrder.Ticket)
                throw new ArgumentException(nameof(oldOrder) + " and " + nameof(newOrder) + " need to have the same ticket id");
            OldOrder = oldOrder;
            NewOrder = newOrder;
            ModifyType = OrderModifiedTypes.None;
            Compare();
        }
        private void Compare()
        {
            if(NewOrder != null && OldOrder != null)
            {
                if (OldOrder.StopLoss != NewOrder.StopLoss)
                    ModifyType |= OrderModifiedTypes.StopLoss;
                if (OldOrder.TakeProfit != NewOrder.TakeProfit)
                    ModifyType |= OrderModifiedTypes.TakeProfit;
                if (OldOrder.Operation != NewOrder.Operation)
                    ModifyType |= OrderModifiedTypes.Operation;
            }
        }
    }
}
