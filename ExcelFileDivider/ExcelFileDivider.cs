using OfficeOpenXml;

namespace ExcelFileDivider;

public static class ExcelFileDivider
{
    public static void Divide(IEnumerable<string> excelFilenames)
    {
        var rawScheduleDirectoryPath = Environment.GetEnvironmentVariable("RAW_SCHEDULE_DIRECTORY");
        var dividedScheduleDirectoryPath = Environment.GetEnvironmentVariable("DIVIDED_SCHEDULE_DIRECTORY");
        var currentDirectoryPath = Directory.GetCurrentDirectory();
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    
        foreach (var excelFilename in excelFilenames)
        {
            var path = $@"{currentDirectoryPath}\{rawScheduleDirectoryPath}\{excelFilename}";
            
            if (excelFilename.Split('.').Last() == "xls")
            {
                
                path = $@"\{rawScheduleDirectoryPath}\new{excelFilename.Replace("xls", "xlsx")}";
                ConverterXlsToXlsx.ConvertToXlsxFile(
                    currentDirectoryPath + $@"\{rawScheduleDirectoryPath}\{excelFilename}", 
                    currentDirectoryPath + path);
            }
            
            using var package = new ExcelPackage(new FileInfo(path));
            var worksheets = package.Workbook.Worksheets;

            foreach (var worksheet in worksheets)
            {
                Console.WriteLine(excelFilename);
                var groupName = FetchGroupName.Fetch(worksheet);
                if (groupName == string.Empty) continue;

                if (File.Exists(currentDirectoryPath + $@"\{dividedScheduleDirectoryPath}\{groupName}.xlsx"))
                    File.Delete(currentDirectoryPath + $@"\{dividedScheduleDirectoryPath}\{groupName}.xlsx");
                
                using var newPackage = new ExcelPackage(currentDirectoryPath + $@"\{dividedScheduleDirectoryPath}\{groupName}.xlsx");
                if (newPackage.Workbook.Worksheets.Count > 0) continue;

                newPackage.Workbook.Worksheets.Add(groupName, worksheet);
                newPackage.Save();
            }
        }
    }
}