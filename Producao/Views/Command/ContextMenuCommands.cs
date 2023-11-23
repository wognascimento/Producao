using Producao.Views.CheckList;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
//using Excel = Microsoft.Office.Interop.Excel;

namespace Producao
{
    public static class ContextMenuCommands
    {
        
        enum Etiqueta
        {
            Primeira,
            Segunda,
            Terceira,
            Quarta,
            Quinta,
            Sexta
        }

        #region PrintEtiquetaEmitir
        private static BaseCommand printEtiqueta;
        public static BaseCommand PrintEtiqueta
        {
            get
            {
                if (printEtiqueta == null)
                    printEtiqueta = new BaseCommand(OnImprimirEmitirClicked);
                return printEtiqueta;
            }
        }
        private static void OnImprimirEmitirClicked(object obj)
        {
            if (obj is GridColumnContextMenuInfo)
            {
                var grid = ((GridContextMenuInfo)obj).DataGrid;
                EtiquetaViewModel vm = (EtiquetaViewModel)grid.DataContext; //DataContext = {Producao.Views.EtiquetaViewModel}
                var itens = (ObservableCollection<EtiquetaProducaoModel>)grid.ItemsSource; //

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open("Modelos/ETIQUETA_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];

                var etiqueta = Enum.Parse(typeof(Etiqueta), "Primeira");

                foreach (var item in itens)
                {
                    switch (etiqueta)
                    {
                        case Etiqueta.Primeira:
                            {
                                worksheet.Range["A1"].Text = vm.Sigla.sigla_serv;
                                worksheet.Range["B2"].Text = vm.Dado.qtd_nao_expedida.ToString();
                                worksheet.Range["C2"].Text = vm.BaseSettings.Database;
                                worksheet.Range["D2"].Text = vm.Dado.coddetalhescompl.ToString();
                                worksheet.Range["B5"].Text = vm.Dado.local_shoppings;
                                worksheet.Range["A8"].Text = vm.Dado.descricao_completa.Replace("ÚNICO", null);

                                etiqueta = Enum.Parse(typeof(Etiqueta), "Segunda");
                                break;
                            }
                        case Etiqueta.Segunda:
                            {
                                worksheet.Range["F1"].Text = vm.Sigla.sigla_serv;
                                worksheet.Range["G2"].Text = vm.Dado.qtd_nao_expedida.ToString();
                                worksheet.Range["H2"].Text = vm.BaseSettings.Database;
                                worksheet.Range["I2"].Text = vm.Dado.coddetalhescompl.ToString();
                                worksheet.Range["G5"].Text = vm.Dado.local_shoppings;
                                worksheet.Range["F8"].Text = vm.Dado.descricao_completa.Replace("ÚNICO", null);
                                etiqueta = Enum.Parse(typeof(Etiqueta), "Terceira");
                                break;
                            }
                        case Etiqueta.Terceira:
                            {
                                worksheet.Range["A13"].Text = vm.Sigla.sigla_serv;
                                worksheet.Range["B14"].Text = vm.Dado.qtd_nao_expedida.ToString();
                                worksheet.Range["C14"].Text = vm.BaseSettings.Database;
                                worksheet.Range["D14"].Text = vm.Dado.coddetalhescompl.ToString();
                                worksheet.Range["B17"].Text = vm.Dado.local_shoppings;
                                worksheet.Range["A20"].Text = vm.Dado.descricao_completa.Replace("ÚNICO", null);
                                etiqueta = Enum.Parse(typeof(Etiqueta), "Quarta");
                                break;
                            }
                        case Etiqueta.Quarta:
                            {
                                worksheet.Range["A13"].Text = vm.Sigla.sigla_serv;
                                worksheet.Range["G14"].Text = vm.Dado.qtd_nao_expedida.ToString();
                                worksheet.Range["H14"].Text = vm.BaseSettings.Database;
                                worksheet.Range["I14"].Text = vm.Dado.coddetalhescompl.ToString();
                                worksheet.Range["G17"].Text = vm.Dado.local_shoppings;
                                worksheet.Range["F20"].Text = vm.Dado.descricao_completa.Replace("ÚNICO", null);
                                etiqueta = Enum.Parse(typeof(Etiqueta), "Quinta");
                                break;
                            }
                        case Etiqueta.Quinta:
                            {
                                worksheet.Range["A25"].Text = vm.Sigla.sigla_serv;
                                worksheet.Range["B26"].Text = vm.Dado.qtd_nao_expedida.ToString();
                                worksheet.Range["C26"].Text = vm.BaseSettings.Database;
                                worksheet.Range["D26"].Text = vm.Dado.coddetalhescompl.ToString();
                                worksheet.Range["B29"].Text = vm.Dado.local_shoppings;
                                worksheet.Range["A32"].Text = vm.Dado.descricao_completa.Replace("ÚNICO", null);
                                etiqueta = Enum.Parse(typeof(Etiqueta), "Sexta");
                                break;
                            }
                        case Etiqueta.Sexta:
                            {
                                worksheet.Range["F25"].Text = vm.Sigla.sigla_serv;
                                worksheet.Range["G26"].Text = vm.Dado.qtd_nao_expedida.ToString();
                                worksheet.Range["H26"].Text = vm.BaseSettings.Database;
                                worksheet.Range["I26"].Text = vm.Dado.coddetalhescompl.ToString();
                                worksheet.Range["G29"].Text = vm.Dado.local_shoppings;
                                worksheet.Range["F32"].Text = vm.Dado.descricao_completa.Replace("ÚNICO", null);
                                etiqueta = Enum.Parse(typeof(Etiqueta), "Primeira");
                                break;
                            }
                        default: break;
                    }
                    Console.WriteLine(etiqueta);
                }
                workbook.SaveAs($"Impressos/ETIQUETA_{vm.Dado.coddetalhescompl}.xlsx"); 
            }
        }
        #endregion

        #region PrintEtiquetaEmitir
        private static BaseCommand printEtiquetaEmitidas;
        public static BaseCommand PrintEtiquetaEmitidas
        {
            get
            {
                if (printEtiquetaEmitidas == null)
                    printEtiquetaEmitidas = new BaseCommand(OnImprimirEmitidasClicked);
                return printEtiquetaEmitidas;
            }
        }
        private static void OnImprimirEmitidasClicked(object obj)
        {
            if (obj is GridColumnContextMenuInfo)
            {
                var grid = ((GridContextMenuInfo)obj).DataGrid;
                EtiquetaEmitidaViewModel vm = (EtiquetaEmitidaViewModel)grid.DataContext; //DataContext = {Producao.Views.EtiquetaViewModel}
                //var itens = (ObservableCollection<EtiquetaEmitidaModel>)grid.ItemsSource; //
                var filteredResult = grid.View.Records.Select(recordentry => recordentry.Data);
                var itens = grid.View.Records.Count;

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open("Modelos/ETIQUETA_MODELO.xlsx");
                //IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];

                var etiqueta = Enum.Parse(typeof(Etiqueta), "Primeira");
                int paginas = (int)Math.Ceiling(Decimal.Divide(itens, 6));
                int _etiqueta = 1;
                int _pagina = 1;

                //foreach (EtiquetaEmitidaModel item in filteredResult)
                for (int i = 0; i < grid.View.Records.Count; i++)
                {

                    try
                    {

                        EtiquetaEmitidaModel item = (EtiquetaEmitidaModel)grid.View.Records[i].Data;
                        switch (etiqueta)
                        {
                            case Etiqueta.Primeira:
                                {
                                    worksheet.Range["A1"].Text = item.sigla;
                                    worksheet.Range["B2"].Text = item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                    worksheet.Range["C2"].Text = vm.BaseSettings.Database;
                                    worksheet.Range["D2"].Text = item.coddetalhescompl.ToString();
                                    worksheet.Range["B5"].Text = item.local_shoppings;
                                    worksheet.Range["A8"].Text = item.descricao_completa.Replace("ÚNICO", null);

                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["PRIMEIRA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                    etiqueta = Enum.Parse(typeof(Etiqueta), "Segunda");
                                    _etiqueta++;
                                    workbook.SaveAs($"Impressos/ETIQUETA_{_pagina}.xlsx");
                                    break;
                                }
                            case Etiqueta.Segunda:
                                {
                                    worksheet.Range["F1"].Text = item.sigla;
                                    worksheet.Range["G2"].Text = item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                    worksheet.Range["H2"].Text = vm.BaseSettings.Database;
                                    worksheet.Range["I2"].Text = item.coddetalhescompl.ToString();
                                    worksheet.Range["G5"].Text = item.local_shoppings;
                                    worksheet.Range["F8"].Text = item.descricao_completa.Replace("ÚNICO", null);

                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEGUNDA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                    etiqueta = Enum.Parse(typeof(Etiqueta), "Terceira");
                                    _etiqueta++;
                                    workbook.SaveAs($"Impressos/ETIQUETA_{_pagina}.xlsx");
                                    break;
                                }
                            case Etiqueta.Terceira:
                                {
                                    worksheet.Range["A13"].Text = item.sigla;
                                    worksheet.Range["B14"].Text = item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                    worksheet.Range["C14"].Text = vm.BaseSettings.Database;
                                    worksheet.Range["D14"].Text = item.coddetalhescompl.ToString();
                                    worksheet.Range["B17"].Text = item.local_shoppings;
                                    worksheet.Range["A20"].Text = item.descricao_completa.Replace("ÚNICO", null);

                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["TERCEIRA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                    etiqueta = Enum.Parse(typeof(Etiqueta), "Quarta");
                                    _etiqueta++;
                                    workbook.SaveAs($"Impressos/ETIQUETA_{_pagina}.xlsx");
                                    break;
                                }
                            case Etiqueta.Quarta:
                                {
                                    worksheet.Range["F13"].Text = item.sigla;
                                    worksheet.Range["G14"].Text = item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                    worksheet.Range["H14"].Text = vm.BaseSettings.Database;
                                    worksheet.Range["I14"].Text = item.coddetalhescompl.ToString();
                                    worksheet.Range["G17"].Text = item.local_shoppings;
                                    worksheet.Range["F20"].Text = item.descricao_completa.Replace("ÚNICO", null);

                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUARTA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                    etiqueta = Enum.Parse(typeof(Etiqueta), "Quinta");
                                    _etiqueta++;
                                    workbook.SaveAs($"Impressos/ETIQUETA_{_pagina}.xlsx");
                                    break;
                                }
                            case Etiqueta.Quinta:
                                {
                                    worksheet.Range["A25"].Text = item.sigla;
                                    worksheet.Range["B26"].Text = item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                    worksheet.Range["C26"].Text = vm.BaseSettings.Database;
                                    worksheet.Range["D26"].Text = item.coddetalhescompl.ToString();
                                    worksheet.Range["B29"].Text = item.local_shoppings;
                                    worksheet.Range["A32"].Text = item.descricao_completa.Replace("ÚNICO", null);

                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["QUINTA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                    etiqueta = Enum.Parse(typeof(Etiqueta), "Sexta");
                                    _etiqueta++;
                                    workbook.SaveAs($"Impressos/ETIQUETA_{_pagina}.xlsx");
                                    break;
                                }
                            case Etiqueta.Sexta:
                                {
                                    worksheet.Range["F25"].Text = item.sigla;
                                    worksheet.Range["G26"].Text = item.volumes > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                    worksheet.Range["H26"].Text = vm.BaseSettings.Database;
                                    worksheet.Range["I26"].Text = item.coddetalhescompl.ToString();
                                    worksheet.Range["G29"].Text = item.local_shoppings;
                                    worksheet.Range["F32"].Text = item.descricao_completa.Replace("ÚNICO", null);

                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    worksheet.Range["SEXTA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                    etiqueta = Enum.Parse(typeof(Etiqueta), "Primeira");
                                    _etiqueta = 1;
                                    workbook.SaveAs($"Impressos/ETIQUETA_{_pagina}.xlsx");

                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["PRIMEIRA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEGUNDA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["TERCEIRA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUARTA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUINTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["QUINTA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEXTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                    worksheet.Range["SEXTA"].CellStyle.Font.Color = ExcelKnownColors.None;


                                    _pagina++;
                                    break;
                                }
                            default: break;
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                }

                try
                {
                    string path = Directory.GetCurrentDirectory();
                    for (int i = 1; i <= paginas; i++)
                    {
                        string file = @$"{path}\Impressos\ETIQUETA_{i}.xlsx";
                        //var excelApp = new Excel.Application();

                        //Excel.Workbooks books = excelApp.Workbooks;
                        //Excel._Workbook sheet = books.Open(file);
                        //sheet.PrintOutEx();

                        //sheet.Close();
                        //books.Close();
                        //excelApp.Quit();

                        Process.Start(
                            new ProcessStartInfo(file)
                            {
                                Verb = "Print",
                                UseShellExecute = true,
                            });

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
               

            }
        }
        #endregion
    }
}
