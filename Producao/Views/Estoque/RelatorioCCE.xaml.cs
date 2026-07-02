using ClosedXML.Excel;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Producao.Views.Estoque
{
    /// <summary>
    /// Interação lógica para RelatorioCCE.xam
    /// </summary>
    public partial class RelatorioCCE : UserControl
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public RelatorioCCE()
        {
            InitializeComponent();
            DataContext = new RelatorioCCEViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                RelatorioCCEViewModel vm = (RelatorioCCEViewModel)DataContext;
                vm.Planilhas = await vm.GetPlanilhasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnExportarExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                RelatorioCCEViewModel vm = (RelatorioCCEViewModel)DataContext;
                if (vm.Planilha?.planilha is not string planilha)
                    throw new InvalidOperationException("Selecione uma planilha.");

                vm.Descricoes = await vm.GetDescricoesAsync(planilha);
                var filePath = BaseSettings.ResolveImpressosPath("RELATORIO_CCE.xlsx");
                using var workbook = new XLWorkbook(BaseSettings.ResolveModeloPath("RELATORIO_CCE_MODELO.xlsx"));
                var worksheet = workbook.Worksheet(1);
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);

                worksheet.Cell("H1").Value = vm.Planilha.planilha;

                var row = 5;
                foreach (var item in vm.Descricoes)
                {
                    var codigoRange = worksheet.Range(row, 1, row, 2).Merge();
                    codigoRange.Value = item.codcompladicional;
                    ApplyBodyStyle(codigoRange);
                    codigoRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var descricaoRange = worksheet.Range(row, 3, row, 11).Merge();
                    descricaoRange.Value = item.descricao_completa ?? string.Empty;
                    ApplyBodyStyle(descricaoRange);

                    for (var column = 12; column <= 21; column++)
                    {
                        ApplyBodyStyle(worksheet.Cell(row, column));
                    }

                    row++;
                }

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

        private void RowDefinition_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private static void ApplyBodyStyle(IXLRange range)
        {
            ApplyBodyStyle(range.Style);
        }

        private static void ApplyBodyStyle(IXLCell cell)
        {
            ApplyBodyStyle(cell.Style);
        }

        private static void ApplyBodyStyle(IXLStyle style)
        {
            style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            style.Border.InsideBorder = XLBorderStyleValues.Thin;
            style.Border.OutsideBorderColor = XLColor.FromArgb(191, 191, 191);
            style.Border.InsideBorderColor = XLColor.FromArgb(191, 191, 191);
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            style.Alignment.WrapText = true;
        }
    }

    class RelatorioCCEViewModel : INotifyPropertyChanged
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

        private ObservableCollection<QryDescricao> _descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return _descricoes; }
            set { _descricoes = value; RaisePropertyChanged("Descricoes"); }
        }
        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }

        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            const string sql = "SELECT * FROM producao.relplan WHERE ativo = '1' ORDER BY planilha;";
            await using var connection = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            return new ObservableCollection<RelplanModel>(await connection.QueryAsync<RelplanModel>(sql));
        }

        public async Task<ObservableCollection<QryDescricao>> GetDescricoesAsync(string planilha)
        {
            const string sql = "SELECT * FROM producao.qry3descricoes WHERE inativo = '0' AND planilha = @planilha;";
            await using var connection = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            return new ObservableCollection<QryDescricao>(
                await connection.QueryAsync<QryDescricao>(sql, new { planilha }));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
