using System;
using Newtonsoft.Json;

namespace MT5API
{
    public class MQLTradeRequest
    {
        public EnumTradeRequestActions Action { get; set; }              // Trade operation type
        public ulong Magic { get; set; }                                 // Expert Advisor ID (magic number)
        public ulong Order { get; set; }                                 // Order ticket
        public string Symbol { get; set; }                               // Trade symbol
        public double Volume { get; set; }                               // Requested volume for a deal in lots
        public double Price { get; set; }                                // Price
        public double StopLimit { get; set; }                            // StopLimit level of the order
        public double SL { get; set; }                                   // Stop Loss level of the order
        public double TP { get; set; }                                   // Take Profit level of the order
        public ulong Deviation { get; set; }                             // Maximal possible deviation from the requested price
        public EnumOrderType Type { get; set; }                          // Order type
        public EnumOrderTypeFilling TypeFilling { get; set; }            // Order execution type
        public EnumOrderTypeTime TypeTime { get; set; }                  // Order expiration type

        [JsonIgnore]
        public DateTime Expiration                                       // Order expiration time (for the orders of ORDER_TIME_SPECIFIED type)
        {
            get { return MT5TimeConverter.ConvertFromMTTime(MTExpiration); }
            set { MTExpiration =  MT5TimeConverter.ConvertToMTTime(value); } 
        }

        public string Comment { get; set; }                              // Order comment
        public ulong Position { get; set; }                              // Position ticket
        public ulong PositionBy { get; set; }                            // The ticket of an opposite position
        public int MTExpiration { get; private set; }

        public override string ToString()
        {
            return $"Action={Action}; Magic={Magic}; Order={Order}; Symbol={Symbol}; Volume={Volume}; Price={Price}; StopLimit={StopLimit}; SL={SL}; TP={TP}; Deviation={Deviation}; Type={Type}; TypeFilling={TypeFilling}; TypeTime={TypeTime}; Expiration={Expiration}; Comment={Comment}; Position={Position}; PositionBy={PositionBy}";
        }
    }
}
