namespace Domain.Extensions;

public static class DateTimeExtension {
    public static string GetDaysSince(this DateTime date) {
        var days = (DateTime.Now - date).Days;
        return days switch {
            0 => "Today",
            1 => "Yesterday",
            _ => $"{days} days ago"
        };
    }
}