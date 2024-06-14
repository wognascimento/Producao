using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;


namespace Producao.Views.RelatoriosTecnicos
{
    /// <summary>
    /// Interação lógica para CargaEletrica.xam
    /// </summary>
    public partial class CargaEletrica : UserControl
    {
        public CargaEletrica()
        {
            InitializeComponent();
            DataContext = new CargaEletricaViewModel();

            //this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CargaEletricaViewModel vm = (CargaEletricaViewModel)DataContext;
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CargaEletricaViewModel vm = (CargaEletricaViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Itens = await Task.Run(async () => await vm.GetFechaAsync(vm?.Sigla?.sigla));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void RadGridView_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            //CurrentItem = {Producao.ViewFechaModel}
            //OriginalSource = {Telerik.Windows.Controls.RadGridView}
            CargaEletricaViewModel vm = (CargaEletricaViewModel)DataContext;
            RadGridView? grid = e.OriginalSource as RadGridView;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                if (e.Cell.Column.UniqueName == "cargaeletrica_led")
                {
                    var propostaDimensao = grid.Items.CurrentEditItem as ViewFechaModel; //grid.CurrentItem = {Producao.ViewFechaModel}
                    await Task.Run(() => vm.CargaelEtricaAsync(propostaDimensao));
                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex?.InnerException?.Message, "Erro ao inserir", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void RadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                CargaEletricaViewModel vm = (CargaEletricaViewModel)DataContext;
                DataBaseSettings BaseSettings = DataBaseSettings.Instance;
                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                worksheet.IsGridLinesVisible = false;

                IStyle headerStyle;
                IStyle bodyStyle;

                bodyStyle = workbook.Styles.Add("BodyStyle");
                bodyStyle.BeginUpdate();
                bodyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                bodyStyle.Font.Bold = true;
                bodyStyle.WrapText = true;
                bodyStyle.EndUpdate();

                headerStyle = workbook.Styles.Add("headerStyle");
                headerStyle.BeginUpdate();
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.WrapText = true;
                headerStyle.EndUpdate();

                worksheet.Range["A1"].Text = $"{vm.Sigla.nome}";
                worksheet.Range["A1"].CellStyle.Font.Bold = true;
                worksheet.Range["A1"].CellStyle.Font.Size = 25;

                worksheet.Range["A2"].Text = "CARGA ELÉTRICA APROXIMADA";
                worksheet.Range["A2"].CellStyle.Font.Bold = true;
                worksheet.Range["A2"].CellStyle.Font.Size = 25;

               
                worksheet.Range["A3"].Text = $"ITEM";
                worksheet.Range["A3"].ColumnWidth = 5;
                 
                worksheet.Range["B3"].Text = $"LOCAL";
                worksheet.Range["B3"].ColumnWidth = 20;
                
                worksheet.Range["C3"].Text = $"DESCRIÇÃO";
                worksheet.Range["C3"].ColumnWidth = 30;
                worksheet.Range["C3"].WrapText = true;

                worksheet.Range["D3"].Text = $"QTDE";
                worksheet.Range["D3"].ColumnWidth = 5;

                worksheet.Range["E3"].Text = $"DIMENSÃO";
                worksheet.Range["E3"].ColumnWidth = 30;
                worksheet.Range["E3"].WrapText = true;

                worksheet.Range["F3"].Text = $"DEMANDA TOTAL (kW)";
                worksheet.Range["F3"].ColumnWidth = 10;

                //var itensGrid = itens.Items.ToList;

                IEnumerable<object> items = this.itens.Items.Cast<ViewFechaModel>().Select(item => new { item.item, item.localitem, item.descricao, item.qtd, item.dimensao, demanda = ((item.cargaeletrica_led * item.qtd) * 220) / 1000 } ); //Producao.ViewFechaModel

                worksheet.ImportData(items, 4, 1, false);


                worksheet.Rows[2].CellStyle = bodyStyle;


                worksheet.Range[$"A4:F{items.Count() + 3}"].CellStyle = headerStyle;

                worksheet.Range[$"A4:A{items.Count() + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"A4:A{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"B4:B{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"C4:C{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"D4:D{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                worksheet.Range[$"D4:D{items.Count() + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                worksheet.Range[$"E4:E{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"F4:F{items.Count() + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"F4:F{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                //worksheet.Range[$"G3:G{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                //worksheet.Range[$"H3:H{items.Count() + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                //worksheet.Range[$"H3:H{items.Count() + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
               


                worksheet.PageSetup.PrintTitleColumns = "$A:$F";
                worksheet.PageSetup.PrintTitleRows = "$1:$3";
                worksheet.PageSetup.Orientation = ExcelPageOrientation.Portrait;
                worksheet.PageSetup.LeftMargin = 0.0;
                worksheet.PageSetup.RightMargin = 0.0;
                worksheet.PageSetup.TopMargin = 0.0;
                worksheet.PageSetup.BottomMargin = 0.5;
                worksheet.PageSetup.RightFooter = "&P";
                worksheet.PageSetup.LeftFooter = "&D";
                //worksheet.PageSetup.CenterVertically = true;
                worksheet.PageSetup.CenterHorizontally = true;

                workbook.SaveAs("Impressos/CARGA-ELETRICA.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\CARGA-ELETRICA.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class CargaEletricaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ViewFechaModel> _itens;
        public ObservableCollection<ViewFechaModel> Itens { get { return _itens; } set { _itens = value; RaisePropertyChanged("Itens"); } }

        private ViewFechaModel _item;
        public ViewFechaModel Item { get { return _item; } set { _item = value; RaisePropertyChanged("Item"); } }

        private ObservableCollection<ClientesModel> _siglas;
        public ObservableCollection<ClientesModel> Siglas { get { return _siglas; } set { _siglas = value; RaisePropertyChanged("Siglas"); } }
        private ClientesModel _sigla;
        public ClientesModel Sigla { get { return _sigla; } set { _sigla = value; RaisePropertyChanged("Sigla"); } }

        public async Task<ObservableCollection<ClientesModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                //var data = await db.Siglas.OrderBy(c => c.sigla_serv).ToListAsync();
                var data = await db.Siglas
                    .GroupBy(item => new { item.sigla, item.nome })
                    .OrderBy(group => group.Key.sigla) // Ordenar os grupos pela sigla
                    .Select(group => new ClientesModel
                    {
                        sigla = group.Key.sigla,
                        nome = group.Key.nome
                    }).ToListAsync();
                return new ObservableCollection<ClientesModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ViewFechaModel>> GetFechaAsync(string? sigla)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ViewFechas
                    .OrderBy(c => c.item)
                    .Where(c => c.sigla == sigla && c.descricao != "Terceirizado") //SeImed([bloco_revisao]=0 Ou [bloco_revisao]=10 Ou [item] Como "*B";1;0)
                    .ToListAsync();
                return new ObservableCollection<ViewFechaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CargaelEtricaAsync(ViewFechaModel? propostaDimensao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.propostaDimensoes.FindAsync(propostaDimensao.coddimensao);
                //var comple = await db.ComplementoCheckLists.FirstOrDefaultAsync(p => p.codcompl == compChkList.codcompl);
                if (data != null)
                {
                    if (data.cargaeletrica_led.HasValue)
                    {
                        data.cargaeletrica_led = propostaDimensao.cargaeletrica_led;
                        db.Entry(data).Property(p => p.cargaeletrica_led).IsModified = true;
                    }
                    await db.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
