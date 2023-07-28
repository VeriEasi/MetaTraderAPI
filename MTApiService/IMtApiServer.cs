using System.Collections.Generic;

namespace MTAPIService
{
    public interface IMTAPIServer
    {
        MTResponse SendCommand(MTCommand command);
        List<MTQuote> GetQuotes();
    }
}
