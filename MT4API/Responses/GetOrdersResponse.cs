using System.Collections.Generic;

namespace MT4API.Responses
{
    internal class GetOrdersResponse: ResponseBase
    {
        public List<MT4Order> Orders { get; set; } 
    }
}