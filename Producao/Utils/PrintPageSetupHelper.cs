using ClosedXML.Excel;
using OfficeOpenXml;
using Producao.Views.CentralModelos.Compat;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.Model.Printing;

namespace Producao.Utils
{
    internal static class PrintPageSetupHelper
    {
        public const double LeftMargin = 0d;
        public const double RightMargin = 0d;
        public const double TopMargin = 0.5d / 2.54d;
        public const double BottomMargin = 0.5d / 2.54d;
        public const double HeaderMargin = 0d;
        public const double FooterMargin = 0d;

        public static void ApplyA4Margins(IXLWorksheet worksheet)
        {
            worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
            worksheet.PageSetup.Margins.Left = LeftMargin;
            worksheet.PageSetup.Margins.Right = RightMargin;
            worksheet.PageSetup.Margins.Top = TopMargin;
            worksheet.PageSetup.Margins.Bottom = BottomMargin;
            worksheet.PageSetup.Margins.Header = HeaderMargin;
            worksheet.PageSetup.Margins.Footer = FooterMargin;
        }

        public static void ApplyA4Margins(ExcelWorksheet worksheet)
        {
            worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
            worksheet.PrinterSettings.LeftMargin = LeftMargin;
            worksheet.PrinterSettings.RightMargin = RightMargin;
            worksheet.PrinterSettings.TopMargin = TopMargin;
            worksheet.PrinterSettings.BottomMargin = BottomMargin;
            worksheet.PrinterSettings.HeaderMargin = HeaderMargin;
            worksheet.PrinterSettings.FooterMargin = FooterMargin;
        }

        public static void ApplyA4Margins(Worksheet worksheet)
        {
            worksheet.WorksheetPageSetup.PaperType = PaperTypes.A4;
            worksheet.WorksheetPageSetup.Margins = new PageMargins(
                LeftMargin,
                TopMargin,
                RightMargin,
                BottomMargin,
                HeaderMargin,
                FooterMargin);
        }

        public static void ApplyA4Margins(IWorksheet worksheet)
        {
            worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
            worksheet.PageSetup.LeftMargin = LeftMargin;
            worksheet.PageSetup.RightMargin = RightMargin;
            worksheet.PageSetup.TopMargin = TopMargin;
            worksheet.PageSetup.BottomMargin = BottomMargin;
            worksheet.PageSetup.HeaderMargin = HeaderMargin;
            worksheet.PageSetup.FooterMargin = FooterMargin;
        }
    }
}
