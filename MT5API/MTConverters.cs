using System;
using MTAPIService;
using System.Collections;

namespace MT5API
{
    internal static class MTConverters
    {
        private static readonly MTLog Log = LogConfigurator.GetLogger(typeof(MTConverters));

        #region Values Converters

        public static bool ParseResult(this string inputString, char separator, out double result)
        {
            Log.Debug($"ParseResult: inputString = {inputString}, separator = {separator}");

            var retVal = false;
            result = 0;

            if (string.IsNullOrEmpty(inputString) == false)
            {
                string[] values = inputString.Split(separator);
                if (values.Length == 2)
                {
                    try
                    {
                        retVal = int.Parse(values[0]) != 0;

                        result = double.Parse(values[1]);
                    }
                    catch (Exception ex)
                    {
                        retVal = false;
                        Log.Error($"ParseResult: {ex.Message}");
                    }
                }
            }
            else
            {
                Log.Warn("ParseResult: input srting is null or empty!");
            }

            return retVal;
        }

        public static bool ParseResult(this string inputString, char separator, out DateTime from, out DateTime to)
        {
            Log.Debug($"ParseResult: inputString = {inputString}, separator = {separator}");

            var retVal = false;

            from = new DateTime();
            to = new DateTime();

            if (string.IsNullOrEmpty(inputString) == false)
            {
                var values = inputString.Split(separator);
                if (values.Length == 3)
                {
                    try
                    {
                        retVal = int.Parse(values[0]) != 0;

                        var iFrom = int.Parse(values[1]);
                        from = MT5TimeConverter.ConvertFromMTTime(iFrom);

                        var iTo= int.Parse(values[2]);
                        to = MT5TimeConverter.ConvertFromMTTime(iTo);
                    }
                    catch (Exception ex)
                    {
                        retVal = false;
                        Log.Error($"ParseResult: {ex.Message}");
                    }
                }
            }
            else
            {
                Log.Warn("ParseResult: input srting is null or empty!");
            }

            return retVal;
        }

        public static ArrayList ToArrayList(this MQLTradeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException();

            var exp = MT5TimeConverter.ConvertToMTTime(request.Expiration);

            return new ArrayList { (int)request.Action, request.Magic, request.Order, request.Symbol, request.Volume
                , request.Price, request.StopLimit, request.SL, request.TP, request.Deviation, (int)request.Type
                , (int)request.TypeFilling, (int)request.TypeTime, exp, request.Comment, request.Position, request.PositionBy };
        }

        #endregion
    }
}
