using OfficeOpenXml;
using System.Reflection;

ExcelPackage.License.SetNonCommercialOrganization("SIG");
var assembly = typeof(Producao.MainWindow).Assembly;
var exporter = assembly.GetType("Producao.Views.kit.KitRequisicaoExporter", true)!;
var method = exporter.GetMethod("PreencherEtiquetas", BindingFlags.NonPublic | BindingFlags.Static)!;
var model = method.GetParameters()[1].ParameterType.GetGenericArguments()[0];
foreach (int count in new[] { 1, 2, 3, 4, 5 })
{
    using var package = new ExcelPackage();
    var sheet = package.Workbook.Worksheets.Add("Etiquetas");
    sheet.PrinterSettings.TopMargin = sheet.PrinterSettings.BottomMargin = 0.5d / 2.54d;
    var items = Array.CreateInstance(model, count);
    for (int i = 0; i < count; i++) items.SetValue(Activator.CreateInstance(model), i);
    method.Invoke(null, new object[] { sheet, items });
    if (!sheet.PrinterSettings.FitToPage || sheet.PrinterSettings.FitToWidth != 1 ||
        sheet.PrinterSettings.FitToHeight != 0 || sheet.PrinterSettings.PrintArea.End.Row != count * 16 - 1)
        throw new Exception("Invalid print area or scaling mode.");
    for (int row = 1; row < count * 16; row++)
        if (sheet.Row(row).PageBreak != (row % 32 == 31 && row < count * 16 - 1)) throw new Exception("Incorrect page break.");
    for (int i = 1; i <= count; i++)
    {
        if (sheet.Row(i * 16).Height != 78) throw new Exception("Incorrect spacer height.");
        if (sheet.Row(i * 16).Hidden != (i % 2 == 0)) throw new Exception("Page spacer must not be printed.");
    }
    using var saved = new System.IO.MemoryStream(package.GetAsByteArray());
    using var reopened = new ExcelPackage(saved);
    var settings = reopened.Workbook.Worksheets[0].PrinterSettings;
    for (int i = 1; i <= count; i++)
        if (reopened.Workbook.Worksheets[0].Row(i * 16).Hidden != (i % 2 == 0))
            throw new Exception("Spacer visibility was not persisted.");
    if (!settings.FitToPage || settings.FitToWidth != 1 || settings.FitToHeight != 0 ||
        settings.PaperSize != ePaperSize.A4 || settings.Orientation != eOrientation.Portrait)
        throw new Exception("Print settings were not persisted.");
    Console.WriteLine($"PASS: {count} labels, A4 portrait, width 1, automatic height; saved and reopened.");
}
