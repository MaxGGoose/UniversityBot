using File = System.IO.File;

namespace ScheduleFilesDownloader;

public static class FileDownloader
{
    public static async Task Download(string linkToFile)
    {
        var httpResult = await HttpClientInstance.GetInstance().GetAsync(linkToFile);

        await using var resultStream = await httpResult.Content.ReadAsStreamAsync();
        
        if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\schedule_files"))
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\schedule_files");
        
        await using var fileStream = File.Create(Directory.GetCurrentDirectory() + $@"\{linkToFile.Split("/").Last()}");

        await resultStream.CopyToAsync(fileStream);
    }
}