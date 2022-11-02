using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace ExcelFileDivider;

public static class ConverterXlsToXlsx
{
    public static MemoryStream Convert(Stream sourceStream)
    {
// Открытие xls
        var source = new HSSFWorkbook(sourceStream);
// Создание объекта для будущего xlsx
        var destination = new XSSFWorkbook();
// Копируем листы из xls и добавляем в xlsx
        for (int i = 0; i < source.NumberOfSheets; i++)
        {
            var xssfSheet = (XSSFSheet)destination.CreateSheet(source.GetSheetAt(i).SheetName);
            var hssfSheet = (HSSFSheet)source.GetSheetAt(i);
            CopyStyles(hssfSheet, xssfSheet);
            CopySheet(hssfSheet, xssfSheet);
        }
// Возвращаем сконвертированный результат
        using (var ms = new MemoryStream())
        {
            destination.Write(ms);
            return ms;
        }
    }
    
    public static void ConvertToXlsxFile(string xlsPath, string destPath)
    {
        MemoryStream result;
        using (FileStream fs = new FileStream(xlsPath, FileMode.Open))
        {
            result = Convert(fs);
        }
        using (FileStream fs = new FileStream(destPath, FileMode.OpenOrCreate))
        {
            fs.Write(result.ToArray());
        }
    }

    private static CellRangeAddress GetMergedRegion(HSSFSheet sheet, int rowNum, short cellNum)
    {
        for (var i = 0; i < sheet.NumMergedRegions; i++)
        {
            var merged = sheet.GetMergedRegion(i);
            if (merged.IsInRange(rowNum, cellNum))
            {
                return merged;
            }
        }
        return null;
    }

    private static bool IsNewMergedRegion(CellRangeAddress newMergedRegion,List<CellRangeAddress> mergedRegions)
    {
        return !mergedRegions.Any(r =>
            r.FirstColumn == newMergedRegion.FirstColumn &&
            r.LastColumn == newMergedRegion.LastColumn &&
            r.FirstRow == newMergedRegion.FirstRow &&
            r.LastRow == newMergedRegion.LastRow);
    }

    private static void CopySheet(HSSFSheet source, XSSFSheet destination)
    {
        var maxColumnNum = 0;
        var mergedRegions = new List<CellRangeAddress>();
        for (int i = source.FirstRowNum; i <= source.LastRowNum; i++)
        {
            var srcRow = (HSSFRow)source.GetRow(i);
            var destRow = (XSSFRow)destination.CreateRow(i);
            if (srcRow != null)
            {
                CopyRow(source, destination, srcRow, destRow, mergedRegions);
                // поиск максимального номера ячейки в строке
                if (srcRow.LastCellNum > maxColumnNum)
                {
                    maxColumnNum = srcRow.LastCellNum;
                }
            }
        }
        // копируем ширину столбцов исходного документа
        for (int i = 0; i <= maxColumnNum; i++)
        {
            destination.SetColumnWidth(i, source.GetColumnWidth(i));
        }
    }

    private static void CopyRow(HSSFSheet srcSheet, XSSFSheet destSheet, HSSFRow srcRow, XSSFRow destRow, List<CellRangeAddress> mergedRegions)
    {
        // Копирование высоты строки
        destRow.Height = srcRow.Height;
        for (int j = srcRow.FirstCellNum; srcRow.LastCellNum >= 0 && j <= srcRow.LastCellNum; j++)
        {
            var oldCell = (HSSFCell)srcRow.GetCell(j);
            var newCell = (XSSFCell)destRow.GetCell(j);
            if (oldCell != null)
            {
                // создание новой ячейки в новой таблице
                newCell = (XSSFCell)destRow.CreateCell(j);
                
                CopyCell(oldCell, newCell);
                // Ниже идет обработка объединенных ячеек
                // Проверка на вхождение текущей ячейки в число объединенных
                var mergedRegion = GetMergedRegion(srcSheet, srcRow.RowNum,
                    (short)oldCell.ColumnIndex);
                // Если ячейка является объединенной
                if (mergedRegion != null)
                {
                    // Проверяем обрабатывали ли мы уже группу объединенных ячеек или нет
                    var newMergedRegion = new CellRangeAddress(mergedRegion.FirstRow,
                        mergedRegion.LastRow, mergedRegion.FirstColumn, mergedRegion.LastColumn);
                    // Если не обрабатывали, то добавляем в текущий диапазон объединенных ячеек текущую ячейку
                    if (IsNewMergedRegion(newMergedRegion, mergedRegions))
                    {
                        mergedRegions.Add(newMergedRegion);
                        destSheet.AddMergedRegion(newMergedRegion);
                    }
                }
            }
        }
    }

    private static void CopyCell(HSSFCell oldCell, XSSFCell newCell)
    {
        CopyCellStyle(oldCell, newCell);
        CopyCellValue(oldCell, newCell);
    }

    private static void CopyCellValue(HSSFCell oldCell, XSSFCell newCell)
    {
        switch (oldCell.CellType)
        {
            case CellType.String:
                newCell.SetCellValue(oldCell.StringCellValue);
                break;
            case CellType.Numeric:
                newCell.SetCellValue(oldCell.NumericCellValue);
                break;
            case CellType.Blank:
                newCell.SetCellType(CellType.Blank);
                break;
            case CellType.Boolean:
                newCell.SetCellValue(oldCell.BooleanCellValue);
                break;
            case CellType.Error:
                newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                break;
            case CellType.Formula:
                newCell.SetCellFormula(oldCell.CellFormula);
                break;
            default:
                break;
        }
    }

    private static void CopyCellStyle(HSSFCell oldCell, XSSFCell newCell)
    {
        if (oldCell.CellStyle == null)
            return;
        newCell.CellStyle = newCell.Sheet.Workbook.GetCellStyleAt((short)(oldCell.CellStyle.Index + 1));
    }

    private static void CopyStyles(HSSFSheet from, XSSFSheet to)
    {
        for (short i = 0; i <= from.Workbook.NumberOfFonts; i++)
        {
            CopyFont(to.Workbook.CreateFont(), from.Workbook.GetFontAt(i));
        }

        for (short i = 0; i < from.Workbook.NumCellStyles; i++)
        {
            CopyStyle(to.Workbook.CreateCellStyle(), from.Workbook.GetCellStyleAt(i), to.Workbook, from.Workbook);
        }
    }
    
    private static void CopyFont(IFont toFront, IFont fontFrom)
    {
        toFront.Charset = fontFrom.Charset;
        toFront.Color = fontFrom.Color;
        toFront.FontHeightInPoints = fontFrom.FontHeightInPoints;
        toFront.FontName = fontFrom.FontName;
        toFront.IsBold = fontFrom.IsBold;
        toFront.IsItalic = fontFrom.IsItalic;
        toFront.IsStrikeout = fontFrom.IsStrikeout;
    }
    
    private static void CopyStyle(ICellStyle toCellStyle, ICellStyle fromCellStyle, IWorkbook toWorkbook, IWorkbook fromWorkbook)
    {
        toCellStyle.Alignment = fromCellStyle.Alignment;
        toCellStyle.BorderBottom = fromCellStyle.BorderBottom;
        toCellStyle.BorderDiagonal = fromCellStyle.BorderDiagonal;
        toCellStyle.BorderDiagonalColor = fromCellStyle.BorderDiagonalColor;
        toCellStyle.BorderDiagonalLineStyle = fromCellStyle.BorderDiagonalLineStyle;
        toCellStyle.BorderLeft = fromCellStyle.BorderLeft;
        toCellStyle.BorderRight = fromCellStyle.BorderRight;
        toCellStyle.BorderTop = fromCellStyle.BorderTop;
        toCellStyle.BottomBorderColor = fromCellStyle.BottomBorderColor;
        toCellStyle.DataFormat = fromCellStyle.DataFormat;
        toCellStyle.FillBackgroundColor = fromCellStyle.FillBackgroundColor;
        toCellStyle.FillForegroundColor = fromCellStyle.FillForegroundColor;
        toCellStyle.FillPattern = fromCellStyle.FillPattern;
        toCellStyle.Indention = fromCellStyle.Indention;
        toCellStyle.IsHidden = fromCellStyle.IsHidden;
        toCellStyle.IsLocked = fromCellStyle.IsLocked;
        toCellStyle.LeftBorderColor = fromCellStyle.LeftBorderColor;
        toCellStyle.RightBorderColor = fromCellStyle.RightBorderColor;
        toCellStyle.Rotation = fromCellStyle.Rotation;
        toCellStyle.ShrinkToFit = fromCellStyle.ShrinkToFit;
        toCellStyle.TopBorderColor = fromCellStyle.TopBorderColor;
        toCellStyle.VerticalAlignment = fromCellStyle.VerticalAlignment;
        toCellStyle.WrapText = fromCellStyle.WrapText;
        toCellStyle.SetFont(toWorkbook.GetFontAt((short)(fromCellStyle.GetFont(fromWorkbook).Index + 1)));
    }
}