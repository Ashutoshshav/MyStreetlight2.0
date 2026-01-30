using System.Globalization;
using System.Security.Claims;

namespace Streetlight2._0.Utilities
{
    public static class Helper
    {
        public static string FormatDate(DateTime? datetime)
        {
            return datetime.Value.ToString("dd MMM yyyy") ?? "N/A";
        }

        public static string FormatDateTime(DateTime? datetime)
        {
            return datetime.HasValue ? datetime.Value.ToString("dd MMM yyyy hh:mm tt") : "N/A";
        }

        public static string FormatTime(DateTime? datetime)
        {
            return datetime.HasValue ? datetime.Value.ToString("hh:mm tt") : "N/A";
        }

        public static DateTime GetIndianStandardDateTime()
        {
            // Get the current UTC time and convert it to Indian Standard Time (IST)
            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, istZone);
        }

        public static string FormatToIndianCurrency(decimal amount)
        {
            CultureInfo indianCulture = new CultureInfo("en-IN");
            string formatted = string.Format(indianCulture, "{0:N2}", amount); // Formats as 5,415.02
            return "₹ " + formatted;
        }

        public static decimal? TryParseDecimal(string input)
        {
            return decimal.TryParse(input, out var result) ? result : null;
        }

        public static string ConvertCoONMinTOCoONHour(decimal coONMin)
        {
            int totalMinutes = (int)coONMin;

            // Creating a TimeSpan from minutes
            TimeSpan time = TimeSpan.FromMinutes(totalMinutes);

            // Format as hh:mm
            string formatted = time.ToString(@"hh\:mm");

            Console.WriteLine(formatted); // Output: 01:30

            return formatted;
        }

        public static string GetCriticalColorByTime(DateTime faultTime)
        {
            var diff = DateTime.Now - faultTime;

            if (diff.TotalHours < 3) // Minimal Critical
            {
                // Neutral look, subtle hover only
                return "bg-rose-100 ";
            }
            else if (diff.TotalHours < 5) // Medium Critical
            {
                return "bg-rose-200";
            }
            else // More Critical
            {
                return "bg-rose-300";
            }
        }

        public static string ToTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return $"{timeSpan.Seconds} seconds ago";

            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} minutes ago";

            if (timeSpan.TotalHours < 24)
                return $"{timeSpan.Hours} hours ago";

            if (timeSpan.TotalDays < 30)
                return $"{timeSpan.Days} days ago";

            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} months ago";

            return $"{(int)(timeSpan.TotalDays / 365)} years ago";
        }

        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.HasClaim("Permission", permission);
        }
    }
}
