using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.Estoque
{
    /// <summary>
    /// Interação lógica para SaldoEstoque.xam
    /// </summary>
    public partial class SaldoEstoque : UserControl
    {
        public SaldoEstoque()
        {
            InitializeComponent();
            DataContext = new SaldoEstoqueViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                SaldoEstoqueViewModel vm = (SaldoEstoqueViewModel)DataContext;
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSaldoDetalhado(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                SaldoEstoqueViewModel vm = (SaldoEstoqueViewModel)DataContext;
                vm.SaldoDetalhados = await Task.Run(() => vm.GetSaldoDetalhadosAsync(vm.Planilha.planilha));
                using (ExcelEngine excelEngine = new())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Create a new workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet sheet = workbook.Worksheets[0];

                    //Import data from the data table with column header, at first row and first column, 
                    //and by its column type.

                    ExcelImportDataOptions importDataOptions = new ExcelImportDataOptions()
                    {
                        FirstRow = 1,
                        FirstColumn = 1,
                        IncludeHeader = true,
                        PreserveTypes = true
                    };
                    sheet.ImportData(vm.SaldoDetalhados, importDataOptions);

                    //Creating Excel table or list object and apply style to the table
                    IListObject table = sheet.ListObjects.Create("Employee_PersonalDetails", sheet.UsedRange);

                    table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium14;

                    //Autofit the columns
                    sheet.UsedRange.AutofitColumns();

                    //Save the file in the given path
                    Stream excelStream = File.Create(System.IO.Path.GetFullPath(@"Output.xlsx"));
                    workbook.SaveAs("Impressos\\SALDO_ESTOQUE_DETALHADO.xlsx");

                    Process.Start(new ProcessStartInfo("Impressos\\SALDO_ESTOQUE_DETALHADO.xlsx")
                    {
                        UseShellExecute = true
                    });

                    excelStream.Dispose();
                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class SaldoEstoqueViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<RelplanModel> _planilhas;
        public ObservableCollection<RelplanModel> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
        }
        private RelplanModel _planilha;
        public RelplanModel Planilha
        {
            get { return _planilha; }
            set { _planilha = value; RaisePropertyChanged("Planilha"); }
        }

        private ObservableCollection<SaldoDetalhadoModel> _saldoDetalhados;
        public ObservableCollection<SaldoDetalhadoModel> SaldoDetalhados
        {
            get { return _saldoDetalhados; }
            set { _saldoDetalhados = value; RaisePropertyChanged("SaldoDetalhados"); }
        }
        private SaldoDetalhadoModel _saldoDetalhado;
        public SaldoDetalhadoModel SaldoDetalhado
        {
            get { return _saldoDetalhado; }
            set { _saldoDetalhado = value; RaisePropertyChanged("SaldoDetalhado"); }
        }

        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                return new ObservableCollection<RelplanModel>(await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1")).ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<ObservableCollection<SaldoDetalhadoModel>> GetSaldoDetalhadosAsync(string? planilha)
        {
            try
            {
                using DatabaseContext db = new();
                return new ObservableCollection<SaldoDetalhadoModel>(await db.SaldoDetalhados.Where(c => c.planilha == planilha).ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
