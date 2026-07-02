using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
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
    /// Interação lógica para Estabilidade.xam
    /// </summary>
    public partial class Estabilidade : UserControl
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public Estabilidade()
        {
            InitializeComponent();
            DataContext = new EstabilidadeViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;
                if (sender is RadComboBox combo)
                    vm.Sigla = combo.SelectedItem as string;

                if (vm.Sigla == null)
                {
                    vm.Detalhes = [];
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                vm.Detalhes = await vm.GetItensAsync(vm.Sigla);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void DetalhesCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            try
            {
                if (e.Cell.Column.UniqueName != "relatorio_estabilidade")
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;
                var grid = sender as RadGridView;
                var item = grid?.Items.CurrentEditItem as EstabilidadeItem ?? e.Cell.DataContext as EstabilidadeItem;

                if (item != null)
                    await vm.SaveEstabilidadeAsync(item);

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;

                vm.Cliente = await vm.GetClienteAsync(vm.Sigla);

                var provider = new XlsxFormatProvider();
                Workbook workbook;
                using (var input = File.OpenRead(BaseSettings.ResolveModeloPath("RELATORIO_ESTABILIDADE_MODELO.xlsx")))
                {
                    workbook = provider.Import(input);
                }

                var worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.WorksheetPageSetup.FitToPages = false;
                worksheet.WorksheetPageSetup.CenterHorizontally = true;
                worksheet.WorksheetPageSetup.ScaleFactor = new Size(0.97, 0.97);
                var border = new CellBorder(CellBorderStyle.Thin, ThemableColor.FromColor(System.Windows.Media.Colors.Black));
                var allBorders = new CellBorders(
                    border,
                    border,
                    border,
                    border,
                    border,
                    border,
                    CellBorder.Default,
                    CellBorder.Default);

                SetText(worksheet, 2, 1, vm.Cliente?.nome);
                SetText(worksheet, 2, 6, vm.Cliente?.cidade);
                SetText(worksheet, 2, 9, vm.Cliente?.est);

                var itens = vm.Detalhes.Where(d => d.relatorio_estabilidade?.Length >0).ToList();

                int startRow = 13;
                int numberOfRowsToInsert = 4;

                foreach (var item in itens)
                {
                    InsertRows(worksheet, startRow, numberOfRowsToInsert);

                    MergeAndSetText(worksheet, startRow, 0, startRow + 1, 0, "Produto: ", allBorders, 11);
                    MergeAndSetText(worksheet, startRow, 1, startRow + 1, 10, $"{item.descricaocomercial} {item.dimensao}", allBorders, 10);
                    startRow += 2;

                    MergeAndSetText(worksheet, startRow, 0, startRow, 0, "Local: ", allBorders, 11);
                    MergeAndSetText(worksheet, startRow, 1, startRow, 10, $"{item.local} {item.detalhe_local}", allBorders, 10);
                    startRow++;

                    MergeAndSetText(worksheet, startRow, 0, startRow, 10, item.relatorio_estabilidade, allBorders, 10);
                    worksheet.Rows[startRow].SetHeight(item.relatorio_estabilidade.Length < 136 ? RowHeight.AutoFit : new RowHeight(100, true));
                    startRow++;
                }

                var caminhoArquivo = BaseSettings.ResolveImpressosPath($"Estabilidade-{vm.Sigla}.xlsx");
                using (var output = File.Open(caminhoArquivo, FileMode.Create))
                {
                    provider.Export(workbook, output, TimeSpan.FromSeconds(30));
                }

                Process.Start(new ProcessStartInfo(caminhoArquivo)
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private static void InsertRows(Worksheet worksheet, int rowIndex, int count)
        {
            for (var i = 0; i < count; i++)
                worksheet.Rows[rowIndex].Insert();
        }

        private static void MergeAndSetText(
            Worksheet worksheet,
            int fromRow,
            int fromColumn,
            int toRow,
            int toColumn,
            string? value,
            CellBorders borders,
            double fontSize)
        {
            var range = worksheet.Cells[fromRow, fromColumn, toRow, toColumn];
            range.Merge();
            range.SetValueAsText(value ?? string.Empty);
            range.SetBorders(borders);
            range.SetIsWrapped(true);
            range.SetFontSize(fontSize);
            range.SetHorizontalAlignment(RadHorizontalAlignment.Left);
            range.SetVerticalAlignment(RadVerticalAlignment.Center);
        }

        private static void SetText(Worksheet worksheet, int row, int column, string? value)
        {
            worksheet.Cells[row, column].SetValueAsText(value ?? string.Empty);
        }
    }

    public class EstabilidadeViewModel : INotifyPropertyChanged
    {
        private static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName) { this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); }

        private ObservableCollection<string> _siglas;
        public ObservableCollection<string> Siglas { get { return _siglas; } set { _siglas = value; RaisePropertyChanged("Siglas"); } }

        private string _sigla;
        public string Sigla { get { return _sigla; } set { _sigla = value; RaisePropertyChanged("Sigla"); } }

        private ObservableCollection<EstabilidadeItem> _detalhes;
        public ObservableCollection<EstabilidadeItem> Detalhes { get { return _detalhes; } set { _detalhes = value; RaisePropertyChanged("Detalhes"); } }

        private ClienteModel _cliente;
        public ClienteModel Cliente { get { return _cliente; } set { _cliente = value; RaisePropertyChanged("Cliente"); } }

        public async Task<ObservableCollection<string>> GetSiglasAsync()
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<string>(
                    @"SELECT sigla
                      FROM producao.view_sigla_chkgeral
                      GROUP BY sigla
                      ORDER BY sigla;");
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ClienteModel> GetClienteAsync(string sigla)
        {
            try
            {
                using var conn = CreateConnection();
                return await conn.QueryFirstOrDefaultAsync<ClienteModel>(
                    @"SELECT sigla, nome, endereco, cidade, bairro, est
                      FROM comercial.clientes
                      WHERE sigla = @sigla;",
                    new { sigla });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<EstabilidadeItem>> GetItensAsync(string sigla)
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<EstabilidadeItem>(
                    @"SELECT
                          q.cod_linha_qdfecha,
                          d.familia,
                          d.descricaocomercial,
                          d.coddesccoml,
                          dim.dimensao,
                          dim.coddimensao,
                          dim.relatorio_estabilidade,
                          q.sigla,
                          q.local,
                          q.detalhe_local
                      FROM comercial.tbl_fecha_qd_quantitativo q
                      JOIN comercial.proposta_dimensaodescricaocomercial dim
                        ON q.coddimensao = dim.coddimensao
                      JOIN comercial.proposta_descricaocomercial d
                        ON dim.coddesccoml = d.coddesccoml
                      WHERE q.sigla = @sigla
                      ORDER BY d.familia, d.descricaocomercial, dim.dimensao;",
                    new { sigla });

                return new ObservableCollection<EstabilidadeItem>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveEstabilidadeAsync(EstabilidadeItem estabilidade)
        {
            try
            {
                if (estabilidade.coddimensao == null)
                    return;

                using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    @"UPDATE comercial.proposta_dimensaodescricaocomercial
                      SET relatorio_estabilidade = @relatorio_estabilidade
                      WHERE coddimensao = @coddimensao;",
                    estabilidade);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
