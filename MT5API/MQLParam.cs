namespace MT5API
{
    public class MQLParam
    {
        public EnumDataType DataType { get; set; }
        public long? IntegerValue { get; set; }
        public double? DoubleValue { get; set; }
        public string StringValue { get; set; }
    }
}
