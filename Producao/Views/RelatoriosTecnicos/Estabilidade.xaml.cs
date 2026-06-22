using Microsoft.EntityFrameworkCore;
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
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
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

                vm.Detalhes = await Task.Run(() => vm.GetItensAsync(vm.Sigla));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
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
                    await Task.Run(() => vm.SaveEstabilidadeAsync(item));

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;

                vm.Cliente = await Task.Run(() => vm.GetClienteAsync(vm.Sigla));

                var provider = new XlsxFormatProvider();
                Workbook workbook;
                using (var input = File.OpenRead(BaseSettings.ResolveModeloPath("RELATORIO_ESTABILIDADE_MODELO.xlsx")))
                {
                    workbook = provider.Import(input);
                }

                var worksheet = workbook.Worksheets[0];
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
                MessageBox.Show(ex.Message);
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
                using DatabaseContext db = new();
                var query = from p in db.Siglas
                            group p by p.sigla into g
                            orderby g.Key
                            select g.Key;

                return new ObservableCollection<string>(await query.ToListAsync());
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
                using DatabaseContext db = new();
                var query = await db.Clientes.FindAsync(sigla);

                return query;
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
                using DatabaseContext db = new();
                var resultado = from qdQuantitativo in db.PropostaFechaQdQuantitativos
                                join dimensaoDescComercial in db.propostaDimensoes
                                on qdQuantitativo.coddimensao equals dimensaoDescComercial.coddimensao
                                join descComercial in db.PropostaDescricaoComercials
                                on dimensaoDescComercial.coddesccoml equals descComercial.coddesccoml
                                where qdQuantitativo.sigla == sigla
                                orderby descComercial.familia, descComercial.descricaocomercial, dimensaoDescComercial.dimensao
                                select new EstabilidadeItem
                                {
                                    cod_linha_qdfecha = qdQuantitativo.cod_linha_qdfecha,
                                    familia = descComercial.familia,
                                    descricaocomercial = descComercial.descricaocomercial,
                                    coddesccoml = descComercial.coddesccoml,
                                    dimensao = dimensaoDescComercial.dimensao,
                                    coddimensao = dimensaoDescComercial.coddimensao,
                                    relatorio_estabilidade = dimensaoDescComercial.relatorio_estabilidade,
                                    sigla = qdQuantitativo.sigla,
                                    local = qdQuantitativo.local,
                                    detalhe_local = qdQuantitativo.detalhe_local
                                };

                var listaResultado = await resultado.ToListAsync();

                return new ObservableCollection<EstabilidadeItem>(listaResultado);
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
                using DatabaseContext db = new();
                
                var dimensao = await db.propostaDimensoes.FindAsync(estabilidade.coddimensao);

                if (estabilidade.relatorio_estabilidade != "")
                {
                    dimensao.relatorio_estabilidade = estabilidade.relatorio_estabilidade;
                    db.Entry(dimensao).Property(p => p.relatorio_estabilidade).IsModified = true;
                } 
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
