using ClosedXML.Excel;
using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

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
                vm.Planilhas = await vm.GetPlanilhasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSaldoDetalhado(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                SaldoEstoqueViewModel vm = (SaldoEstoqueViewModel)DataContext;
                if (vm.Planilha?.planilha is not string planilha)
                    throw new InvalidOperationException("Selecione uma planilha.");

                vm.SaldoDetalhados = await vm.GetSaldoDetalhadosAsync(planilha);
                var filePath = BaseSettings.ResolveImpressosPath("SALDO_ESTOQUE_DETALHADO.xlsx");
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Saldo");
            Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.Cell(1, 1).InsertTable(vm.SaldoDetalhados, "SaldoEstoqueDetalhado", true);
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);

                Process.Start(new ProcessStartInfo(filePath)
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
            const string sql = "SELECT * FROM producao.relplan WHERE ativo = '1' ORDER BY planilha;";
            await using var connection = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            return new ObservableCollection<RelplanModel>(await connection.QueryAsync<RelplanModel>(sql));
        }
        
        public async Task<ObservableCollection<SaldoDetalhadoModel>> GetSaldoDetalhadosAsync(string? planilha)
        {
            const string sql = "SELECT * FROM producao.qry_saldo_detalhado_c WHERE planilha = @planilha;";
            await using var connection = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            return new ObservableCollection<SaldoDetalhadoModel>(
                await connection.QueryAsync<SaldoDetalhadoModel>(sql, new { planilha }));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
