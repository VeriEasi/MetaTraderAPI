namespace MT5API.Requests
{
    internal class Response<T>
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public T Value { get; set; }
    }
}