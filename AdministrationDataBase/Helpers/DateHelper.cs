using System;

namespace AdministrationDataBase.Helpers
{
    public class DateHelper
    {
        public static DateTime GetLocalDate()
        {
            DateTime utcNow = DateTime.UtcNow;
            string timeZoneId = "Europe/Madrid";
            DateTime localDate;
            try
            {
                // Convert to the Madrid/Europe timezone date
                localDate = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));

            }
            catch (TimeZoneNotFoundException)
            {
                timeZoneId = "Romance Standard Time";
                localDate = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
            }

            return localDate;
        }

        public static bool IsInTime(DateTime dateTime)
        {
            return dateTime.AddDays(2) >= DateTime.Now;
        }
    }
}