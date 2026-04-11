namespace Lagrange.Milky.Extension;

public static class DateTimeExtension
{
    public static long ToUnixTimeSeconds(this DateTime time) => new DateTimeOffset(time.ToUniversalTime(), TimeSpan.Zero).ToUnixTimeSeconds();

    public static long LocalTimeToUnixTimeSeconds(this DateTime time) => new DateTimeOffset(time).ToUnixTimeSeconds();
}