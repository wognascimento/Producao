using Dapper;
using Npgsql;
using Producao.Views.CentralModelos.Compat;
using Producao.Views.OrdemServico;
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
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Interação lógica para ViewCentralEmitirOs.xam
    /// </summary>
    public partial class ViewCentralEmitirOs : UserControl
    {


        public ViewCentralEmitirOs()
        {
            InitializeComponent();
            this.DataContext = new ViewCentralEmitirOsViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralEmitirOsViewModel vm = (ViewCentralEmitirOsViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void dgTabela_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ViewCentralEmitirOsViewModel vm = (ViewCentralEmitirOsViewModel)DataContext;
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    vm.Itens = await vm.GetItensAsync();
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private void UserControl_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {

        }
    }

    public class ViewCentralEmitirOsViewModel : INotifyPropertyChanged
    {
        static ViewCentralEmitirOsViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        private static async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private ModeloGerarOsModel item;
        public ModeloGerarOsModel Item
        {
            get { return item; }
            set { item = value; RaisePropertyChanged("Item"); }
        }

        private ObservableCollection<ModeloGerarOsModel> itens;
        public ObservableCollection<ModeloGerarOsModel> Itens
        {
            get { return itens; }
            set { itens = value; RaisePropertyChanged("Itens"); }
        }

        private ObservableCollection<DistribuicaoPAModel> distribuicao;
        public ObservableCollection<DistribuicaoPAModel> Distribuicao
        {
            get { return distribuicao; }
            set { distribuicao = value; RaisePropertyChanged("Distribuicao"); }
        }

        private ObservableCollection<ReqDetalhesModel> _reqDetalhes;
        public ObservableCollection<ReqDetalhesModel> ReqDetalhes
        {
            get { return _reqDetalhes; }
            set { _reqDetalhes = value; RaisePropertyChanged("ReqDetalhes"); }
        }

        private ObservableCollection<DetalhesModeloFitasModel> _controleDetalhes;
        public ObservableCollection<DetalhesModeloFitasModel> ControleDetalhes
        {
            get { return _controleDetalhes; }
            set { _controleDetalhes = value; RaisePropertyChanged("ControleDetalhes"); }
        }

        private ModeloTabelaConversaoModel conversao;
        public ModeloTabelaConversaoModel Conversao
        {
            get { return conversao; }
            set { conversao = value; RaisePropertyChanged("Conversao"); }
        }

        public async Task<ObservableCollection<ModeloGerarOsModel>> GetItensAsync()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qry_modelos_gerar_os
                    WHERE planilha = ANY(@planilhas);
                    """;
                var data = await QueryAsync<ModeloGerarOsModel>(sql, new
                {
                    planilhas = new[]
                    {
                        "ADEREÇO", "CACHO", "DECESP", "ENF AÉREO", "ENF PISO", "FESTÃO DECOR",
                        "FIADA", "GUIR P", "KIT ENF ARV P", "KIT ENF CACHO", "KIT ENF FESTÃO",
                        "KIT ENF GUIR G", "KIT ENF GUIR P", "KIT ENF PA", "KIT ENF PÓRT",
                        "KIT ENF TOP", "KIT ENF WALL TREE", "KIT FITAS", "MÓVEIS", "PONTEIRA",
                        "TOPIÁRIA", "VASO", "WALL TREE", "FUNDO DE CENA", "MARC L", "PÓRTICO",
                        "TRENÓ", "TRILHA"
                    }
                });
                return new ObservableCollection<ModeloGerarOsModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ModeloTabelaConversaoModel> GetConversaoAsync(long? codcompleadicional)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.tbl_conversao
                    WHERE codcompleadicional = @codcompleadicional
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<ModeloTabelaConversaoModel>(sql, new { codcompleadicional });
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<DistribuicaoPAModel>> GetDistribuicoesAsync(long? id_modelo, string? cliente)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qry_detalhes_modelo_distribuicao_pa_excel
                    WHERE id_modelo = @id_modelo
                      AND cliente = @cliente;
                    """;
                var data = await QueryAsync<DistribuicaoPAModel>(sql, new { id_modelo, cliente });
                return new ObservableCollection<DistribuicaoPAModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OsEmissaoProducaoImprimirModel>> GetOsEmitidas(long? num_os_produto)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.qry_os_emissao_producao_imprimir
                    WHERE num_os_produto = @num_os_produto;
                    """;
                var data = await QueryAsync<OsEmissaoProducaoImprimirModel>(sql, new { num_os_produto });
                return new ObservableCollection<OsEmissaoProducaoImprimirModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProdutoServicoModel>> GetServicos(long? num_os_produto)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.tbl_produtos_servico
                    WHERE num_os_produto = @num_os_produto;
                    """;
                var data = await QueryAsync<ProdutoServicoModel>(sql, new { num_os_produto });
                return new ObservableCollection<ProdutoServicoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<long?> GetUltimaOrdemServicoProdutoAsync(long? codcompladicional)
        {
            try
            {
                const string sql = """
                    SELECT num_os_produto
                    FROM producao.tbl_produto_os
                    WHERE cod_compl_adicional = @codcompladicional
                    ORDER BY num_os_produto DESC
                    LIMIT 1;
                    """;
                return await QueryFirstOrDefaultAsync<long?>(sql, new { codcompladicional });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QryModeloModel> GetModeloAsync(long? idModelo)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qrymodelos
                    WHERE id_modelo = @idModelo
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<QryModeloModel>(sql, new { idModelo });
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProdutoServicoModel> GetUltimaOSModeloClienteAsync(long? idModelo, string Sigla)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.tbl_produtos_servico
                    WHERE id_modelo = @idModelo
                      AND cliente = @Sigla
                      AND COALESCE(cancelada_os, '') <> '-1'
                    ORDER BY num_os_servico DESC
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<ProdutoServicoModel>(sql, new { idModelo, Sigla });
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ReqDetalhesModel>> GetRequisicaoDetalServicohesAsync(long? num_os_produto)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qry_req_detalhes
                    WHERE num_os_produto = @num_os_produto
                      AND quantidade > 0;
                    """;
                var data = await QueryAsync<ReqDetalhesModel>(sql, new { num_os_produto });
                return new ObservableCollection<ReqDetalhesModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        public async Task<ObservableCollection<DetalhesModeloFitasModel>> GetItensControleOldAsync(long? idModelo)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qrydetalhesmodelo_fitas
                    WHERE id_modelo = @idModelo;
                    """;
                var data = await QueryAsync<DetalhesModeloFitasModel>(sql, new { idModelo });
                return new ObservableCollection<DetalhesModeloFitasModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<ObservableCollection<ReqDetalhesModel>> GetItensControleAsync(long? idModelo)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qry_req_detalhes
                    WHERE id_modelo = @idModelo
                      AND quantidade > 0;
                    """;
                var data = await QueryAsync<ReqDetalhesModel>(sql, new { idModelo });
                return new ObservableCollection<ReqDetalhesModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public static class ContextMenuCommands
    {

        static DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        static ICommand? createOS;
        public static ICommand CreateOS
        {
            get
            {
                if (createOS == null)
                    createOS = new RelayCommand(OnCreateOSClicked);
                return createOS;
            }
        }
        private static async void OnCreateOSClicked(object obj)
        {

            //var Record = { Producao.ModeloControleOsModel}
            var grid = obj as RadGridView;
            if (grid is null)
                return;

            var record = grid.SelectedItem as ModeloGerarOsModel;
            var item = grid.SelectedItem as ModeloGerarOsModel;
            ViewCentralEmitirOsViewModel vm = (ViewCentralEmitirOsViewModel)grid.DataContext;

            if(record.id_modelo == null)
            {
                MessageBox.Show("Não existe modelo associado a esta linha.");
                return;
            }

            if (record?.qtde > (int)(record?.qtde_os ?? 0))
            {
                try
                {
                    var conversao = await vm.GetConversaoAsync(record.codcompladicional);
                    if (conversao == null)
                    {
                        MessageBox.Show("Não existe fator de calculo para este produto.");
                        return;
                    }
                  

                    var dif = (record?.qtde - (int)(record?.qtde_os ?? 0));
                    //var dif = record?.qtd_chk_list;
                    var window = new ModeloSetoresOrdemServico(record);
                    window.Owner = App.Current.MainWindow;
                    window.ShowDialog();
                    
                    /*Window window = new Window();
                    window.Content = new ModeloSetoresOrdemServico(747); //item?.codcompladicional
                    window.Owner = App.Current.MainWindow;
                    window.Title = "SETORES PARA EMISSÃO DA OERDEM DE SERVIÇO";
                    window.WindowStyle = WindowStyle.ToolWindow; //"ToolWindow"
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner; //"CenterOwner" 
                    window.ResizeMode = ResizeMode.NoResize; //"NoResize"
                    window.Height = 450;
                    window.Width = 500;
                    window.ShowDialog();*/


                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                }
               
            }
            else
            {
                MessageBox.Show("Quantidade indisponivel para Gerar ordem de serviço.");
            }
        }


        static ICommand? reimprimirOS;
        public static ICommand ReimprimirOS
        {
            get
            {
                if (reimprimirOS == null)
                    reimprimirOS = new RelayCommand(OnReimprimirOSClicked);
                return reimprimirOS;
            }
        }
        private async static void OnReimprimirOSClicked(object obj)
        {
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

            var grid = obj as RadGridView;
            if (grid is null)
                return;

            var item = grid.SelectedItem as ModeloGerarOsModel;
            try
            {
                ViewCentralEmitirOsViewModel vm = (ViewCentralEmitirOsViewModel)grid.DataContext;
                var numOsProduto = await vm.GetUltimaOrdemServicoProdutoAsync(item.codcompladicional);
                if (!numOsProduto.HasValue)
                {
                    MessageBox.Show("Nao foi encontrada O.S emitida para este modelo.", "Reimprimir O.S");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                var servicos = await vm.GetOsEmitidas(numOsProduto);
                var printer = new OrdemServicoModeloPrinter();
                await printer.ImprimirAsync(servicos, async numOsProduto => await vm.GetServicos(numOsProduto));

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }


        static ICommand? tabelaPAExcel;
        public static ICommand TabelaPAExcel
        {
            get
            {
                if (tabelaPAExcel == null)
                    tabelaPAExcel = new RelayCommand(OnTabelaPAExcelClicked);
                return tabelaPAExcel;
            }
        }
        private async static void OnTabelaPAExcelClicked(object obj)
        {
            var grid = obj as RadGridView;
            if (grid is null)
                return;

            var item = grid.SelectedItem as ModeloGerarOsModel;
            if (item?.planilha != "KIT ENF PA")
            {
                MessageBox.Show("Produto não é uma P.A");
                return;
            }


            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                ViewCentralEmitirOsViewModel vm = (ViewCentralEmitirOsViewModel)grid.DataContext;
                DataBaseSettings BaseSettings = DataBaseSettings.Instance;
                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
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
                //bodyStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                //bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                //bodyStyle.Font.Bold = true;
                //bodyStyle.WrapText = true;
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
                headerStyle.Font.Bold = true;
                //headerStyle.WrapText = true;
                headerStyle.EndUpdate();

                worksheet.Range["A1"].Text = $"{item.descricao_completa}";
                worksheet.Range["A1:B1"].Merge();
                worksheet.Range["A1:B1"].CellStyle = headerStyle;

                worksheet.Range["A2"].Text = $"TEMA: {item.tema}";
                worksheet.Range["A2:B2"].Merge();
                worksheet.Range["A2:B2"].CellStyle = headerStyle;

                worksheet.Range["A3"].Text = $"CLIENTE: {item.sigla}";
                worksheet.Range["A3:B3"].Merge();
                worksheet.Range["A3:B3"].CellStyle = headerStyle;

                worksheet.Range["A4"].Text = $"COD.";
                worksheet.Range["A4"].CellStyle = headerStyle;
                worksheet.Range["B4"].Text = $"DESCRIÇÃO";
                worksheet.Range["B4"].CellStyle = headerStyle;
                worksheet.Range["C4"].Text = $"QUANT.";
                worksheet.Range["C4"].CellStyle = headerStyle;
                worksheet.Range["D4"].Text = $"PONGA";
                worksheet.Range["D4"].CellStyle = headerStyle;
                worksheet.Range["E4"].Text = $"TRIPÉ";
                worksheet.Range["E4"].CellStyle = headerStyle;
                worksheet.Range["F4"].Text = $"1º ANEL";
                worksheet.Range["F4"].CellStyle = headerStyle;
                worksheet.Range["G4"].Text = $"2º ANEL";
                worksheet.Range["G4"].CellStyle = headerStyle;
                worksheet.Range["H4"].Text = $"3º ANEL";
                worksheet.Range["H4"].CellStyle = headerStyle;
                worksheet.Range["I4"].Text = $"4º ANEL";
                worksheet.Range["I4"].CellStyle = headerStyle;
                worksheet.Range["J4"].Text = $"5º ANEL";
                worksheet.Range["J4"].CellStyle = headerStyle;
                worksheet.Range["K4"].Text = $"6º ANEL";
                worksheet.Range["K4"].CellStyle = headerStyle;
                worksheet.Range["L4"].Text = $"7º ANEL";
                worksheet.Range["L4"].CellStyle = headerStyle;
                worksheet.Range["M4"].Text = $"8º ANEL";
                worksheet.Range["M4"].CellStyle = headerStyle;
                worksheet.Range["N4"].Text = $"9º ANEL";
                worksheet.Range["N4"].CellStyle = headerStyle;
                worksheet.Range["O4"].Text = $"10º ANEL";
                worksheet.Range["O4"].CellStyle = headerStyle;
                worksheet.Range["P4"].Text = $"11º ANEL";
                worksheet.Range["P4"].CellStyle = headerStyle;
                worksheet.Range["Q4"].Text = $"12º ANEL";
                worksheet.Range["Q4"].CellStyle = headerStyle;
                worksheet.Range["R4"].Text = $"13º ANEL";
                worksheet.Range["R4"].CellStyle = headerStyle;
                worksheet.Range["S4"].Text = $"14º ANEL";
                worksheet.Range["S4"].CellStyle = headerStyle;
                worksheet.Range["T4"].Text = $"15º ANEL";
                worksheet.Range["T4"].CellStyle = headerStyle;
                worksheet.Range["U4"].Text = $"16º ANEL";
                worksheet.Range["U4"].CellStyle = headerStyle;
                worksheet.Range["V4"].Text = $"17º ANEL";
                worksheet.Range["V4"].CellStyle = headerStyle;
                worksheet.Range["W4"].Text = $"18º ANEL";
                worksheet.Range["W4"].CellStyle = headerStyle;
                worksheet.Range["X4"].Text = $"19º ANEL";
                worksheet.Range["X4"].CellStyle = headerStyle;
                worksheet.Range["Y4"].Text = $"20º ANEL";
                worksheet.Range["Y4"].CellStyle = headerStyle;
                worksheet.Range["Z4"].Text = $"21º ANEL";
                worksheet.Range["Z4"].CellStyle = headerStyle;
                worksheet.Range["AA4"].Text = $"22º ANEL";
                worksheet.Range["AA4"].CellStyle = headerStyle;


                //UsedRange excludes the blank cells
                worksheet.UsedRangeIncludesFormatting = false;

                //Adding border to highlight the used range
                worksheet.UsedRange.BorderAround();

                vm.Distribuicao = await vm.GetDistribuicoesAsync(item.id_modelo, item.sigla);
                int linha = 5;
                foreach (DistribuicaoPAModel dist in vm.Distribuicao)
                {
                    worksheet.Range[$"A{linha}"].Number = dist.codcompladicional == null ? 0 : (double)dist.codcompladicional;
                    worksheet.Range[$"A{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"B{linha}"].Text = dist.descricao_produto;
                    worksheet.Range[$"B{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"C{linha}"].Number = (dist?.qtd) == null ? 0 : (double)dist.qtd;
                    worksheet.Range[$"C{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"D{linha}"].Number = dist?.p == null ? 0 : (double)dist.p;
                    worksheet.Range[$"D{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"E{linha}"].Number = dist?.t == null ? 0 : (double)dist.t;
                    worksheet.Range[$"E{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"F{linha}"].Number = dist?.anel1 == null ? 0 : (double)dist.anel1;
                    worksheet.Range[$"F{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"G{linha}"].Number = dist?.anel2 == null ? 0 : (double)dist.anel2;
                    worksheet.Range[$"G{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"H{linha}"].Number = dist?.anel3 == null ? 0 : (double)dist.anel3;
                    worksheet.Range[$"H{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"I{linha}"].Number = dist?.anel4 == null ? 0 : (double)dist.anel4;
                    worksheet.Range[$"I{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"J{linha}"].Number = dist?.anel5 == null ? 0 : (double)dist.anel5;
                    worksheet.Range[$"J{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"K{linha}"].Number = dist?.anel6 == null ? 0 : (double)dist.anel6;
                    worksheet.Range[$"K{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"L{linha}"].Number = dist?.anel7 == null ? 0 : (double)dist.anel7;
                    worksheet.Range[$"L{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"M{linha}"].Number = dist?.anel8 == null ? 0 : (double)dist.anel8;
                    worksheet.Range[$"M{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"N{linha}"].Number = dist?.anel9 == null ? 0 : (double)dist.anel9;
                    worksheet.Range[$"N{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"O{linha}"].Number = dist?.anel10 == null ? 0 : (double)dist.anel10;
                    worksheet.Range[$"O{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"P{linha}"].Number = dist?.anel11 == null ? 0 : (double)dist.anel11;
                    worksheet.Range[$"P{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"Q{linha}"].Number = dist?.anel12 == null ? 0 : (double)dist.anel12;
                    worksheet.Range[$"Q{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"R{linha}"].Number = dist?.anel13 == null ? 0 : (double)dist.anel13;
                    worksheet.Range[$"R{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"S{linha}"].Number = dist?.anel14 == null ? 0 : (double)dist.anel14;
                    worksheet.Range[$"S{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"T{linha}"].Number = dist?.anel15 == null ? 0 : (double)dist.anel15;
                    worksheet.Range[$"T{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"U{linha}"].Number = dist?.anel16 == null ? 0 : (double)dist.anel16;
                    worksheet.Range[$"U{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"V{linha}"].Number = dist?.anel17 == null ? 0 : (double)dist.anel17;
                    worksheet.Range[$"V{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"W{linha}"].Number = dist?.anel18 == null ? 0 : (double)dist.anel18;
                    worksheet.Range[$"W{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"X{linha}"].Number = dist?.anel19 == null ? 0 : (double)dist.anel19;
                    worksheet.Range[$"X{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"Y{linha}"].Number = dist?.anel20 == null ? 0 : (double)dist.anel20;
                    worksheet.Range[$"Y{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"Z{linha}"].Number = dist?.anel21 == null ? 0 : (double)dist.anel21;
                    worksheet.Range[$"Z{linha}"].CellStyle = bodyStyle;
                    worksheet.Range[$"AA{linha}"].Number = dist?.anel22 == null ? 0 : (double)dist.anel22;
                    worksheet.Range[$"AA{linha}"].CellStyle = bodyStyle;
                    linha++;
                }

                worksheet.Range["C1"].Text = $"TABELA DE DISTRIBUIÇÃO DE ENFEITES POR ANEL";
                worksheet.Range["$C$1:$AA$3"].Merge();
                worksheet.Range["$C$1:$AA$3"].CellStyle = headerStyle;
                worksheet.Range["$C$1:$AA$3"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["$C$1:$AA$3"].VerticalAlignment = ExcelVAlign.VAlignCenter;



                worksheet.Range[$"A{linha}:B{linha}"].Merge();
                worksheet.Range[$"A{linha}:B{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"A{linha}"].Text = $"TOTAL DE CAIXAS";
                worksheet.Range[$"A{linha}"].HorizontalAlignment = ExcelHAlign.HAlignRight;
                worksheet.Range[$"A{linha}"].VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"B{linha}"].Text = $"";
                worksheet.Range[$"B{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"C{linha}"].Text = $"";
                worksheet.Range[$"C{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"D{linha}"].Text = $"";
                worksheet.Range[$"D{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"E{linha}"].Text = $"";
                worksheet.Range[$"E{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"F{linha}"].Text = $"";
                worksheet.Range[$"F{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"G{linha}"].Text = $"";
                worksheet.Range[$"G{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"H{linha}"].Text = $"";
                worksheet.Range[$"H{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"I{linha}"].Text = $"";
                worksheet.Range[$"I{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"J{linha}"].Text = $"";
                worksheet.Range[$"J{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"K{linha}"].Text = $"";
                worksheet.Range[$"K{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"L{linha}"].Text = $"";
                worksheet.Range[$"L{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"M{linha}"].Text = $"";
                worksheet.Range[$"M{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"N{linha}"].Text = $"";
                worksheet.Range[$"N{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"O{linha}"].Text = $"";
                worksheet.Range[$"O{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"P{linha}"].Text = $"";
                worksheet.Range[$"P{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"Q{linha}"].Text = $"";
                worksheet.Range[$"Q{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"R{linha}"].Text = $"";
                worksheet.Range[$"R{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"S{linha}"].Text = $"";
                worksheet.Range[$"S{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"T{linha}"].Text = $"";
                worksheet.Range[$"T{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"U{linha}"].Text = $"";
                worksheet.Range[$"U{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"V{linha}"].Text = $"";
                worksheet.Range[$"V{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"W{linha}"].Text = $"";
                worksheet.Range[$"W{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"X{linha}"].Text = $"";
                worksheet.Range[$"X{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"Y{linha}"].Text = $"";
                worksheet.Range[$"Y{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"Z{linha}"].Text = $"";
                worksheet.Range[$"Z{linha}"].CellStyle = headerStyle;
                worksheet.Range[$"AA{linha}"].Text = $"";
                worksheet.Range[$"AA{linha}"].CellStyle = headerStyle;




                /*
                worksheet.Range["A2"].Text = $"ITEM";
                worksheet.Range["A2"].ColumnWidth = 5;

                worksheet.Range["B2"].Text = $"LOCAL";
                worksheet.Range["B2"].ColumnWidth = 20;

                worksheet.Range["C2"].Text = $"FAMÍLIA DE PRODUTO PLANILHA";
                worksheet.Range["C2"].ColumnWidth = 20;
                worksheet.Range["C2"].WrapText = true;

                worksheet.Range["D2"].Text = $"DESCRIÇÃO";
                worksheet.Range["D2"].ColumnWidth = 45;
                worksheet.Range["D2"].WrapText = true;

                worksheet.Range["E2"].Text = $"UNID";
                worksheet.Range["E2"].ColumnWidth = 5;

                worksheet.Range["F2"].Text = $"QTDE";
                worksheet.Range["F2"].ColumnWidth = 5;

                worksheet.Range["G2"].Text = $"ORIENTAÇÃO DE MONTAGEM";
                worksheet.Range["G2"].ColumnWidth = 30;

                worksheet.Range["H2"].Text = $"COD DETALHES COMPL";
                worksheet.Range["H2"].ColumnWidth = 10;
                worksheet.Range["H2"].WrapText = true;

                worksheet.Rows[1].CellStyle = bodyStyle;

                await vm.GetChkGeralRelatorioAsync();
                worksheet.ImportData(vm.ChkGeralRelatorios, 3, 1, false);

                worksheet.Range[$"A3:H{vm.ChkGeralRelatorios.Count + 2}"].CellStyle = headerStyle;

                worksheet.Range[$"A3:A{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"A3:A{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"B3:B{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"C3:C{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"D3:D{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"E3:E{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"E3:E{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"F3:G{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"F3:G{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"G3:G{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"H3:H{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"H3:H{vm.ChkGeralRelatorios.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                */

                //worksheet.PageSetup.PrintTitleColumns = "$A:$H";
                //worksheet.PageSetup.PrintTitleRows = "$1:$2";
                worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                worksheet.PageSetup.LeftMargin = Producao.Utils.PrintPageSetupHelper.LeftMargin;
                worksheet.PageSetup.RightMargin = Producao.Utils.PrintPageSetupHelper.RightMargin;
                worksheet.PageSetup.TopMargin = Producao.Utils.PrintPageSetupHelper.TopMargin;
                worksheet.PageSetup.BottomMargin = Producao.Utils.PrintPageSetupHelper.BottomMargin;
                worksheet.PageSetup.RightFooter = "&P";
                worksheet.PageSetup.LeftFooter = "&D";
                worksheet.PageSetup.CenterVertically = true;
                worksheet.PageSetup.CenterHorizontally = true;

                worksheet.UsedRange.AutofitColumns();


                workbook.SaveAs(BaseSettings.ResolveImpressosPath($"TABELA_PA.xlsx"));

                Process.Start(new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"TABELA_PA.xlsx"))
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

        static ICommand? receitaControle;
        public static ICommand ReceitaControle
        {
            get
            {
                if (receitaControle == null)
                    receitaControle = new RelayCommand(OnReceitaControleClicked);
                return receitaControle;
            }
        }

        private async static void OnReceitaControleClicked(object obj)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var grid = obj as RadGridView;
                if (grid is null)
                    return;

                var dados = grid.SelectedItem as ModeloGerarOsModel;
                ViewCentralEmitirOsViewModel vm = (ViewCentralEmitirOsViewModel)grid.DataContext;

                QryModeloModel Modelo = await vm.GetModeloAsync(dados.id_modelo);
                var ultimaOS = await vm.GetUltimaOSModeloClienteAsync(dados.id_modelo, dados.sigla);
                vm.ControleDetalhes = await vm.GetItensControleOldAsync(Modelo.id_modelo);

                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("RECEITA_CENTRAL_MODELO.xlsx"));
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.Range["A1"].Text = "CENTRAL DE MODELOS - CONTROLE";
                worksheet.Range["C2"].Number = Convert.ToDouble( Modelo.id_modelo );
                worksheet.Range["C3"].Text = Modelo.planilha;
                worksheet.Range["C4"].Text = Modelo.descricao_completa;
                worksheet.Range["C5"].Text = Modelo.tema;
                worksheet.Range["A7"].Text = Modelo.obs_modelo;
                worksheet.Range["H4"].Number = Convert.ToDouble( Modelo.multiplica );

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
                bodyStyle.Font.Size = 7;
                //bodyStyle.Font.FontName = "Arial";
                bodyStyle.EndUpdate();

                var index = 9;

                foreach (var item in vm.ControleDetalhes)
                {
                    worksheet.Range[$"A{index}"].Number = Convert.ToDouble(item.codcompladicional);
                    worksheet.Range[$"A{index}"].CellStyle = bodyStyle;
                    //worksheet.Range[$"A{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"A{index}"].CellStyle.Font.Size = 8;

                    worksheet.Range[$"B{index}"].Text = item.planilha;
                    worksheet.Range[$"B{index}"].CellStyle = bodyStyle;
                    //worksheet.Range[$"B{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"B{index}"].CellStyle.Font.Size = 8;

                    worksheet.Range[$"C{index}"].Text = item.descricao_completa;
                    worksheet.Range[$"C{index}:E{index}"].CellStyle = bodyStyle;
                    //worksheet.Range[$"C{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"C{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"C{index}:E{index}"].Merge();
                    worksheet.Range[$"C{index}:E{index}"].WrapText = true;

                    worksheet.Range[$"F{index}"].Text = item.observacao;
                    worksheet.Range[$"F{index}"].CellStyle = bodyStyle;
                    //worksheet.Range[$"F{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"F{index}"].CellStyle.Font.Size = 8;
                    //worksheet.Range[$"F{index}:G{index}"].Merge();
                    worksheet.Range[$"F{index}"].WrapText = true;

                    worksheet.Range[$"G{index}"].Number = 0;
                    worksheet.Range[$"G{index}"].CellStyle = bodyStyle;
                    //worksheet.Range[$"G{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"G{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"G{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    worksheet.Range[$"H{index}"].Number = Math.Ceiling(Convert.ToDouble(item.qtd * dados.qtde_os));
                    worksheet.Range[$"H{index}"].CellStyle = bodyStyle;
                    //worksheet.Range[$"H{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"H{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"H{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    index++;
                }

                workbook.SaveAs(BaseSettings.ResolveImpressosPath($"RECEITA_CENTRAL_MODELO_{Modelo.id_modelo}.xlsx"));
                workbook.Close();

                Process.Start(new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"RECEITA_CENTRAL_MODELO_{Modelo.id_modelo}.xlsx"))
                {
                    Verb = "Print",
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

    }
}

