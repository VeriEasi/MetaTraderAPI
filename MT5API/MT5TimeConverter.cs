using System;

namespace MT5API
{
    internal class MT5TimeConverter
    {
        public static DateTime ConvertFromMTTime(int time)
        {
            var tmpTime = new DateTime(1970, 1, 1);
            return new DateTime(tmpTime.Ticks + (time * 0x989680L));
        }

        public static DateTime ConvertFromMtTime(long time)
        {
            var tmpTime = new DateTime(1970, 1, 1);
            return new DateTime(tmpTime.Ticks + (time * 0x989680L));
        }

        public static int ConvertToMTTime(DateTime? time)
        {
            var result = 0;
            if (time == null || time == DateTime.MinValue)
                return result;
            var tmpTime = new DateTime(1970, 1, 1);
            result = (int)((time.Value.Ticks - tmpTime.Ticks) / 0x989680L);
            return result;
        }
    }
}
