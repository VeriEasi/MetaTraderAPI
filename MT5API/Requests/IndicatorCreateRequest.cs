using System.Collections.Generic;

namespace MT5API.Requests
{
    internal class IndicatorCreateRequest: RequestBase
    {
        public override RequestType RequestType => RequestType.IndicatorCreate;

        public string Symbol { get; set; }
        public EnumTimeframes Period { get; set; }
        public EnumIndicator IndicatorType { get; set; }
        public List<MQLParam> Parameters { get; set; }
    }
}