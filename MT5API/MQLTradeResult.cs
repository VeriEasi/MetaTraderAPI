// ReSharper disable InconsistentNaming

namespace MT5API
{
    public class MQLTradeResult
    {
        public MQLTradeResult(uint retCode, ulong deal, ulong order, double volume, double price, double bid, double ask, string comment, uint requestID)
        {
            RetCode = retCode;
            Deal = deal;
            Order = order;
            Volume = volume;
            Price = price;
            Bid = bid;
            Ask = ask;
            Comment = comment;
            RequestID = requestID;
        }

        internal MQLTradeResult(MQLTradeResult o)
        {
            RetCode = o.RetCode;
            Deal = o.Deal;
            Order = o.Order;
            Volume = o.Volume;
            Price = o.Price;
            Bid = o.Bid;
            Ask = o.Ask;
            Comment = o.Comment;
            RequestID = o.RequestID;
        }

        public uint RetCode { get; }          // Operation return code
        public ulong Deal { get; }            // Deal ticket, if it is performed
        public ulong Order { get; }           // Order ticket, if it is placed
        public double Volume { get; }         // Deal volume, confirmed by broker
        public double Price { get; }          // Deal price, confirmed by broker
        public double Bid { get; }            // Current Bid price
        public double Ask { get; }            // Current Ask price
        public string Comment { get; }        // Broker comment to operation (by default it is filled by the operation description)
        public uint RequestID { get; }       // Request ID set by the terminal during the dispatch

        public override string ToString()
        {
            return $"Retcode={RetCode}; Comment={Comment}; Deal={Deal}; Order={Order}; Volume={Volume}; Price={Price}; Bid={Bid}; Ask={Ask}; RequestID={RequestID}";
        }
    }
}
