namespace MT5API.Events
{
    public class OnLastTimeBarEvent
    {
        public MQLRates Rates { get; set; }
        public string Instrument { get; set; }
        public int ExpertHandle { get; set; }
    }
}