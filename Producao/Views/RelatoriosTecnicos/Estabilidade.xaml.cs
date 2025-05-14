using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;
                vm.Detalhes = await Task.Run(() => vm.GetItensAsync(vm.Sigla));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void DetalhesRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;
                await Task.Run(() => vm.SaveEstabilidadeAsync((EstabilidadeItem)e.RowData));
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

                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open(@$"{BaseSettings.CaminhoSistema}\Modelos\RELATORIO_ESTABILIDADE_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];

                EstabilidadeViewModel vm = (EstabilidadeViewModel)DataContext;

                vm.Cliente = await Task.Run(() => vm.GetClienteAsync(vm.Sigla));

                worksheet.Range["B3"].Text = vm.Cliente.nome;
                worksheet.Range["G3"].Text = vm.Cliente.cidade;
                worksheet.Range["J3"].Text = vm.Cliente.est;

                var itens = vm.Detalhes.Where(d => d.relatorio_estabilidade?.Length >0).ToList();

                int startRow = 14;
                int numberOfRowsToInsert = 4;

                foreach (var item in itens)
                {
                    worksheet.InsertRow(startRow, numberOfRowsToInsert);

                    worksheet.Range[$"A{startRow}:A{startRow+1}"].Merge();
                    worksheet.Range[$"A{startRow}"].Text = "Produto: ";
                    worksheet.Range[$"A{startRow}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"A{startRow}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    worksheet.Range[$"A{startRow}"].WrapText = true;

                    worksheet.Range[$"A{startRow}:A{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:A{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:A{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:A{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                    worksheet.Range[$"B{startRow}:K{startRow+1}"].Merge();
                    worksheet.Range[$"B{startRow}"].Text = $"{item.descricaocomercial} {item.dimensao}";
                    worksheet.Range[$"B{startRow}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"B{startRow}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    worksheet.Range[$"B{startRow}"].CellStyle.Font.Size = 10;
                    worksheet.Range[$"B{startRow}"].WrapText = true;

                    worksheet.Range[$"B{startRow}:K{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"B{startRow}:K{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"B{startRow}:K{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"B{startRow}:K{startRow+1}"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                    startRow += 2;

                    worksheet.Range[$"A{startRow}:A{startRow}"].Merge();
                    worksheet.Range[$"A{startRow}"].Text = "Local: ";
                    worksheet.Range[$"A{startRow}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"A{startRow}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    worksheet.Range[$"A{startRow}"].WrapText = true;

                    worksheet.Range[$"A{startRow}:A{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:A{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:A{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:A{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                    worksheet.Range[$"B{startRow}:K{startRow}"].Merge();
                    worksheet.Range[$"B{startRow}"].Text = $"{item.local} {item.detalhe_local}";
                    worksheet.Range[$"B{startRow}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"B{startRow}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    worksheet.Range[$"B{startRow}"].CellStyle.Font.Size = 10;
                    worksheet.Range[$"B{startRow}"].WrapText = true;

                    worksheet.Range[$"B{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"B{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"B{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"B{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                    startRow++;

                    worksheet.Range[$"A{startRow}:K{startRow}"].Merge();
                    worksheet.Range[$"A{startRow}"].Text = $"{item.relatorio_estabilidade}";
                    worksheet.Range[$"A{startRow}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"A{startRow}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    worksheet.Range[$"A{startRow}"].CellStyle.Font.Size = 10;
                    worksheet.Range[$"A{startRow}"].RowHeight = 100;
                    if (item.relatorio_estabilidade.Length < 136)
                        worksheet.Range[$"A{startRow}"].AutofitRows();
                    worksheet.Range[$"A{startRow}"].WrapText = true;

                    worksheet.Range[$"A{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    worksheet.Range[$"A{startRow}:K{startRow}"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                    startRow++;

                    //break;
                }

                workbook.SaveAs(@$"{BaseSettings.CaminhoSistema}\Estabilidade-{vm.Sigla}.xlsx");

                Process.Start(new ProcessStartInfo(@$"{BaseSettings.CaminhoSistema}\Estabilidade-{vm.Sigla}.xlsx")
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
