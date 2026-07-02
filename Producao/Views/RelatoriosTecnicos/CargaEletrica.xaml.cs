using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Documents.Common.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;


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
                vm.Siglas = await vm.GetSiglasAsync();
                
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CargaEletricaViewModel vm = (CargaEletricaViewModel)DataContext;
                if (sender is RadComboBox combo)
                    vm.Sigla = combo.SelectedItem as ClientesModel;

                if (vm.Sigla == null)
                {
                    vm.Itens = [];
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Itens = await vm.GetFechaAsync(vm?.Sigla?.sigla);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");

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
                    await vm.CargaelEtricaAsync(propostaDimensao);
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

                var workbook = new Workbook();
                var worksheet = workbook.Worksheets.Add();
            Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.Name = "Carga Elétrica";
                worksheet.WorksheetPageSetup.FitToPages = false;
                worksheet.WorksheetPageSetup.ScaleFactor = new Size(0.97, 0.97);

                var items = this.itens.Items.Cast<ViewFechaModel>().ToList();
                var border = new CellBorder(CellBorderStyle.Thin, ThemableColor.FromColor(System.Windows.Media.Colors.Gray));
                var allBorders = new CellBorders(
                    border,
                    border,
                    border,
                    border,
                    border,
                    border,
                    CellBorder.Default,
                    CellBorder.Default);

                SetColumnWidth(worksheet, 0, 45);
                SetColumnWidth(worksheet, 1, 150);
                SetColumnWidth(worksheet, 2, 230);
                SetColumnWidth(worksheet, 3, 45);
                SetColumnWidth(worksheet, 4, 230);
                SetColumnWidth(worksheet, 5, 95);

                SetText(worksheet, 0, 0, vm.Sigla?.nome ?? string.Empty);
                SetTitleStyle(worksheet.Cells[0, 0]);

                SetText(worksheet, 1, 0, "CARGA ELÉTRICA APROXIMADA");
                SetTitleStyle(worksheet.Cells[1, 0]);

                SetText(worksheet, 2, 0, "ITEM");
                SetText(worksheet, 2, 1, "LOCAL");
                SetText(worksheet, 2, 2, "DESCRIÇÃO");
                SetText(worksheet, 2, 3, "QTDE");
                SetText(worksheet, 2, 4, "DIMENSÃO");
                SetText(worksheet, 2, 5, "DEMANDA TOTAL (kW)");

                var headerRange = worksheet.Cells[2, 0, 2, 5];
                headerRange.SetIsBold(true);
                headerRange.SetIsWrapped(true);
                headerRange.SetBorders(allBorders);
                headerRange.SetHorizontalAlignment(RadHorizontalAlignment.Center);
                headerRange.SetVerticalAlignment(RadVerticalAlignment.Center);

                for (var index = 0; index < items.Count; index++)
                {
                    var item = items[index];
                    var row = index + 3;
                    var demanda = (Convert.ToDouble(item.cargaeletrica_led) * Convert.ToDouble(item.qtd) * 220) / 1000;

                    SetText(worksheet, row, 0, item.item);
                    SetText(worksheet, row, 1, item.localitem);
                    SetText(worksheet, row, 2, item.descricao);
                    worksheet.Cells[row, 3].SetValue(Convert.ToDouble(item.qtd));
                    SetText(worksheet, row, 4, item.dimensao);
                    worksheet.Cells[row, 5].SetValue(demanda);
                }

                if (items.Count > 0)
                {
                    var dataRange = worksheet.Cells[3, 0, items.Count + 2, 5];
                    dataRange.SetBorders(allBorders);
                    dataRange.SetIsWrapped(true);
                    dataRange.SetVerticalAlignment(RadVerticalAlignment.Center);

                    worksheet.Cells[3, 0, items.Count + 2, 0].SetHorizontalAlignment(RadHorizontalAlignment.Center);
                    worksheet.Cells[3, 3, items.Count + 2, 3].SetHorizontalAlignment(RadHorizontalAlignment.Center);
                    worksheet.Cells[3, 5, items.Count + 2, 5].SetHorizontalAlignment(RadHorizontalAlignment.Center);
                    worksheet.Cells[3, 5, items.Count + 2, 5].SetFormat(new CellValueFormat("0,00"));
                }

                var caminhoArquivo = BaseSettings.ResolveImpressosPath("CARGA-ELETRICA.xlsx");
                using (var output = File.Open(caminhoArquivo, FileMode.Create))
                {
                    new XlsxFormatProvider().Export(workbook, output, TimeSpan.FromSeconds(30));
                }

                Process.Start(new ProcessStartInfo(caminhoArquivo)
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });


            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private static void SetText(Worksheet worksheet, int row, int column, string? value)
        {
            worksheet.Cells[row, column].SetValueAsText(value ?? string.Empty);
        }

        private static void SetTitleStyle(CellSelection cell)
        {
            cell.SetIsBold(true);
            cell.SetFontSize(25);
        }

        private static void SetColumnWidth(Worksheet worksheet, int column, double width)
        {
            worksheet.Columns[column].SetWidth(new ColumnWidth(width, true));
        }
    }

    public class CargaEletricaViewModel : INotifyPropertyChanged
    {
        private static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
        }

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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<ClientesModel>(
                    @"SELECT sigla, nome
                      FROM producao.view_sigla_chkgeral
                      GROUP BY sigla, nome
                      ORDER BY sigla;");
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<ViewFechaModel>(
                    @"SELECT *
                      FROM comercial.proposta_view_fecha
                      WHERE sigla = @sigla
                        AND descricao <> 'Terceirizado'
                      ORDER BY item;",
                    new { sigla });
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
                if (propostaDimensao?.coddimensao == null)
                    return;

                using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    @"UPDATE comercial.proposta_dimensaodescricaocomercial
                      SET cargaeletrica_led = @cargaeletrica_led
                      WHERE coddimensao = @coddimensao;",
                    new
                    {
                        propostaDimensao.cargaeletrica_led,
                        propostaDimensao.coddimensao
                    });

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
