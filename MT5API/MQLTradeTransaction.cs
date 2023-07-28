using System;
using Newtonsoft.Json;

namespace MT5API
{
    public class MQLTradeTransaction
    {
        public ulong Deal { get; set; }                         // Deal ticket 
        public ulong Order { get; set; }                        // Order ticket 
        public string Symbol { get; set; }                      // Trade symbol name
        public EnumTradeTransactionType Type { get; set; }      // Trade transaction type
        public EnumOrderType OrderType { get; set; }            // Order type
        public EnumOrderState OrderState { get; set; }          // Order state
        public EnumDealType DealType { get; set; }              // Deal type
        public EnumOrderTypeTime TimeType { get; set; }         // Order type by action period

        [JsonIgnore]
        public DateTime TimeExpiration                          // Order expiration time 
        {
            get { return MT5TimeConverter.ConvertFromMTTime(MTTimeExpiration); }
            set { MTTimeExpiration = MT5TimeConverter.ConvertToMTTime(value); }
        }

        public double Price { get; set; }                       // Price  
        public double PriceTrigger { get; set; }                // Stop limit order activation price 
        public double PriceSL { get; set; }                     // Stop Loss level 
        public double PriceTP { get; set; }                     // Take Profit level 
        public double Volume { get; set; }                      // Volume in lots 
        public ulong Position { get; set; }                     // Position ticket 
        public ulong PositionBy { get; set; }                   // Ticket of an opposite position
        public int MTTimeExpiration { get; private set; }

        public override string ToString()
        {
            return $"Deal={Deal}; Order={Order}; Symbol={Symbol}; Type={Type}; OrderType={OrderType}; OrderState={OrderState}; DealType={DealType}; TimeType={TimeType}; TimeExpiration={TimeExpiration}; Price={Price}; PriceTrigger{PriceTrigger}; PriceSL={PriceSL}; PriceTP={PriceTP}; Volume={Volume}; Position={Position}; PositionBy={PositionBy}";
        }
    }
}