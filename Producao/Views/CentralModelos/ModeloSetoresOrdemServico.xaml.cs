using Dapper;
using Npgsql;
using Producao.DataBase.Model;
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
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;


namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Lógica interna para ModeloSetoresOrdemServico.xaml
    /// </summary>
    public partial class ModeloSetoresOrdemServico : Window
    {
        //private long? codcompladicional;
        private ModeloGerarOsModel modeloControle;
        private ProdutoOsModel produtoOsModel;
        ModeloSetoresOrdemServicoViewModel vm;
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public ModeloSetoresOrdemServico(ModeloGerarOsModel modeloControle)
        {
            InitializeComponent();
            this.DataContext = new ModeloSetoresOrdemServicoViewModel();
            this.modeloControle = modeloControle;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vm = (ModeloSetoresOrdemServicoViewModel)DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Setores = await vm.GetSetorsAsync();
                vm.Itens = [];
                //vm.Itens = await vm.GetSetoresProdutoAsync(modeloControle.codcompladicional);
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });
                //vm.Itens.Add(new HistoricoSetorModel { selesao = false });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void dgItens_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dgItens_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.NewObject = new HistoricoSetorModel
            {
                observacao = this.modeloControle.obs,
                selesao = true
            };
        }

        private void OnAddSetor(object sender, RoutedEventArgs e)
        {
            adicionarSetor();
        }

        private async void adicionarSetor()
        {
            Window window = new()
            {
                Owner = this,
                Title = "ADICIONAR SETOR",
                WindowStyle = WindowStyle.ToolWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Content = new AddSetorOrdemServico(this.DataContext),
                Width = 300,
                Height = 300,
            };
            window.ShowDialog();
        }

        private async void OnCreateOrdemServico(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                //ModeloSetoresOrdemServicoViewModel vm = (ModeloSetoresOrdemServicoViewModel)DataContext;

                var setor = false;
                foreach (var item in vm.Itens.Where(i => i.codigo_setor.HasValue))
                {
                    item.selesao = true;
                    item.setor = vm.Setores?.FirstOrDefault(s => s.codigo_setor == item.codigo_setor)?.setor;
                }

                foreach (var item in vm.Itens)
                    if (item.selesao == true) setor = true;

                if (setor == false)
                {
                    MessageBox.Show("Não foi selecionado nenhum setor", "Não é possível emitir Ordem de Serviço");
                    //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                vm.Planilhas = await vm.GetPlanilasReceita(modeloControle.id_modelo);
                if (vm.Planilhas.Count == 0 && (modeloControle.planilha != "VASO" && modeloControle.planilha != "TOPIÁRIA"))
                {
                    MessageBox.Show("Não existe item na receita do modelo", "Não é possível emitir Ordem de Serviço");
                    //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                await vm.CreateOrdenServicoAsync(modeloControle);

                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;

                await ImprimpirRequisicao();
                await ImprimpirOS();
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                MessageBox.Show("OS E REQUISIÇÃO EMITIDAS E IMPRESSAS");
                this.Close();



            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnPrintOS(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await ImprimpirRequisicao();
                await ImprimpirOS();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

        }

        private async Task ImprimpirOS()
        {
            try
            {
                var servicos = await vm.GetOsEmitidas(vm.ProdutoOsModel.num_os_produto);
                var printer = new OrdemServicoModeloPrinter();
                await printer.ImprimirAsync(servicos, async numOsProduto => await vm.GetServicos(numOsProduto));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ImprimpirRequisicao()
        {
            try
            {
                var servico = await vm.GetServicoRequisicao(vm.ProdutoOsModel.num_os_produto);
                var requisicoes = await vm.GetRequisicaoAsync(servico.num_os_servico);

                if (servico.setor_caminho.Contains("FITAS"))
                {
                    await OnPrintControle(servico.id_modelo, servico.num_os_servico);
                }

                foreach (var re in requisicoes)
                {
                    vm.ReqDetalhes = await vm.GetRequisicaoDetalhesAsync(re.num_requisicao);

                    ReqDetalhesModel requi = (from r in vm.ReqDetalhes select r).FirstOrDefault();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;
                    IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("REQUISICAO_MODELO.xlsx"));
                    IWorksheet worksheet = workbook.Worksheets[0];
                    Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                    worksheet.Range["C2"].Number = Convert.ToDouble(requi?.num_requisicao);
                    worksheet.Range["E2"].DateTime = Convert.ToDateTime(requi?.data);
                    worksheet.Range["C3"].Text = requi?.alterado_por;
                    worksheet.Range["G3"].Text = requi?.setor_caminho;
                    worksheet.Range["C4"].Text = requi?.cliente;
                    worksheet.Range["F4"].Text = requi?.tema;
                    worksheet.Range["C5"].Text = requi?.item_memorial;
                    worksheet.Range["F5"].Text = requi?.local_shoppings;
                    worksheet.Range["C6"].Number = Convert.ToDouble(requi?.num_os_servico);
                    worksheet.Range["E6"].Text = requi?.produtocompleto;
                    worksheet.Range["N6"].Number = Convert.ToDouble(requi?.coddetalhescompl);

                    var itens = (from i in vm.ReqDetalhes where i.quantidade > 0 select new { i.quantidade, i.planilha, i.descricao_completa, i.unidade, i.observacao, i.codcompladicional }).ToList();
                    var index = 9;
                    foreach (var item in itens)
                    {
                        worksheet.Range[$"A{index}"].Number = (double)item.quantidade;
                        worksheet.Range[$"A{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[$"A{index}"].CellStyle.Font.Size = 7;

                        worksheet.Range[$"B{index}"].Number = (double)item.codcompladicional;
                        worksheet.Range[$"B{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[$"B{index}"].CellStyle.Font.Size = 7;

                        worksheet.Range[$"C{index}:D{index}"].Text = item.planilha;
                        worksheet.Range[$"C{index}:D{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                        worksheet.Range[$"C{index}:D{index}"].CellStyle.Font.Size = 7;
                        worksheet.Range[$"C{index}:D{index}"].Merge();
                        worksheet.Range[$"C{index}:D{index}"].WrapText = true;

                        worksheet.Range[$"E{index}:K{index}"].Text = item.descricao_completa;
                        worksheet.Range[$"E{index}:K{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                        worksheet.Range[$"E{index}:K{index}"].CellStyle.Font.Size = 7;
                        worksheet.Range[$"E{index}:K{index}"].Merge();
                        worksheet.Range[$"E{index}:K{index}"].WrapText = true;

                        worksheet.Range[$"L{index}"].Text = item.unidade;
                        worksheet.Range[$"L{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[$"L{index}"].CellStyle.Font.Size = 7;

                        worksheet.Range[$"M{index}:N{index}"].Text = item.observacao;
                        worksheet.Range[$"M{index}:N{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                        worksheet.Range[$"M{index}:N{index}"].CellStyle.Font.Size = 7;
                        worksheet.Range[$"M{index}:N{index}"].Merge();
                        worksheet.Range[$"M{index}:N{index}"].WrapText = true;
                        worksheet.Range[$"M{index}:N{index}"].AdjustRowHeightToText(15, 9.75);
                        worksheet.Range[$"A{index}:N{index}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                        index++;
                    }
                    //workbook.SaveAs($"Impressos/REQUISICAO_{requi.num_requisicao}.xlsx");
                    workbook.SaveAs(BaseSettings.ResolveImpressosPath($"REQUISICAO_MODELO_{re.num_requisicao}.xlsx"));
                    Process.Start(
                    new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"REQUISICAO_MODELO_{re.num_requisicao}.xlsx"))
                    {
                        Verb = "Print",
                        UseShellExecute = true,
                    });
                    worksheet.Clear();
                    workbook.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnPrintControle(long? idModelo, long? numServico)
        {
            try
            {
                QryModeloModel Modelo = await vm.GetModeloAsync(idModelo);
                vm.ReqDetalhes = await vm.GetRequisicaoDetalServicohesAsync(numServico);

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("RECEITA_CENTRAL_MODELO.xlsx"));
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.Range["A1"].Text = "CENTRAL DE MODELOS - CONTROLE";
                worksheet.Range["C2"].Text = Modelo.id_modelo.ToString();
                worksheet.Range["C3"].Text = Modelo.planilha;
                worksheet.Range["C4"].Text = Modelo.descricao_completa;
                worksheet.Range["C5"].Text = Modelo.tema;
                worksheet.Range["A7"].Text = Modelo.obs_modelo;
                worksheet.Range["H4"].Text = Modelo.multiplica.ToString();

                var index = 9;

                foreach (var item in vm.ReqDetalhes)
                {
                    worksheet.Range[$"A{index}"].Text = item.codcompladicional.ToString();
                    worksheet.Range[$"A{index}"].CellStyle.Font.FontName = "Arial";
                    worksheet.Range[$"A{index}"].CellStyle.Font.Size = 8;

                    worksheet.Range[$"B{index}"].Text = item.planilha;
                    worksheet.Range[$"B{index}"].CellStyle.Font.FontName = "Arial";
                    worksheet.Range[$"B{index}"].CellStyle.Font.Size = 8;

                    worksheet.Range[$"C{index}"].Text = item.descricao_completa;
                    worksheet.Range[$"C{index}"].CellStyle.Font.FontName = "Arial";
                    worksheet.Range[$"C{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"C{index}:E{index}"].Merge();
                    worksheet.Range[$"C{index}:E{index}"].WrapText = true;

                    worksheet.Range[$"F{index}"].Text = item.observacao;
                    worksheet.Range[$"F{index}"].CellStyle.Font.FontName = "Arial";
                    worksheet.Range[$"F{index}"].CellStyle.Font.Size = 8;
                    //worksheet.Range[$"F{index}:G{index}"].Merge();
                    worksheet.Range[$"F{index}"].WrapText = true;

                    worksheet.Range[$"G{index}"].Text = "0";
                    worksheet.Range[$"G{index}"].CellStyle.Font.FontName = "Arial";
                    worksheet.Range[$"G{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"G{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    worksheet.Range[$"H{index}"].Text = item.quantidade.ToString();
                    worksheet.Range[$"H{index}"].CellStyle.Font.FontName = "Arial";
                    worksheet.Range[$"H{index}"].CellStyle.Font.Size = 8;
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
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

    }

    public class ModeloSetoresOrdemServicoViewModel : INotifyPropertyChanged
    {
        static ModeloSetoresOrdemServicoViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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

        private static Task<long> InsertProdutoOsAsync(NpgsqlConnection conn, ProdutoOsModel produto, NpgsqlTransaction transaction)
        {
            const string sql = """
                INSERT INTO producao.tbl_produto_os
                    (tipo, planilha, cod_produto, cod_desc_adicional, cod_compl_adicional, quantidade,
                     data_emissao, responsavel_emissao, id_modelo, solicitado_por, codigo_saida, cliente)
                VALUES
                    (@tipo, @planilha, @cod_produto, @cod_desc_adicional, @cod_compl_adicional, @quantidade,
                     @data_emissao, @responsavel_emissao, @id_modelo, @solicitado_por, @codigo_saida, @cliente)
                RETURNING num_os_produto;
                """;

            return conn.ExecuteScalarAsync<long>(sql, produto, transaction);
        }

        private static Task InsertObsOsAsync(NpgsqlConnection conn, ObsOsModel obs, NpgsqlTransaction transaction)
        {
            const string sql = """
                INSERT INTO producao.tbl_obs_os
                    (num_os_produto, cod_compl_adicional, num_caminho, codigo_setor, setor_caminho,
                     orientacao_caminho, distribuir_os, cliente, solicitado_por, solicitado_data,
                     emitida, produtos_servico_num_os_servico, cancelar, cancelado_por, cancelado_em, pt)
                VALUES
                    (@num_os_produto, @cod_compl_adicional, @num_caminho, @codigo_setor, @setor_caminho,
                     @orientacao_caminho, @distribuir_os, @cliente, @solicitado_por, @solicitado_data,
                     @emitida, @produtos_servico_num_os_servico, @cancelar, @cancelado_por, @cancelado_em, @pt);
                """;

            return conn.ExecuteAsync(sql, obs, transaction);
        }

        private static Task<long> InsertProdutoServicoAsync(NpgsqlConnection conn, ProdutoServicoModel servico, NpgsqlTransaction transaction)
        {
            const string sql = """
                INSERT INTO producao.tbl_produtos_servico
                    (num_os_produto, tipo, codigo_setor, setor_caminho, quantidade, data_inicio, data_fim,
                     cliente, tema, orientacao_caminho, codigo_setor_proximo, setor_caminho_proximo, fase,
                     responsavel_emissao_os, emitida_por, emitida_data, meta_data, turno, ajuste_projeto,
                     cancelada_os, retrabalho, recebido_setor_data, concluida_os_data, impresso,
                     cod_detalhe_compl, id_modelo, alterado_por, alterado_data, status, data_status,
                     status_por, motivo_cancelamento, aprovado, aprovado_por, aprovado_em, programacao_ordem,
                     programacao_status, programacao_observacao, programacao_inserido_por, programacao_inserido_data,
                     meta_lider, pagina, pt)
                VALUES
                    (@num_os_produto, @tipo, @codigo_setor, @setor_caminho, @quantidade, @data_inicio, @data_fim,
                     @cliente, @tema, @orientacao_caminho, @codigo_setor_proximo, @setor_caminho_proximo, @fase,
                     @responsavel_emissao_os, @emitida_por, @emitida_data, @meta_data, @turno, @ajuste_projeto,
                     @cancelada_os, @retrabalho, @recebido_setor_data, @concluida_os_data, @impresso,
                     @cod_detalhe_compl, @id_modelo, @alterado_por, @alterado_data, @status, @data_status,
                     @status_por, @motivo_cancelamento, @aprovado, @aprovado_por, @aprovado_em, @programacao_ordem,
                     @programacao_status, @programacao_observacao, @programacao_inserido_por, @programacao_inserido_data,
                     @meta_lider, @pagina, @pt)
                RETURNING num_os_servico;
                """;

            return conn.ExecuteScalarAsync<long>(sql, servico, transaction);
        }

        private static Task<long> InsertRequisicaoAsync(NpgsqlConnection conn, RequisicaoModel requisicao, NpgsqlTransaction transaction)
        {
            const string sql = """
                INSERT INTO producao.t_requisicao
                    (num_os_servico, data, alterado_por, concluida)
                VALUES
                    (@num_os_servico, @data, @alterado_por, @concluida)
                RETURNING num_requisicao;
                """;

            return conn.ExecuteScalarAsync<long>(sql, requisicao, transaction);
        }

        private static Task InsertDetalheRequisicaoAsync(NpgsqlConnection conn, DetalheRequisicaoModel detalhe, NpgsqlTransaction transaction)
        {
            const string sql = """
                INSERT INTO producao.t_detalhes_req
                    (num_requisicao, quantidade, data, alterado_por, ok, data_ok, ok_expedido, observacao,
                     voltagem, local_shop, complemento_chk, codcompladicional, volume, dividir_qtd_volume, agupar)
                VALUES
                    (@num_requisicao, @quantidade, @data, @alterado_por, @ok, @data_ok, @ok_expedido, @observacao,
                     @voltagem, @local_shop, @complemento_chk, @codcompladicional, @volume, @dividir_qtd_volume, @agupar);
                """;

            return conn.ExecuteAsync(sql, detalhe, transaction);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private HistoricoSetorModel _item;
        public HistoricoSetorModel Iteme
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Iteme"); }
        }
        private ObservableCollection<HistoricoSetorModel> _itens;
        public ObservableCollection<HistoricoSetorModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private SetorModel _setor;
        public SetorModel Setor
        {
            get { return _setor; }
            set { _setor = value; RaisePropertyChanged("Setor"); }
        }
        private ObservableCollection<SetorModel> _setores;
        public ObservableCollection<SetorModel> Setores
        {
            get { return _setores; }
            set { _setores = value; RaisePropertyChanged("Setores"); }
        }

        private OsEmissaoProducaoImprimirModel _emitida;
        public OsEmissaoProducaoImprimirModel Emitida
        {
            get { return _emitida; }
            set { _emitida = value; RaisePropertyChanged("Emitida"); }
        }
        private ObservableCollection<OsEmissaoProducaoImprimirModel> _emitidas;
        public ObservableCollection<OsEmissaoProducaoImprimirModel> Emitidas
        {
            get { return _emitidas; }
            set { _emitidas = value; RaisePropertyChanged("Emitidas"); }
        }

        private ProdutoServicoModel _serviso;
        public ProdutoServicoModel Servico
        {
            get { return _serviso; }
            set { _serviso = value; RaisePropertyChanged("Servico"); }
        }
        private ObservableCollection<ProdutoServicoModel> _servicos;
        public ObservableCollection<ProdutoServicoModel> Servicos
        {
            get { return _servicos; }
            set { _servicos = value; RaisePropertyChanged("Servicos"); }
        }

        private ObservableCollection<string> _planilhas;
        public ObservableCollection<string> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
        }

        private ProdutoOsModel _produtoOsModel;
        public ProdutoOsModel ProdutoOsModel
        {
            get { return _produtoOsModel; }
            set { _produtoOsModel = value; RaisePropertyChanged("ProdutoOsModel"); }
        }

        private ObservableCollection<QryRequisicaoDetalheModel> _qryRequisicaoDetalhes;
        public ObservableCollection<QryRequisicaoDetalheModel> QryRequisicaoDetalhes
        {
            get { return _qryRequisicaoDetalhes; }
            set { _qryRequisicaoDetalhes = value; RaisePropertyChanged("QryRequisicaoDetalhes"); }
        }


        private ObservableCollection<ReqDetalhesModel> _reqDetalhes;
        public ObservableCollection<ReqDetalhesModel> ReqDetalhes
        {
            get { return _reqDetalhes; }
            set { _reqDetalhes = value; RaisePropertyChanged("ReqDetalhes"); }
        }

        public async Task<ObservableCollection<HistoricoSetorModel>> GetSetoresProdutoAsync(long? codcompladicional)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.view_historico_setor
                    WHERE codcompladicional = @codcompladicional;
                    """;

                var data = await QueryAsync<HistoricoSetorModel>(sql, new { codcompladicional });
                return new ObservableCollection<HistoricoSetorModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ObservableCollection<SetorModel>> GetSetorsAsync()
        {
            try
            {
                const string sql = """
                    SELECT
                        setor || ' - ' || galpao AS setor,
                        codigo_setor
                    FROM producao.tbl_setor
                    WHERE inativo = '0    ';
                    """;

                var data = await QueryAsync<SetorModel>(sql);
                return new ObservableCollection<SetorModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateOrdenServicoAsync(ModeloGerarOsModel modeloControle)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            var Setores = (from e in Itens where e.selesao == true select e).ToList();
            if (Setores.Count == 0)
                throw new InvalidOperationException("Não existe setor para criar ordem de serviço.");

            var quantidade = (modeloControle?.qtde - (int)(modeloControle?.qtde_os ?? 0));
            try
            {
                const string produtoSql = """
                    SELECT *
                    FROM producao.qry3descricoes
                    WHERE codcompladicional = @codcompladicional
                    LIMIT 1;
                    """;

                var produto = await conn.QueryFirstOrDefaultAsync<QryDescricao>(
                    produtoSql,
                    new { modeloControle.codcompladicional },
                    transaction);

                if (produto is null)
                    throw new InvalidOperationException("Produto não encontrado para criar ordem de serviço.");

                var produtoOsModel = new ProdutoOsModel
                {
                    tipo = "KIT",
                    planilha = produto.planilha,
                    quantidade = quantidade,
                    responsavel_emissao = Environment.UserName,
                    data_emissao = DateTime.Now,
                    cod_produto = produto.codigo,
                    cod_desc_adicional = produto.coduniadicional,
                    cod_compl_adicional = produto.codcompladicional,
                    id_modelo = modeloControle.id_modelo
                };
                produtoOsModel.num_os_produto = await InsertProdutoOsAsync(conn, produtoOsModel, transaction);

                ProdutoOsModel = produtoOsModel;

                for (int i = 0; i < Setores.Count; i++)
                {
                    var item = Setores[i];
                    var Obs = new ObsOsModel
                    {
                        num_os_produto = produtoOsModel.num_os_produto,
                        cod_compl_adicional = produto.codcompladicional,
                        num_caminho = i + 1,
                        codigo_setor = item.codigo_setor,
                        setor_caminho = item.setor,
                        orientacao_caminho = item.observacao,
                        distribuir_os = "No setor",
                        cliente = modeloControle.sigla,
                        solicitado_por = Environment.UserName,
                        solicitado_data = DateTime.Now
                    };

                    await InsertObsOsAsync(conn, Obs, transaction);
                }

                const string solicitAbertaSql = """
                    SELECT *
                    FROM producao.qry_os_emissao_aberta_form
                    WHERE num_os_produto = @num_os_produto
                    ORDER BY num_caminho;
                    """;
                var solictAberta = (await conn.QueryAsync<OrdemServicoEmissaoAbertaForm>(
                    solicitAbertaSql,
                    new { produtoOsModel.num_os_produto },
                    transaction)).ToList();

                for (int i = 0; i < solictAberta.Count; i++)
                {
                    var item = solictAberta[i];
                    var produtoServicoModel = new ProdutoServicoModel
                    {
                        num_os_produto = item.num_os_produto,
                        tipo = item.tipo,
                        codigo_setor = item.codigo_setor,
                        setor_caminho = item.setor_caminho,
                        quantidade = item.quantidade,
                        data_inicio = DateTime.Now,
                        data_fim = DateTime.Now.AddDays(15),
                        cliente = item.cliente,
                        tema = item.tema,
                        orientacao_caminho = item.orientacao_caminho,
                        codigo_setor_proximo = ((i + 1) < solictAberta.Count) ? solictAberta[i + 1].codigo_setor : 39,
                        setor_caminho_proximo = ((i + 1) < solictAberta.Count) ? solictAberta[i + 1].setor_caminho : "FINAL - TODOS",
                        fase = "PRODUÇÃO",
                        responsavel_emissao_os = Environment.UserName,
                        emitida_por = Environment.UserName,
                        emitida_data = DateTime.Now,
                        turno = "DIURNO",
                        id_modelo = item.id_modelo,
                    };

                    produtoServicoModel.num_os_servico = await InsertProdutoServicoAsync(conn, produtoServicoModel, transaction);

                    //ADICIONAR REQUISIÇÃO DE MATERIAL
                    if (i == 0)
                    {
                        foreach (var planilha in Planilhas)
                        {
                            if (!planilha.Contains("FITAS"))
                            {
                                var requisicao = new RequisicaoModel { num_os_servico = produtoServicoModel.num_os_servico, data = DateTime.Now, alterado_por = Environment.UserName };
                                requisicao.num_requisicao = await InsertRequisicaoAsync(conn, requisicao, transaction);

                                const string detalhesSql = """
                                    SELECT *
                                    FROM modelos.qry_detalhes_modelo
                                    WHERE planilha = @planilha
                                      AND id_modelo = @id_modelo;
                                    """;

                                var detalhes = await conn.QueryAsync<DetalhesModeloModel>(
                                    detalhesSql,
                                    new { planilha, modeloControle.id_modelo },
                                    transaction);

                                foreach (DetalhesModeloModel detalhe in detalhes)
                                {
                                    var detReq = new DetalheRequisicaoModel
                                    {
                                        num_requisicao = requisicao.num_requisicao,
                                        codcompladicional = detalhe.codcompladicional,
                                        quantidade = modeloControle.planilha == "ADEREÇO" || modeloControle.planilha == "FIADA" || modeloControle.planilha == "ENF PISO" ? Math.Ceiling((double)detalhe.qtd) : Math.Ceiling((double)(detalhe.qtd * produtoServicoModel.quantidade)),
                                        observacao = detalhe.observacao,
                                        data = DateTime.Now,
                                        alterado_por = Environment.UserName
                                    };
                                    await InsertDetalheRequisicaoAsync(conn, detReq, transaction);
                                }
                            }
                        }
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ProdutoServicoModel> GetServicoRequisicao(long? num_os_produto)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.tbl_produtos_servico
                    WHERE num_os_produto = @num_os_produto
                    LIMIT 1;
                    """;

                var data = await QueryFirstOrDefaultAsync<ProdutoServicoModel>(sql, new { num_os_produto });
                return data;
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

        public async Task<ObservableCollection<string>> GetPlanilasReceita(long? id_modelo)
        {
            try
            {
                const string sql = """
                    SELECT DISTINCT planilha
                    FROM modelos.qry_receita_detalhes_criado
                    WHERE id_modelo = @id_modelo
                    ORDER BY planilha;
                    """;

                var data = await QueryAsync<string>(sql, new { id_modelo });

                ObservableCollection<string> nos = new ObservableCollection<string>();
                foreach (var item in data)
                    nos.Add(item);

                return nos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<Requisicao>> GetRequisicaoAsync(long? num_os_servico)
        {
            try
            {
                const string sql = """
                    SELECT num_requisicao
                    FROM modelos.qry_req_detalhes
                    WHERE num_os_servico = @num_os_servico
                    GROUP BY num_requisicao;
                    """;

                var data = await QueryAsync<Requisicao>(sql, new { num_os_servico });
                return new ObservableCollection<Requisicao>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ReqDetalhesModel>> GetRequisicaoDetalhesAsync(long? num_requisicao)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qry_req_detalhes
                    WHERE num_requisicao = @num_requisicao;
                    """;

                var data = await QueryAsync<ReqDetalhesModel>(sql, new { num_requisicao });
                return new ObservableCollection<ReqDetalhesModel>(data);
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

        public async Task<ObservableCollection<ReqDetalhesModel>> GetRequisicaoDetalServicohesAsync(long? num_os_servico)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qry_req_detalhes
                    WHERE num_os_servico = @num_os_servico
                      AND quantidade > 0;
                    """;

                var data = await QueryAsync<ReqDetalhesModel>(sql, new { num_os_servico });
                return new ObservableCollection<ReqDetalhesModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public class Requisicao
    {
        public long? num_requisicao { get; set; }
    }
}

