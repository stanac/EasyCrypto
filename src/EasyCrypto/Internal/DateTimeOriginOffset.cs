namespace EasyCrypto.Internal;

internal static class DateTimeOriginOffset
{
    private static readonly DateTime _originTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long GetOffset() => GetOffsetUtc(DateTime.UtcNow);

    public static long GetOffset(DateTime value) => GetOffsetUtc(value.ToUniversalTime());

    private static long GetOffsetUtc(DateTime value)
    {
        TimeSpan diff = value.ToUniversalTime() - _originTime;
        return (long)diff.TotalMilliseconds;
    }
}