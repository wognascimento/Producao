using Dapper;
using Npgsql;
using Producao.Views.CentralModelos.Compat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewEtiquetaCheckListEmitida.xam
    /// </summary>
    public partial class ViewEtiquetaCheckListEmitida : UserControl
    {
        private bool _dadosCarregados;

        public ViewEtiquetaCheckListEmitida()
        {
            InitializeComponent();
            this.DataContext = new EtiquetaEmitidaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_dadosCarregados)
                return;

            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                EtiquetaEmitidaViewModel vm = (EtiquetaEmitidaViewModel)DataContext;
                await vm.GetEtiquetasAsync();
                _dadosCarregados = true;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private void OnImprimirClick(object sender, RoutedEventArgs e)
        {
            EtiquetaEmitidaViewModel vm = (EtiquetaEmitidaViewModel)DataContext;
            var etiquetasFiltradas = dgEtiquetasEmitidas.Items
                .OfType<EtiquetaEmitidaModel>()
                .ToList();

            ImprimirEtiquetas(etiquetasFiltradas, vm.BaseSettings);
        }

        private enum Etiqueta
        {
            Primeira,
            Segunda,
            Terceira,
            Quarta,
            Quinta,
            Sexta
        }

        public static void ImprimirEtiquetas(System.Collections.Generic.IEnumerable<EtiquetaEmitidaModel>? etiquetas, DataBaseSettings baseSettings)
        {
            var itens = etiquetas?.ToList() ?? [];
            if (itens.Count == 0)
            {
                MessageBox.Show("Não existem etiquetas para imprimir.");
                return;
            }

            try
            {
                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                using IWorkbook workbook = application.Workbooks.Open(baseSettings.ResolveModeloPath("ETIQUETA_MODELO.xlsx"));
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);

                var etiqueta = Etiqueta.Primeira;
                int paginas = (int)Math.Ceiling(decimal.Divide(itens.Count, 6));
                int pagina = 1;

                foreach (var item in itens)
                {
                    switch (etiqueta)
                    {
                        case Etiqueta.Primeira:
                            PreencherEtiqueta(worksheet, "A1", "B2", "C2", "D2", "B5", "A8", "PRIMEIRA", item, baseSettings);
                            etiqueta = Etiqueta.Segunda;
                            workbook.SaveAs(baseSettings.ResolveImpressosPath($"ETIQUETA_{pagina}.xlsx"));
                            break;
                        case Etiqueta.Segunda:
                            PreencherEtiqueta(worksheet, "F1", "G2", "H2", "I2", "G5", "F8", "SEGUNDA", item, baseSettings);
                            etiqueta = Etiqueta.Terceira;
                            workbook.SaveAs(baseSettings.ResolveImpressosPath($"ETIQUETA_{pagina}.xlsx"));
                            break;
                        case Etiqueta.Terceira:
                            PreencherEtiqueta(worksheet, "A13", "B14", "C14", "D14", "B17", "A20", "TERCEIRA", item, baseSettings);
                            etiqueta = Etiqueta.Quarta;
                            workbook.SaveAs(baseSettings.ResolveImpressosPath($"ETIQUETA_{pagina}.xlsx"));
                            break;
                        case Etiqueta.Quarta:
                            PreencherEtiqueta(worksheet, "F13", "G14", "H14", "I14", "G17", "F20", "QUARTA", item, baseSettings);
                            etiqueta = Etiqueta.Quinta;
                            workbook.SaveAs(baseSettings.ResolveImpressosPath($"ETIQUETA_{pagina}.xlsx"));
                            break;
                        case Etiqueta.Quinta:
                            PreencherEtiqueta(worksheet, "A25", "B26", "C26", "D26", "B29", "A32", "QUINTA", item, baseSettings);
                            etiqueta = Etiqueta.Sexta;
                            workbook.SaveAs(baseSettings.ResolveImpressosPath($"ETIQUETA_{pagina}.xlsx"));
                            break;
                        case Etiqueta.Sexta:
                            PreencherEtiqueta(worksheet, "F25", "G26", "H26", "I26", "G29", "F32", "SEXTA", item, baseSettings, useVolumeCheck: true);
                            etiqueta = Etiqueta.Primeira;
                            workbook.SaveAs(baseSettings.ResolveImpressosPath($"ETIQUETA_{pagina}.xlsx"));
                            LimparEtiquetas(worksheet);
                            pagina++;
                            break;
                    }
                }

                for (int i = 1; i <= paginas; i++)
                {
                    string file = baseSettings.ResolveImpressosPath($"ETIQUETA_{i}.xlsx");
                    Process.Start(new ProcessStartInfo(file)
                    {
                        Verb = "Print",
                        UseShellExecute = true,
                    });
                }
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private static void PreencherEtiqueta(
            IWorksheet worksheet,
            string siglaCell,
            string volumeCell,
            string anoCell,
            string codigoCell,
            string localCell,
            string descricaoCell,
            string rangeName,
            EtiquetaEmitidaModel item,
            DataBaseSettings baseSettings,
            bool useVolumeCheck = false)
        {
            worksheet.Range[siglaCell].Text = item.sigla;
            worksheet.Range[volumeCell].Text = useVolumeCheck
                ? item.volumes > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString()
                : item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
            worksheet.Range[anoCell].Text = baseSettings.Database;
            worksheet.Range[codigoCell].Text = item.coddetalhescompl.ToString();
            worksheet.Range[localCell].Text = item.local_shoppings;
            worksheet.Range[descricaoCell].Text = item.descricao_completa?.Replace("ÚNICO", string.Empty);

            worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
            worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
            worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
            worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
            worksheet.Range[rangeName].CellStyle.Font.Color = ExcelKnownColors.Black;
        }

        private static void LimparEtiquetas(IWorksheet worksheet)
        {
            foreach (var rangeName in new[] { "PRIMEIRA", "SEGUNDA", "TERCEIRA", "QUARTA", "QUINTA", "SEXTA" })
            {
                worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                worksheet.Range[rangeName].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                worksheet.Range[rangeName].CellStyle.Font.Color = ExcelKnownColors.None;
            }
        }
    }

    public class EtiquetaEmitidaViewModel : INotifyPropertyChanged
    {
        static EtiquetaEmitidaViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        public DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        private ObservableCollection<EtiquetaEmitidaModel> _etiquetas;
        public ObservableCollection<EtiquetaEmitidaModel> Etiquetas
        {
            get { return _etiquetas; }
            set { _etiquetas = value; RaisePropertyChanged("Etiquetas"); }
        }
        private EtiquetaEmitidaModel _etiqueta;
        public EtiquetaEmitidaModel Etiqueta
        {
            get { return _etiqueta; }
            set { _etiqueta = value; RaisePropertyChanged("Etiqueta"); }
        }

        public EtiquetaEmitidaViewModel() { }

        public async Task GetEtiquetasAsync()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.etiqueta_emitida;
                    """;

                var data = await QueryAsync<EtiquetaEmitidaModel>(sql);
                Etiquetas = new ObservableCollection<EtiquetaEmitidaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}

