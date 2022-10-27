namespace ScheduleFilesDownloader;

internal static class HttpClientInstance
{
    private static HttpClient? _instance;

    public static HttpClient GetInstance()
    { return _instance ??= new HttpClient(); }
}