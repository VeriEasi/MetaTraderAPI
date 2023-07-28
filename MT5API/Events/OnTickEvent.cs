namespace MT5API.Events
{
    internal class OnTickEvent
    {
        public MQLTick Tick { get; set; }
        public string Instrument { get; set; }
        public int ExpertHandle { get; set; }
    }
}