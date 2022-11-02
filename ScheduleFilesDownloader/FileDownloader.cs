using File = System.IO.File;

namespace ScheduleFilesDownloader;

internal static class FileDownloader
{
    public static async Task Download(string linkToFile)
    {
        var httpResult = await HttpClientInstance.GetInstance().GetAsync(linkToFile);

        await using var resultStream = await httpResult.Content.ReadAsStreamAsync();
        await using var fileStream = File.Create(Directory.GetCurrentDirectory() + $@"\{Environment.GetEnvironmentVariable("RAW_SCHEDULE_DIRECTORY")}\{linkToFile.Split("/").Last()}");
        
        await resultStream.CopyToAsync(fileStream);
    }
}