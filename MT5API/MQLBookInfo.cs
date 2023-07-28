namespace MT5API
{
    public class MQLBookInfo
    {
        public MQLBookInfo(EnumBookType type, double price, long volume)
        {
            Type = type;
            Price = price;
            Volume = volume;
        }

        public MQLBookInfo()
        { }

        public EnumBookType Type { get; set; }    // Order type from ENUM_BOOK_TYPE enumeration
        public double Price { get; set; }           // Price
        public long Volume { get; set; }            // Volume
        public double VolumeReal { get; set; }   // Volume for the current Last price with greater accuracy 

        public override string ToString()
        {
            return $"type = {Type}; price = {Price}; volume = {Volume}";
        }
    }
}
