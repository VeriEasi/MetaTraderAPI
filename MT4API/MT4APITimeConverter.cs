using System;

namespace MT4API
{
    class MT4APITimeConverter
    {
        public static DateTime ConvertFromMTTime(int time)
        {
            DateTime tmpTime = new DateTime(1970, 1, 1);
            return new DateTime(tmpTime.Ticks + (time * 0x989680L));
        }

        public static int ConvertToMTTime(DateTime? time)
        {
            int result = 0;
            if (time != null && time != DateTime.MinValue)
            {
                DateTime tmpTime = new DateTime(1970, 1, 1);
                result = (int)((time.Value.Ticks - tmpTime.Ticks) / 0x989680L);
            }
            return result;
        }
    }
}
