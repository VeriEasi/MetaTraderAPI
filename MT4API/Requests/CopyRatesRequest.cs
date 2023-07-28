namespace MT4API.Requests
{
    internal abstract class CopyRatesRequestBase : RequestBase
    {
        public enum CopyRatesTypeEnum
        {
            CopyRates01 = 1,
            CopyRates02 = 2,
            CopyRates03 = 3,
        }

        public override RequestType RequestType => RequestType.CopyRates;

        public abstract CopyRatesTypeEnum CopyRatesType { get; }

        public string SymbolName { get; set; }
        public EnumTimeframes Timeframe { get; set; }
    }

    internal class CopyRates1Request : CopyRatesRequestBase
    {
        public override CopyRatesTypeEnum CopyRatesType => CopyRatesTypeEnum.CopyRates01;

        public int StartPos { get; set; }
        public int Count { get; set; }
    }

    internal class CopyRates2Request : CopyRatesRequestBase
    {
        public override CopyRatesTypeEnum CopyRatesType => CopyRatesTypeEnum.CopyRates02;

        public int StartTime { get; set; }
        public int Count { get; set; }
    }

    internal class CopyRates3Request : CopyRatesRequestBase
    {
        public override CopyRatesTypeEnum CopyRatesType => CopyRatesTypeEnum.CopyRates03;

        public int StartTime { get; set; }
        public int StopTime { get; set; }
    }
}
