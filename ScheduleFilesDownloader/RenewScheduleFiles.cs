namespace ScheduleFilesDownloader;

public class ScheduleFilesRenewer
{
    public static async Task<bool> Renew()
    {
        if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\schedule_files"))
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\schedule_files");
        
        var linksToNewFiles = LinkCatcher
            .Catch().Result
            .ToArray();
        var newScheduleFilenames = linksToNewFiles
            .Select(file => file?
                .Split('/')
                .Last())
            .ToArray();
        var currentScheduleFilenames = Directory
            .GetFiles(Directory.GetCurrentDirectory() + @"\schedule_files")
            .Select(filename => filename
                .Split('\\')
                .Last())
            .ToArray();

        if (currentScheduleFilenames.SequenceEqual(newScheduleFilenames)) return false;

        var linksToFilesNeedToUpdate = linksToNewFiles.ExceptBy(currentScheduleFilenames, s => s?.Split().Last());

        foreach (var link in linksToFilesNeedToUpdate)
        {
            File.Delete(Directory.GetCurrentDirectory() + @"\schedule_files" + $@"\{link?.Split('/').Last()}");
            if (link != null) await FileDownloader.Download(link);
        }
        
        return true;
    }
}