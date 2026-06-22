using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Producao.Views.CentralModelos.Compat;
using Producao.Views.OrdemServico;
using System;
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
                vm.Setores = await Task.Run(vm.GetSetorsAsync);
                vm.Itens = [];
                //vm.Itens = await Task.Run(() => vm.GetSetoresProdutoAsync(modeloControle.codcompladicional));
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
                MessageBox.Show(ex.Message);
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

                vm.Planilhas = await Task.Run(() => vm.GetPlanilasReceita(modeloControle.id_modelo));
                if (vm.Planilhas.Count == 0 && (modeloControle.planilha != "VASO" && modeloControle.planilha != "TOPIÁRIA"))
                {
                    MessageBox.Show("Não existe item na receita do modelo", "Não é possível emitir Ordem de Serviço");
                    //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                await Task.Run(() => vm.CreateOrdenServicoAsync(modeloControle));

                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;

                await Task.Run(ImprimpirRequisicao);
                await Task.Run(ImprimpirOS);
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                MessageBox.Show("OS E REQUISIÇÃO EMITIDAS E IMPRESSAS");
                this.Close();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnPrintOS(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(ImprimpirRequisicao);
                await Task.Run(ImprimpirOS);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

        }

        private async Task ImprimpirOS()
        {
            try
            {
                var servicos = await Task.Run(() => vm.GetOsEmitidas(vm.ProdutoOsModel.num_os_produto));
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
                var servico = await Task.Run(() => vm.GetServicoRequisicao(vm.ProdutoOsModel.num_os_produto));
                var requisicoes = await Task.Run(() => vm.GetRequisicaoAsync(servico.num_os_servico));

                if (servico.setor_caminho.Contains("FITAS"))
                {
                    await Task.Run(() => OnPrintControle(servico.id_modelo, servico.num_os_servico));
                }

                foreach (var re in requisicoes)
                {
                    vm.ReqDetalhes = await Task.Run(() => vm.GetRequisicaoDetalhesAsync(re.num_requisicao));

                    ReqDetalhesModel requi = (from r in vm.ReqDetalhes select r).FirstOrDefault();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;
                    IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("REQUISICAO_MODELO.xlsx"));
                    IWorksheet worksheet = workbook.Worksheets[0];
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
                QryModeloModel Modelo = await Task.Run(() => vm.GetModeloAsync(idModelo));
                vm.ReqDetalhes = await Task.Run(() => vm.GetRequisicaoDetalServicohesAsync(numServico));

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("RECEITA_CENTRAL_MODELO.xlsx"));
                IWorksheet worksheet = workbook.Worksheets[0];
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
                MessageBox.Show(ex.Message);
            }
        }

    }

    public class ModeloSetoresOrdemServicoViewModel : INotifyPropertyChanged
    {
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
                using DatabaseContext db = new();
                var data = await db.HistoricosSetor.Where(c => c.codcompladicional == codcompladicional).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await (from s in db.SetorProducaos where s.inativo == "0    " select new SetorModel { setor =  s.setor + " - " + s.galpao, codigo_setor = s.codigo_setor}).ToListAsync();
                return new ObservableCollection<SetorModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateOrdenServicoAsync(ModeloGerarOsModel modeloControle)
        {
            using DatabaseContext db = new();
            var strategy = db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () => 
            {
                using var transaction = db.Database.BeginTransaction();
                var Setores = (from e in Itens where e.selesao == true select e).ToList();
                if (Setores.Count == 0)
                    throw new InvalidOperationException("Não existe setor para criar ordem de serviço.");

                var quantidade = (modeloControle?.qtde - (int)(modeloControle?.qtde_os ?? 0));
                try
                {
                    var produto = await db.Descricoes.Where(d => d.codcompladicional == modeloControle.codcompladicional).FirstOrDefaultAsync();
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
                    await db.ProdutoOs.AddAsync(produtoOsModel);
                    await db.SaveChangesAsync();

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

                        await db.ObsOs.AddAsync(Obs);
                        await db.SaveChangesAsync();
                    }

                    var solictAberta = await db.OrdemServicoEmissaoAbertas.OrderBy(o => o.num_caminho).Where(o => o.num_os_produto == produtoOsModel.num_os_produto).ToListAsync();
                    for (int i = 0; i < solictAberta.Count; i++)
                    {
                        var item = solictAberta[i];
                        var teste = ((i + 1) < solictAberta.Count);
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

                        await db.ProdutoServicos.AddAsync(produtoServicoModel);
                        await db.SaveChangesAsync();


                        //ADICIONAR REQUISIÇÃO DE MATERIAL
                        if (i == 0)
                        {
                            foreach (var planilha in Planilhas)
                            {
                                if (!planilha.Contains("FITAS"))
                                {
                                    var requisicao = new RequisicaoModel { num_os_servico = produtoServicoModel.num_os_servico, data = DateTime.Now, alterado_por = Environment.UserName };

                                    await db.Requisicoes.SingleMergeAsync(requisicao);
                                    await db.SaveChangesAsync();

                                    //adicinar requisicao

                                    //var detalhes = db.DetalhesModelo.Where()
                                    //modeloControle.planilha
                                    var detalhes = await db.DetalhesModelo.Where(d => d.planilha == planilha && d.id_modelo == modeloControle.id_modelo).ToListAsync();
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
                                        await db.RequisicaoDetalhes.SingleMergeAsync(detReq);
                                        await db.SaveChangesAsync();
                                    }
                                    //throw new Exception("FORÇAR ERRO PARA NÃO CONCLUIR A TRANSAÇÃO");
                                    // IMPRIMIR REQUISIÇÃO
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }); 
        }

        public async Task<ProdutoServicoModel> GetServicoRequisicao(long? num_os_produto)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ProdutoServicos.Where(i => i.num_os_produto == num_os_produto).FirstOrDefaultAsync();
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
                using DatabaseContext db = new();
                var data = await db.ImprimirOsS.Where(i => i.num_os_produto == num_os_produto).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.ProdutoServicos.Where(i => i.num_os_produto == num_os_produto).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.qryReceitas.Where(r => r.id_modelo == id_modelo).OrderBy(g => g.planilha).GroupBy(g => new { g.planilha, g.id_modelo }).Select(p => new { p.Key.planilha }).ToListAsync();

                ObservableCollection<string> nos = new ObservableCollection<string>();
                foreach (var item in data)
                    nos.Add(item.planilha);

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
                using DatabaseContext db = new();
                var data = await db.ReqDetalhes
                    .Where(r => r.num_os_servico == num_os_servico)
                    .GroupBy(p => p.num_requisicao)
                    .Select(x => new Requisicao { num_requisicao = x.Key })
                    .ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.ReqDetalhes.Where(r => r.num_requisicao == num_requisicao).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.qryModelos.Where(r => r.id_modelo == idModelo).FirstOrDefaultAsync();
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
                using DatabaseContext db = new();
                var data = await db.ReqDetalhes.Where(r => r.num_os_servico == num_os_servico && r.quantidade > 0).ToListAsync();
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
