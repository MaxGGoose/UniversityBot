namespace ScheduleFilesDownloader;

public class ScheduleFilesRenewer
{
    public static async Task<bool> Renew()
    {
        var currentDirectoryPath = Directory.GetCurrentDirectory();
        var rawScheduleDirectory = Environment.GetEnvironmentVariable("RAW_SCHEDULE_DIRECTORY");
        var dividedScheduleDirectory = Environment.GetEnvironmentVariable("DIVIDED_SCHEDULE_DIRECTORY");
        
        if (!Directory.Exists(currentDirectoryPath + $@"\{rawScheduleDirectory}"))
            Directory.CreateDirectory(currentDirectoryPath + $@"\{rawScheduleDirectory}");
        
        if (!Directory.Exists(currentDirectoryPath + $@"\{dividedScheduleDirectory}"))
            Directory.CreateDirectory(currentDirectoryPath + $@"\{dividedScheduleDirectory}");
        
        var linksToNewFiles = LinkCatcher
            .Catch().Result
            .ToArray();
        var newScheduleFilenames = linksToNewFiles
            .Select(file => file?
                .Split('/')
                .Last())
            .ToArray();
        var currentScheduleFilenames = Directory
            .GetFiles(currentDirectoryPath + $@"\{rawScheduleDirectory}")
            .Select(filename => filename
                .Split('\\')
                .Last())
            .ToArray();

        if (currentScheduleFilenames.Order().SequenceEqual(newScheduleFilenames.Order())) return false;

        var linksToFilesNeedToUpdate = linksToNewFiles.ExceptBy(currentScheduleFilenames, s => s?.Split('/').Last()).ToList();

        foreach (var link in linksToFilesNeedToUpdate)
        {
            File.Delete(currentDirectoryPath + $@"\{rawScheduleDirectory}" + $@"\{link?.Split('/').Last()}");
            if (link != null) await FileDownloader.Download(link);
        }
        
        ExcelFileDivider.ExcelFileDivider.Divide(linksToFilesNeedToUpdate.Select(link => link!.Split('/').Last()));
        
        return true;
    }
}