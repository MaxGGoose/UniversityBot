using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace ExcelFileDivider;

public static class FetchGroupName
{
    public static string Fetch(ExcelWorksheet excelWorksheet)
    {
        var regex = new Regex("[А-Яа-я]+[-]+[0-9]+[-]?[0-9]*[а-я]*");
        var cells = new List<string>(1);
        
        for (var row = 10; row < 16; row++)
        {
            for (var col = 1; col < 5; col++)
            {
                cells.Add(excelWorksheet.Cells[row, col].Text);
            }
        }

        var temp = string.Join(' ', cells).Replace(" - ", "-");

        Console.WriteLine(regex.Match(temp).Value);
        
        return regex.Match(temp).Value;
    }
}