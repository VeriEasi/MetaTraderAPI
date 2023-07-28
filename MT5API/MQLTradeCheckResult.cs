// ReSharper disable InconsistentNaming

namespace MT5API
{
    public class MQLTradeCheckResult
    {
        public uint RetCode { get; set; }               // Reply code
        public double Balance { get; set; }             // Balance after the execution of the deal
        public double Equity { get; set; }              // Equity after the execution of the deal
        public double Profit { get; set; }              // Floating profit
        public double Margin { get; set; }              // Margin requirements
        public double MarginFree { get; set; }         // Free margin
        public double MarginLevel { get; set; }        // Margin level
        public string Comment { get; set; }             // Comment to the reply code (description of the error)

        public MQLTradeCheckResult(uint retCode
            , double balance
            , double equity
            , double profit
            , double margin
            , double marginFree
            , double marginLevel
            , string comment)
        {
            RetCode = retCode;
            Balance = balance;
            Equity = equity;
            Profit = profit;
            Margin = margin;
            MarginFree = marginFree;
            MarginLevel = marginLevel;
            Comment = comment;
        }

        public MQLTradeCheckResult()
        { }

        public override string ToString()
        {
            return $"Retcode={RetCode}; Comment={Comment}; Balance={Balance}; Equity={Equity}; Profit={Profit}; Margin={Margin}; MarginFree={MarginFree}; MarginLevel={MarginLevel}";
        }
    }
}
