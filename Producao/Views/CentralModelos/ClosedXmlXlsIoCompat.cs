using ClosedXML.Excel;
using System;
using System.Collections;
using System.Linq;

namespace Producao.Views.CentralModelos.Compat
{
    internal sealed class ExcelEngine : IDisposable
    {
        public IApplication Excel { get; } = new();

        public void Dispose()
        {
        }
    }

    internal sealed class IApplication
    {
        public ExcelVersion DefaultVersion { get; set; }
        public WorkbookCollection Workbooks { get; } = new();
    }

    internal sealed class WorkbookCollection
    {
        public IWorkbook Open(string filePath) => new(new XLWorkbook(filePath));
        public IWorkbook OpenReadOnly(string filePath) => Open(filePath);
        public IWorkbook Create(int sheetCount) => new(new XLWorkbook(), sheetCount);
    }

    internal sealed class IWorkbook : IDisposable
    {
        private readonly XLWorkbook workbook;

        public IWorkbook(XLWorkbook workbook, int sheetCount = 0)
        {
            this.workbook = workbook;
            if (sheetCount > 0)
            {
                for (var i = 0; i < sheetCount; i++)
                    this.workbook.Worksheets.Add($"Planilha{i + 1}");
            }

            Worksheets = new WorksheetCollection(this.workbook);
            Styles = new StyleCollection();
        }

        public WorksheetCollection Worksheets { get; }
        public StyleCollection Styles { get; }

        public void SaveAs(string filePath) => workbook.SaveAs(filePath);
        public void Close() => Dispose();
        public void Dispose() => workbook.Dispose();
    }

    internal sealed class WorksheetCollection
    {
        private readonly XLWorkbook workbook;

        public WorksheetCollection(XLWorkbook workbook)
        {
            this.workbook = workbook;
        }

        public IWorksheet this[int index] => new(workbook.Worksheet(index + 1));
    }

    internal sealed class StyleCollection
    {
        public IStyle Add(string name) => new();
    }

    internal sealed class IWorksheet
    {
        private readonly IXLWorksheet worksheet;

        public IWorksheet(IXLWorksheet worksheet)
        {
            this.worksheet = worksheet;
            Range = new RangeAccessor(worksheet);
            Rows = new RowCollection(worksheet);
            PageSetup = new PageSetupAdapter(worksheet);
        }

        public RangeAccessor Range { get; }
        public RowCollection Rows { get; }
        public PageSetupAdapter PageSetup { get; }
        public bool UsedRangeIncludesFormatting { get; set; }

        public bool TabSelected
        {
            get => worksheet.TabSelected;
            set => worksheet.TabSelected = value;
        }

        public bool TabActive
        {
            get => worksheet.TabActive;
            set => worksheet.TabActive = value;
        }

        public bool IsGridLinesVisible
        {
            get => worksheet.ShowGridLines;
            set => worksheet.ShowGridLines = value;
        }

        public IRange UsedRange => new(worksheet.RangeUsed() ?? worksheet.Range("A1"));

        public IRange this[int firstRow, int firstColumn, int lastRow, int lastColumn]
            => new(worksheet.Range(firstRow, firstColumn, lastRow, lastColumn));

        public void ShowRange(IRange range, bool show)
        {
            foreach (var row in range.Rows())
            {
                if (show)
                    row.Worksheet.Row(row.RowNumber()).Unhide();
                else
                    row.Worksheet.Row(row.RowNumber()).Hide();
            }
        }

        public void Clear() => worksheet.Clear();

        public void Hide() => worksheet.Hide();

        public void Unhide() => worksheet.Unhide();

        public void ImportData(IEnumerable data, int firstRow, int firstColumn, bool includeHeaders)
        {
            var row = firstRow;
            foreach (var item in data)
            {
                if (item is null)
                    continue;

                var properties = item.GetType().GetProperties();
                if (includeHeaders && row == firstRow)
                {
                    for (var i = 0; i < properties.Length; i++)
                        worksheet.Cell(row, firstColumn + i).Value = properties[i].Name;
                    row++;
                }

                for (var i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item);
                    worksheet.Cell(row, firstColumn + i).Value = XLCellValue.FromObject(value ?? string.Empty);
                }

                row++;
            }
        }
    }

    internal sealed class RowCollection
    {
        private readonly IXLWorksheet worksheet;

        public RowCollection(IXLWorksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        public RowAdapter this[int index] => new(worksheet.Row(index + 1));
    }

    internal sealed class RowAdapter
    {
        private readonly IXLRow row;

        public RowAdapter(IXLRow row)
        {
            this.row = row;
        }

        public IStyle CellStyle
        {
            get => new(row.Style);
            set => row.Style = value.Style;
        }
    }

    internal sealed class RangeAccessor
    {
        private readonly IXLWorksheet worksheet;

        public RangeAccessor(IXLWorksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        public IRange this[string address] => new(worksheet.Range(address));
    }

    internal sealed class IRange
    {
        private readonly IXLRange range;

        public IRange(IXLRange range)
        {
            this.range = range;
        }

        public IStyle CellStyle
        {
            get => new(range.Style);
            set => range.Style = value.Style;
        }

        public BorderCollection Borders => new(range.Style);

        public string? Text
        {
            get => range.FirstCell().GetString();
            set => range.FirstCell().Value = value ?? string.Empty;
        }

        public object? Value
        {
            get => range.FirstCell().GetString();
            set => range.FirstCell().Value = XLCellValue.FromObject(value ?? string.Empty);
        }

        public double Number
        {
            get => range.FirstCell().GetDouble();
            set => range.FirstCell().Value = value;
        }

        public DateTime DateTime
        {
            get => range.FirstCell().GetDateTime();
            set => range.FirstCell().Value = value;
        }

        public bool WrapText
        {
            get => range.Style.Alignment.WrapText;
            set => range.Style.Alignment.WrapText = value;
        }

        public double RowHeight
        {
            set
            {
                foreach (var row in range.Rows())
                    row.Worksheet.Row(row.RowNumber()).Height = value;
            }
        }

        public double ColumnWidth
        {
            set
            {
                foreach (var column in range.Columns())
                    column.WorksheetColumn().Width = value;
            }
        }

        public ExcelHAlign HorizontalAlignment
        {
            set => range.Style.Alignment.Horizontal = ToHorizontal(value);
        }

        public ExcelVAlign VerticalAlignment
        {
            set => range.Style.Alignment.Vertical = ToVertical(value);
        }

        public string NumberFormat
        {
            set => range.Style.NumberFormat.Format = value;
        }

        public IRange Merge()
        {
            range.Merge();
            return this;
        }

        public void BorderAround() => range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        public void AutofitColumns() => range.Worksheet.ColumnsUsed().AdjustToContents();

        public IXLRangeRows Rows() => range.Rows();

        private static XLAlignmentHorizontalValues ToHorizontal(ExcelHAlign value)
            => value switch
            {
                ExcelHAlign.HAlignCenter => XLAlignmentHorizontalValues.Center,
                ExcelHAlign.HAlignRight => XLAlignmentHorizontalValues.Right,
                _ => XLAlignmentHorizontalValues.Left
            };

        private static XLAlignmentVerticalValues ToVertical(ExcelVAlign value)
            => value == ExcelVAlign.VAlignCenter
                ? XLAlignmentVerticalValues.Center
                : XLAlignmentVerticalValues.Top;
    }

    internal sealed class IStyle
    {
        public IStyle()
            : this(new XLWorkbook().Style)
        {
        }

        public IStyle(IXLStyle style)
        {
            Style = style;
            Borders = new BorderCollection(style);
            Font = new FontAdapter(style);
        }

        public IXLStyle Style { get; }
        public BorderCollection Borders { get; }
        public FontAdapter Font { get; }
        public object Color
        {
            set
            {
                Style.Fill.BackgroundColor = value switch
                {
                    ExcelKnownColors knownColor => ExcelColorHelper.ToColor(knownColor),
                    System.Drawing.Color drawingColor => XLColor.FromArgb(drawingColor.R, drawingColor.G, drawingColor.B),
                    _ => Style.Fill.BackgroundColor
                };
            }
        }

        public ExcelHAlign HorizontalAlignment
        {
            set => Style.Alignment.Horizontal = value switch
            {
                ExcelHAlign.HAlignCenter => XLAlignmentHorizontalValues.Center,
                ExcelHAlign.HAlignRight => XLAlignmentHorizontalValues.Right,
                _ => XLAlignmentHorizontalValues.Left
            };
        }

        public ExcelVAlign VerticalAlignment
        {
            set => Style.Alignment.Vertical = value == ExcelVAlign.VAlignCenter
                ? XLAlignmentVerticalValues.Center
                : XLAlignmentVerticalValues.Top;
        }

        public bool WrapText
        {
            set => Style.Alignment.WrapText = value;
        }

        public void BeginUpdate()
        {
        }

        public void EndUpdate()
        {
        }
    }

    internal sealed class BorderCollection
    {
        private readonly IXLStyle style;

        public BorderCollection(IXLStyle style)
        {
            this.style = style;
        }

        public BorderAdapter this[ExcelBordersIndex index] => new(style, index);
    }

    internal sealed class BorderAdapter
    {
        private readonly IXLStyle style;
        private readonly ExcelBordersIndex index;

        public BorderAdapter(IXLStyle style, ExcelBordersIndex index)
        {
            this.style = style;
            this.index = index;
        }

        public ExcelLineStyle LineStyle
        {
            set
            {
                var lineStyle = value == ExcelLineStyle.Thin
                    ? XLBorderStyleValues.Thin
                    : value == ExcelLineStyle.Dashed
                        ? XLBorderStyleValues.Dashed
                        : XLBorderStyleValues.None;

                switch (index)
                {
                    case ExcelBordersIndex.EdgeTop:
                        style.Border.TopBorder = lineStyle;
                        break;
                    case ExcelBordersIndex.EdgeBottom:
                        style.Border.BottomBorder = lineStyle;
                        break;
                    case ExcelBordersIndex.EdgeLeft:
                        style.Border.LeftBorder = lineStyle;
                        break;
                    case ExcelBordersIndex.EdgeRight:
                        style.Border.RightBorder = lineStyle;
                        break;
                }
            }
        }

        public ExcelKnownColors Color
        {
            set
            {
                var color = ExcelColorHelper.ToColor(value);
                switch (index)
                {
                    case ExcelBordersIndex.EdgeTop:
                        style.Border.TopBorderColor = color;
                        break;
                    case ExcelBordersIndex.EdgeBottom:
                        style.Border.BottomBorderColor = color;
                        break;
                    case ExcelBordersIndex.EdgeLeft:
                        style.Border.LeftBorderColor = color;
                        break;
                    case ExcelBordersIndex.EdgeRight:
                        style.Border.RightBorderColor = color;
                        break;
                }
            }
        }
    }

    internal sealed class FontAdapter
    {
        private readonly IXLStyle style;

        public FontAdapter(IXLStyle style)
        {
            this.style = style;
        }

        public bool Bold
        {
            set => style.Font.Bold = value;
        }

        public double Size
        {
            set => style.Font.FontSize = value;
        }

        public string FontName
        {
            set => style.Font.FontName = value;
        }

        public ExcelKnownColors Color
        {
            set => style.Font.FontColor = ExcelColorHelper.ToColor(value);
        }
    }

    internal sealed class PageSetupAdapter
    {
        private readonly IXLWorksheet worksheet;

        public PageSetupAdapter(IXLWorksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        public ExcelPageOrientation Orientation
        {
            set => worksheet.PageSetup.PageOrientation = value == ExcelPageOrientation.Landscape
                ? XLPageOrientation.Landscape
                : XLPageOrientation.Portrait;
        }

        public double LeftMargin { set => worksheet.PageSetup.Margins.Left = value; }
        public double RightMargin { set => worksheet.PageSetup.Margins.Right = value; }
        public double TopMargin { set => worksheet.PageSetup.Margins.Top = value; }
        public double BottomMargin { set => worksheet.PageSetup.Margins.Bottom = value; }
        public string RightFooter { set => worksheet.PageSetup.Footer.Right.AddText(value); }
        public string LeftFooter { set => worksheet.PageSetup.Footer.Left.AddText(value); }
        public bool CenterVertically { set => worksheet.PageSetup.CenterVertically = value; }
        public bool CenterHorizontally { set => worksheet.PageSetup.CenterHorizontally = value; }
        public string PrintTitleColumns { set { } }
        public string PrintTitleRows { set { } }
        public string PrintArea
        {
            set
            {
                worksheet.PageSetup.PrintAreas.Clear();
                worksheet.PageSetup.PrintAreas.Add(value.Replace("$", string.Empty));
            }
        }
        public int FitToPagesWide { set => worksheet.PageSetup.PagesWide = value; }
        public int FitToPagesTall { set => worksheet.PageSetup.PagesTall = value; }
        public int Zoom { set => worksheet.PageSetup.Scale = value; }
    }

    internal static class ExcelColorHelper
    {
        public static XLColor ToColor(ExcelKnownColors value)
            => value switch
            {
                ExcelKnownColors.Black => XLColor.Black,
                ExcelKnownColors.None => XLColor.NoColor,
                _ => XLColor.FromArgb(191, 191, 191)
            };
    }

    internal enum ExcelVersion { Xlsx }
    internal enum ExcelBordersIndex { EdgeTop, EdgeBottom, EdgeLeft, EdgeRight }
    internal enum ExcelLineStyle { None, Thin, Dashed }
    internal enum ExcelKnownColors { None, Black, Grey_25_percent }
    internal enum ExcelHAlign { HAlignLeft, HAlignCenter, HAlignRight }
    internal enum ExcelVAlign { VAlignTop, VAlignCenter }
    internal enum ExcelPageOrientation { Portrait, Landscape }
}
