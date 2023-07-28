using System.Collections.Generic;

namespace MT4API.Responses
{
    internal class CopyRatesResponse: ResponseBase
    {
        public List<MQLRates> Rates { get; set; }
    }
}
