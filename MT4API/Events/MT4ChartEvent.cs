namespace MT4API.Events
{
    internal class MT4ChartEvent
    {
        public long ChartID { get; set; }
        public int EventID { get; set; }
        public long LParam { get; set; }
        public double DParam { get; set; }
        public string SParam { get; set; }
    }
}