public enum SqsPendingStatus
{
    PendingTrue = 'T',
    PendingFalse = 'F'
}

public static class SqsPendingStatusExtensions
{
    public static string ToCharString(this SqsPendingStatus status)
    {
        return ((char)status).ToString();
    }
}