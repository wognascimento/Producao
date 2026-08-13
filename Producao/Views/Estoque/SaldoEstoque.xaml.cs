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
using Telerik.Windows.Controls;

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
                if (sender is RadButton button)
                    button.IsEnabled = false;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                SaldoEstoqueViewModel vm = (SaldoEstoqueViewModel)DataContext;
                if (vm.Planilha?.planilha is not string planilha)
                    throw new InvalidOperationException("Selecione uma planilha.");

                vm.SaldoDetalhados = await vm.GetSaldoDetalhadosAsync(planilha);
                var filePath = BaseSettings.ResolveImpressosPath("SALDO_ESTOQUE_DETALHADO.xlsx");
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Saldo");
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                Producao.Utils.ExcelExportHelper.InsertTypedTable(worksheet, vm.SaldoDetalhados, "SaldoEstoqueDetalhado");
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);

                Process.Start(new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (TimeoutException ex)
            {
                Producao.ErrorDialog.Show(new TimeoutException("A consulta do saldo de estoque demorou mais que o limite configurado. Tente novamente ou solicite a otimizacao da view producao.qry_saldo_detalhado_c para esta planilha.", ex), "Tempo esgotado");
            }
            catch (NpgsqlException ex) when (ex.InnerException is TimeoutException timeout)
            {
                Producao.ErrorDialog.Show(new TimeoutException("A consulta do saldo de estoque demorou mais que o limite configurado. Tente novamente ou solicite a otimizacao da view producao.qry_saldo_detalhado_c para esta planilha.", timeout), "Tempo esgotado");
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                if (sender is RadButton button)
                    button.IsEnabled = true;
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
            const int commandTimeoutSeconds = 300;
            const string sql = """
                SELECT
                    planilha,
                    descricao,
                    descricao_adicional,
                    complementoadicional,
                    codcompladicional,
                    vida_util,
                    custo,
                    chks,
                    saldo_patrimonial,
                    unidade,
                    saldo_patrimonial_ano_anterior,
                    saldo_disponivel_ano_anterior,
                    estoque_inicial_nao_processado,
                    estoque_inicial_processado,
                    cce,
                    oss_peca_nova,
                    oss_recuperacao,
                    movimentacao_entrada_processada,
                    movimentacao_de_entrada_nao_processada,
                    total_entradas,
                    requisicao_geral,
                    movimentacao_saída,
                    requisicoes_internas,
                    movimentacao_de_saidas_gerais,
                    movimentacao_de_saidas_processadas,
                    descartes_gerais,
                    total_de_saidas,
                    total_produzido,
                    saldo_de_estoque_produzido,
                    saldo_disponível,
                    inventariado,
                    inativo
                FROM producao.qry_saldo_detalhado_c
                WHERE planilha = @planilha;
                """;
            await using var connection = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            return new ObservableCollection<SaldoDetalhadoModel>(
                await connection.QueryAsync<SaldoDetalhadoModel>(sql, new { planilha }, commandTimeout: commandTimeoutSeconds));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
