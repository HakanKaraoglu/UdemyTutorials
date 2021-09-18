using System;

namespace BandApi.Helpers
{
    public static class YearsAgo
    {
        public static int FoundedYearsAgo(this DateTime datetime)
        {
            return DateTime.Now.Year - datetime.Year;
        }
    }
}
