using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.Views.OrdemServico.Requisicao;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao
{
    public class CheckListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SiglaChkListModel> _siglas;
        public ObservableCollection<SiglaChkListModel> Siglas
        {
            get { return _siglas; }
            set
            {
                _siglas = value;
                RaisePropertyChanged("Siglas");
            }
        }
        private SiglaChkListModel _sigla;
        public SiglaChkListModel Sigla
        {
            get { return _sigla; }
            set
            {
                _sigla = value;
                RaisePropertyChanged("Sigla");
            }
        }
        private ObservableCollection<RelplanModel> _planilhas;
        public ObservableCollection<RelplanModel> Planilhas
        {
            get { return _planilhas; }
            set
            {
                _planilhas = value;
                RaisePropertyChanged("Planilhas");
            }
        }
        private RelplanModel _planilha;
        public RelplanModel Planilha
        {
            get { return _planilha; }
            set
            {
                _planilha = value;
                RaisePropertyChanged("Planilha");
            }
        }
        private ObservableCollection<ProdutoModel> _produtos;
        public ObservableCollection<ProdutoModel> Produtos
        {
            get { return _produtos; }
            set
            {
                _produtos = value;
                RaisePropertyChanged("Produtos");
            }
        }
        private ProdutoModel _produto;
        public ProdutoModel Produto
        {
            get { return _produto; }
            set
            {
                _produto = value;
                RaisePropertyChanged("Produto");
            }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _descAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
        {
            get { return _descAdicionais; }
            set
            {
                _descAdicionais = value;
                RaisePropertyChanged("DescAdicionais");
            }
        }
        private TabelaDescAdicionalModel _descAdicional;
        public TabelaDescAdicionalModel DescAdicional
        {
            get { return _descAdicional; }
            set
            {
                _descAdicional = value;
                RaisePropertyChanged("DescAdicional");
            }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _compleAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
        {
            get { return _compleAdicionais; }
            set
            {
                _compleAdicionais = value;
                RaisePropertyChanged("CompleAdicionais");
            }
        }
        private TblComplementoAdicionalModel _compledicional;
        public TblComplementoAdicionalModel Compledicional
        {
            get { return _compledicional; }
            set
            {
                _compledicional = value;
                RaisePropertyChanged("Compledicional");
            }
        }

        private ObservableCollection<ComplementoCheckListModel> _complementoCheckLists;
        public ObservableCollection<ComplementoCheckListModel> ComplementoCheckLists
        {
            get { return _complementoCheckLists; }
            set
            {
                _complementoCheckLists = value;
                RaisePropertyChanged("ComplementoCheckLists");
            }
        }

        private ComplementoCheckListModel complementoCheckList;
        public ComplementoCheckListModel ComplementoCheckList
        {
            get { return complementoCheckList; }
            set
            {
                complementoCheckList = value;
                RaisePropertyChanged("ComplementoCheckList");
            }
        }
        private ObservableCollection<object> _locaisshopping;
        public ObservableCollection<object> Locaisshopping
        {
            get { return _locaisshopping; }
            set
            {
                _locaisshopping = value;
                RaisePropertyChanged("Locaisshopping");
            }
        }

        private QryCheckListGeralModel _checkListGeral;
        public QryCheckListGeralModel CheckListGeral
        {
            get { return _checkListGeral; }
            set
            {
                _checkListGeral = value;
                RaisePropertyChanged("CheckListGeral");
            }
        }
        private ObservableCollection<QryCheckListGeralModel> _checkListGerais;
        public ObservableCollection<QryCheckListGeralModel> CheckListGerais
        {
            get { return _checkListGerais; }
            set
            {
                _checkListGerais = value;
                RaisePropertyChanged("CheckListGerais");
            }
        }

        private DetalhesComplemento _detCompl;
        public DetalhesComplemento DetCompl
        {
            get { return _detCompl; }
            set
            {
                _detCompl = value;
                RaisePropertyChanged("DetCompl");
            }
        }
        private ObservableCollection<DetalhesComplemento> _detCompls;
        public ObservableCollection<DetalhesComplemento> DetCompls
        {
            get { return _detCompls; }
            set
            {
                _detCompls = value;
                RaisePropertyChanged("DetCompls");
            }
        }

        private QryCheckListGeralComplementoModel _checkListGeralComplemento;
        public QryCheckListGeralComplementoModel CheckListGeralComplemento
        {
            get { return _checkListGeralComplemento; }
            set
            {
                _checkListGeralComplemento = value;
                RaisePropertyChanged("CheckListGeralComplemento");
            }
        }
        private ObservableCollection<QryCheckListGeralComplementoModel> _checkListGeralComplementos;
        public ObservableCollection<QryCheckListGeralComplementoModel> CheckListGeralComplementos
        {
            get { return _checkListGeralComplementos; }
            set
            {
                _checkListGeralComplementos = value;
                RaisePropertyChanged("CheckListGeralComplementos");
            }
        }

        private SetorProducaoModel _setorProducao;
        public SetorProducaoModel SetorProducao
        {
            get { return _setorProducao; }
            set
            {
                _setorProducao = value;
                RaisePropertyChanged("SetorProducao");
            }
        }
        private ObservableCollection<SetorProducaoModel> _setoresProducao;
        public ObservableCollection<SetorProducaoModel> SetoresProducao
        {
            get { return _setoresProducao; }
            set
            {
                _setoresProducao = value;
                RaisePropertyChanged("SetoresProducao");
            }
        }


        /**/
        private ProdutoOsModel _produtoOs;
        public ProdutoOsModel ProdutoOs
        {
            get { return _produtoOs; }
            set
            {
                _produtoOs = value;
                RaisePropertyChanged("ProdutoOs");
            }
        }
        private ObservableCollection<ProdutoOsModel> _produtoOss;
        public ObservableCollection<ProdutoOsModel> ProdutoOss
        {
            get { return _produtoOss; }
            set
            {
                _produtoOss = value;
                RaisePropertyChanged("ProdutoOss");
            }
        }

        /**/
        private ProdutoServicoModel _produtoServico;
        public ProdutoServicoModel ProdutoServico
        {
            get { return _produtoServico; }
            set
            {
                _produtoServico = value;
                RaisePropertyChanged("ProdutoServico");
            }
        }
        private ObservableCollection<ProdutoServicoModel> _produtoServicos;
        public ObservableCollection<ProdutoServicoModel> ProdutoServicos
        {
            get { return _produtoServicos; }
            set
            {
                _produtoServicos = value;
                RaisePropertyChanged("ProdutoServicos");
            }
        }

        private RequisicaoReceitaModel _requiReceita;
        public RequisicaoReceitaModel RequiReceita
        {
            get { return _requiReceita; }
            set { _requiReceita = value; RaisePropertyChanged("RequiReceita"); }
        }

        private ObservableCollection<RequisicaoReceitaModel> _requiReceitas;
        public ObservableCollection<RequisicaoReceitaModel> RequiReceitas
        {
            get { return _requiReceitas; }
            set { _requiReceitas = value; RaisePropertyChanged("RequiReceitas"); }
        }

        /**/
        private RequisicaoModel _requisicao;
        public RequisicaoModel Requisicao
        {
            get { return _requisicao; }
            set
            {
                _requisicao = value;
                RaisePropertyChanged("Requisicao");
            }
        }
        private ObservableCollection<RequisicaoModel> _requisicoes;
        public ObservableCollection<RequisicaoModel> Requisicoes
        {
            get { return _requisicoes; }
            set
            {
                _requisicoes = value;
                RaisePropertyChanged("Requisicoes");
            }
        }

        /**/
        private DetalheRequisicaoModel _requisicaoDetalhe;
        public DetalheRequisicaoModel RequisicaoDetalhe
        {
            get { return _requisicaoDetalhe; }
            set
            {
                _requisicaoDetalhe = value;
                RaisePropertyChanged("RequisicaoDetalhe");
            }
        }
        private ObservableCollection<DetalheRequisicaoModel> _requisicaoDetalhes;
        public ObservableCollection<DetalheRequisicaoModel> RequisicaoDetalhes
        {
            get { return _requisicaoDetalhes; }
            set
            {
                _requisicaoDetalhes = value;
                RaisePropertyChanged("RequisicaoDetalhes");
            }
        }

        private QryRequisicaoDetalheModel _qryRequisicaoDetalhe;
        public QryRequisicaoDetalheModel QryRequisicaoDetalhe
        {
            get { return _qryRequisicaoDetalhe; }
            set
            {
                _qryRequisicaoDetalhe = value;
                RaisePropertyChanged("QryRequisicaoDetalhe");
            }
        }
        private ObservableCollection<QryRequisicaoDetalheModel> _qryRequisicaoDetalhes;
        public ObservableCollection<QryRequisicaoDetalheModel> QryRequisicaoDetalhes
        {
            get { return _qryRequisicaoDetalhes; }
            set
            {
                _qryRequisicaoDetalhes = value;
                RaisePropertyChanged("QryRequisicaoDetalhes");
            }
        }


        private IList _chkGeralRelatorios;
        public IList ChkGeralRelatorios
        {
            get { return _chkGeralRelatorios; }
            set
            {
                _chkGeralRelatorios = value;
                RaisePropertyChanged("ChkGeralRelatorios");
            }
        }

        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get
            {
                return rowDataCommand;
            }
            set
            {
                rowDataCommand = value;
            }
        }


        public CheckListViewModel()
        {
            DetCompl = new DetalhesComplemento();
            ComplementoCheckList = new ComplementoCheckListModel();
            Siglas = new ObservableCollection<SiglaChkListModel>();
            Planilhas = new ObservableCollection<RelplanModel>();

            //Task.Run(async () => { Siglas = await GetSiglasAsync(); });
            //Task.Run(async () => { Planilhas = await GetPlanilhasAsync(); });

            rowDataCommand = new RelayCommand(ChangeCanExecute);
        }
        public async Task<ObservableCollection<SetorProducaoModel>> GetSetoresAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.SetorProducaos.OrderBy(c => c.setor).Where(c => c.inativo.Equals("0")).ToListAsync();
                return new ObservableCollection<SetorProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async void ChangeCanExecute(object obj)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                if (CheckListGeralComplemento == null)
                {
                    MessageBox.Show("Salva o registro para poder criar requisição");
                    return;
                }

                this.SetoresProducao = await Task.Run(GetSetoresAsync);
                var window = new Window();
                var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                SfMultiColumnDropDownControl sfMultiColumn = new SfMultiColumnDropDownControl();
                sfMultiColumn.Height= 38;
                sfMultiColumn.AllowAutoComplete = true;
                sfMultiColumn.IsDropDownOpen = true;
                sfMultiColumn.AutoGenerateColumns = false;
                sfMultiColumn.GridColumnSizer = GridLengthUnitType.AutoLastColumnFill;
                sfMultiColumn.SearchCondition = SearchCondition.Contains; //"Contains";
                sfMultiColumn.DisplayMember = "setor";
                sfMultiColumn.ValueMember = "codigo_setor";
                sfMultiColumn.Columns.Add(new GridTextColumn() { MappingName = "setor" });
                sfMultiColumn.Columns.Add(new GridTextColumn() { MappingName = "localizacao" });
                sfMultiColumn.Columns.Add(new GridTextColumn() { MappingName = "galpao" });
                sfMultiColumn.ItemsSource = SetoresProducao;
                stackPanel.Children.Add(sfMultiColumn);
                Button btn = new Button();
                btn.Content = "OK";
                btn.Click += async (s, e) =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        SetorProducao = (SetorProducaoModel)sfMultiColumn.SelectedItem;
                        var produtoServico = await Task.Run(() => CriateOsChklistAsync(SetorProducao)); 
                        window.Close();
                        RequisicaoMaterial detailsWindow = new RequisicaoMaterial(produtoServico);
                        detailsWindow.Owner = Window.GetWindow((DependencyObject)obj); //Window.GetWindow((DependencyObject)obj)
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        detailsWindow.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    } 
                };

                var produtoServico = await Task.Run(async () => await GetProdutoServicoAsync(CheckListGeralComplemento.coddetalhescompl));

                if (produtoServico == null)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                    stackPanel.Children.Add(btn);
                    window.Content = stackPanel;
                    window.Title = "Criar Requisição";
                    window.Height = 100;
                    window.Width = 350;
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.WindowStyle = WindowStyle.ToolWindow;
                    window.ResizeMode = ResizeMode.NoResize;
                    //window.Owner = (Window)obj;
                    window.ShowDialog();
                    
                }
                else
                {

                    Requisicao = await Task.Run(() => GetRequisicaoAsync(produtoServico.num_os_servico));
                    QryRequisicaoDetalhes = await Task.Run(() => GetRequisicaoDetalhesAsync(Requisicao.num_requisicao));
                    RequisicaoMaterial detailsWindow = new RequisicaoMaterial(produtoServico); //ProdutoServico
                    detailsWindow.Owner = Window.GetWindow((DependencyObject)obj);  //(Window)obj;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    detailsWindow.ShowDialog();
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            
        }


        public async Task<ProdutoServicoModel> CriateOsChklistAsync(SetorProducaoModel setorProducao)
        {
            
            using DatabaseContext db = new();
            var strategy = db.Database.CreateExecutionStrategy();
            ProdutoServicoModel produtoServico = new();
            await strategy.ExecuteAsync(async () => 
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var produtoOs = new ProdutoOsModel
                    {
                        tipo = "PEÇA NOVA",
                        planilha = CheckListGeral.planilha,
                        cod_produto = CheckListGeral.codigo,
                        cod_desc_adicional = CheckListGeral.coduniadicional,
                        cod_compl_adicional = CheckListGeralComplemento.codcompladicional,
                        quantidade = 0,
                        data_emissao = DateTime.Now,
                        responsavel_emissao = Environment.UserName,
                        solicitado_por = Environment.UserName
                    };
                    await db.ProdutoOs.AddAsync(produtoOs);
                    await db.SaveChangesAsync();
                    produtoServico = new ProdutoServicoModel
                    {
                        num_os_produto = produtoOs.num_os_produto,
                        tipo = produtoOs.tipo,
                        codigo_setor = setorProducao.codigo_setor,
                        setor_caminho = $"{setorProducao.setor} - {setorProducao.galpao}",
                        quantidade = produtoOs.quantidade,
                        data_inicio = DateTime.Now,
                        data_fim = DateTime.Now.AddDays(1),
                        cliente = Sigla.sigla_serv,
                        tema = Sigla.tema,
                        orientacao_caminho = "OS DESTINADA A REQUISIÇÃO DE MATERIAL PARA A PLANILHA",
                        codigo_setor_proximo = 39,
                        setor_caminho_proximo = "FINAL - TODOS",
                        fase = "PRODUÇÃO",
                        responsavel_emissao_os = Environment.UserName,
                        emitida_por = Environment.UserName,
                        emitida_data = DateTime.Now,
                        retrabalho = "NÃO",
                        impresso = "-1",
                        cod_detalhe_compl = CheckListGeralComplemento.coddetalhescompl
                    };
                    await db.ProdutoServicos.AddAsync(produtoServico);
                    await db.SaveChangesAsync();
                    var requisicao = new RequisicaoModel
                    {
                        num_os_servico = produtoServico.num_os_servico,
                        data = DateTime.Now,
                        alterado_por = Environment.UserName

                    };
                    await db.Requisicoes.AddAsync(requisicao);
                    await db.SaveChangesAsync();
                    var receita = await db.RequisicaoReceitas.Where(r => r.codcompladicional_produto == produtoOs.cod_compl_adicional).ToListAsync();
                    foreach (var item in receita)
                    {
                        var ReqDetalhe = new DetalheRequisicaoModel
                        {
                            cod_det_req = null,
                            num_requisicao = requisicao.num_requisicao,
                            codcompladicional = item.codcompladicional_receita,
                            quantidade = item.quantidade * CheckListGeralComplemento.qtd,
                            data = DateTime.Now,
                            alterado_por = Environment.UserName
                        };
                        await db.RequisicaoDetalhes.AddAsync(ReqDetalhe);
                        await db.SaveChangesAsync();

                    }

                    transaction.Commit();

                   
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

            });

            return produtoServico;
        }



        public async Task<ProdutoOsModel> CriarOsProdutoAsync(ProdutoOsModel ProdutoOs)
        {
            try
            {
                using DatabaseContext db = new();
                db.Entry(ProdutoOs).State = ProdutoOs.num_os_produto == null ?
                                   EntityState.Added :
                                   EntityState.Modified;

                await db.SaveChangesAsync();
                return ProdutoOs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProdutoServicoModel> CriarProdutoServicoAsync(ProdutoServicoModel ProdutoServico)
        {
            try
            {
                using DatabaseContext db = new();
                db.Entry(ProdutoServico).State = ProdutoServico.num_os_servico == null ?
                                   EntityState.Added :
                                   EntityState.Modified;

                await db.SaveChangesAsync();
                return ProdutoServico;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ProdutoServicoModel> GetProdutoServicoAsync(long? coddetalhescompl)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ProdutoServicos
                    .Where(p => p.cod_detalhe_compl == coddetalhescompl)
                    .FirstOrDefaultAsync();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<RequisicaoReceitaModel>> GetReceitaRequisicaoAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.RequisicaoReceitas.Where(r => r.codcompladicional_produto == codcompladicional).ToListAsync();
                return new ObservableCollection<RequisicaoReceitaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RequisicaoModel> CriarRequisicaoAsync(RequisicaoModel Requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                db.Entry(Requisicao).State = Requisicao.num_requisicao == null ?
                                   EntityState.Added :
                                   EntityState.Modified;

                await db.SaveChangesAsync();
                return Requisicao;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DetalheRequisicaoModel> AddProdutoRequisicaoAsync(DetalheRequisicaoModel RequisicaoDetalhe)
        {
            try
            {
                using DatabaseContext db = new();
                db.Entry(RequisicaoDetalhe).State = RequisicaoDetalhe.cod_det_req == null ?
                                   EntityState.Added :
                                   EntityState.Modified;

                await db.SaveChangesAsync();
                return RequisicaoDetalhe;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RequisicaoModel> GetRequisicaoAsync(long? num_os_servico)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Requisicoes.Where(r => r.num_os_servico == num_os_servico).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryRequisicaoDetalheModel>> GetRequisicaoDetalhesAsync(long? num_requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.QryRequisicaoDetalhes.Where(r => r.num_requisicao == num_requisicao).ToListAsync();
                return new ObservableCollection<QryRequisicaoDetalheModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<SiglaChkListModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Siglas.OrderBy(c => c.sigla_serv).ToListAsync();
                return new ObservableCollection<SiglaChkListModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1") && !c.planilha.Contains("ESTOQUE") && !c.planilha.Contains("ALMOX")).ToListAsync();
                return new ObservableCollection<RelplanModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ObservableCollection<ProdutoModel>> GetProdutosAsync(string? planilha)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Produtos
                    .OrderBy(c => c.descricao)
                    .Where(c => c.planilha.Equals(planilha))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                return new ObservableCollection<ProdutoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ObservableCollection<TabelaDescAdicionalModel>> GetDescAdicionaisAsync(long? codigo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.DescAdicionais
                    .OrderBy(c => c.descricao_adicional)
                    .Where(c => c.codigoproduto.Equals(codigo))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();

                return new ObservableCollection<TabelaDescAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TblComplementoAdicionalModel>> GetCompleAdicionaisAsync(long? coduniadicional)
        {
            try
            {
                CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                using DatabaseContext db = new();
                var data = await db.ComplementoAdicionais
                    .OrderBy(c => c.complementoadicional)
                    .Where(c => c.coduniadicional.Equals(coduniadicional))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();

                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<object>> GetLocaisShoppAsync(long? id_aprovado)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ComplementoCheckLists
                    .OrderBy(c => c.local_shoppings)
                    .Where(c => c.id_aprovado == id_aprovado)
                    .Select(s => new
                    {
                        s.local_shoppings
                    }).ToArrayAsync();
                return new ObservableCollection<object>(data.GroupBy(x => x.local_shoppings));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryCheckListGeralModel>> GetCheckListGeralAsync(long? id_aprovado)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.CheckListGerals
                    .OrderBy(c => c.id)
                    .Where(c => c.id_aprovado == id_aprovado && c.kp == null)
                    .ToListAsync();
                return new ObservableCollection<QryCheckListGeralModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ComplementoCheckListModel> AddComplementoCheckListAsync(ComplementoCheckListModel ComplementoCheckList)
        {
            /*
            if (ComplementoCheckList == null)
            {
                throw new ArgumentNullException($"{nameof(AddComplementoCheckListAsync)} entity must not be null");
            }
            */
            try
            {
                using DatabaseContext db = new();
                //db.Entry(ComplementoCheckList).State = ComplementoCheckList.codcompl == null ? EntityState.Added : EntityState.Modified;
                await db.ComplementoCheckLists.SingleMergeAsync(ComplementoCheckList);
                await db.SaveChangesAsync();

                return ComplementoCheckList;
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task EditComplementoCheckListAsync(ComplementoCheckListModel compChkList)
        {
            try
            {
                using DatabaseContext db = new();
                //db.Entry(ComplementoCheckList).State = ComplementoCheckList.codcompl == null ? EntityState.Added : EntityState.Modified;
                var comple = await db.ComplementoCheckLists.FirstOrDefaultAsync(p => p.codcompl == compChkList.codcompl);
                if (compChkList != null)
                {
                    if (compChkList.obs != "")
                    {
                        comple.obs = compChkList.obs;
                        db.Entry(comple).Property(p => p.obs).IsModified = true;
                    }
                    if (compChkList.orient_montagem != "")
                    {
                        comple.orient_montagem = compChkList.orient_montagem;
                        db.Entry(comple).Property(p => p.orient_montagem).IsModified = true;
                    }
                    if (compChkList.orient_desmont != "")
                    {
                        comple.orient_desmont = compChkList.orient_desmont;
                        db.Entry(comple).Property(p => p.orient_desmont).IsModified = true;
                    }
                    if (compChkList.ordem != "")
                    {
                        comple.ordem = compChkList.ordem;
                        db.Entry(comple).Property(p => p.ordem).IsModified = true;
                    }
                    if (compChkList.carga != "")
                    {
                        comple.carga = compChkList.carga;
                        db.Entry(comple).Property(p => p.carga).IsModified = true;
                    }
                    if (compChkList.alterado_por != "")
                    {
                        comple.alterado_por = compChkList.alterado_por;
                        db.Entry(comple).Property(p => p.alterado_por).IsModified = true;
                    }
                    if (compChkList.alterado_em != null)
                    {
                        comple.alterado_em = compChkList.alterado_em;
                        db.Entry(comple).Property(p => p.alterado_em).IsModified = true;
                    }
                    await db.SaveChangesAsync();
                }
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task<QryCheckListGeralModel> GetSelectCheckListAsync(long CodCompl)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.CheckListGerals
                    .Where(c => c.codcompl == CodCompl)
                    .FirstOrDefaultAsync();
                return data;
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryCheckListGeralComplementoModel>> GetCheckListGeralComplementoAsync(long? codcompl)
        {
            try
            {
                //CheckListGeralComplementos = new ObservableCollection<QryCheckListGeralComplementoModel>();
                using DatabaseContext db = new();
                var data = await db.CheckListGeralComplementos
                    .OrderBy(c => c.coddetalhescompl)
                    .Where(c => c.codcompl == codcompl)
                    .ToListAsync();

                return new ObservableCollection<QryCheckListGeralComplementoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DetalhesComplemento> AddDetalhesComplementoCheckListAsync(DetalhesComplemento detCompl)
        {
            try
            {
                using DatabaseContext db = new();
                //db.Entry(detCompl).State = detCompl.coddetalhescompl == null ? EntityState.Added : EntityState.Modified;
                await db.DetalhesComplementos.SingleMergeAsync(detCompl);
                await db.SaveChangesAsync();

                return detCompl;
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task<DetalhesComplemento> ConfirmarComplementoCheckListAsync(DetalhesComplemento detCompl)
        {
            try
            {
                using DatabaseContext db = new();

                var det = db.DetalhesComplementos.FirstOrDefault(p => p.coddetalhescompl == detCompl.coddetalhescompl);
                if (det != null)
                {
                    // Atualiza somente o campo necessário (no caso, o nome)
                    //produto.Nome = novoNome;

                    // Informa ao Entity Framework que o objeto foi modificado

                    det.confirmado = detCompl.confirmado;
                    det.confirmado_data = detCompl.confirmado_data;
                    det.confirmado_por = detCompl.confirmado_por;
                    det.desabilitado_confirmado_data = detCompl.desabilitado_confirmado_data;
                    det.desabilitado_confirmado_por = detCompl.desabilitado_confirmado_por;

                    db.Entry(det).Property(p => p.confirmado).IsModified = true;
                    db.Entry(det).Property(p => p.confirmado_data).IsModified = true;
                    db.Entry(det).Property(p => p.confirmado_por).IsModified = true;
                    db.Entry(det).Property(p => p.desabilitado_confirmado_data).IsModified = true;
                    db.Entry(det).Property(p => p.desabilitado_confirmado_por).IsModified = true;

                    // Salva apenas a atualização do campo modificado
                    await db.SaveChangesAsync();
                }

                //db.Entry(detCompl).State = detCompl.coddetalhescompl == null ? EntityState.Added : EntityState.Modified;
                //await db.DetalhesComplementos.SingleMergeAsync(detCompl);
                //await db.SaveChangesAsync();

                return det;
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task<IList> GetChkGeralRelatorioAsync(long? id_aprovado)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ChkGeralRelatorios
                    .OrderBy(c => c.ordem)
                    .Where(c => c.id_aprovado == id_aprovado)
                    .Select(s => new
                    {
                        s.item_memorial,
                        s.local_shoppings,
                        s.planilha,
                        s.descricao_dd,
                        s.unidade,
                        s.qtd, //qtd = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:D}", s.qtd),
                        s.orient_montagem,
                        s.coddetalhescompl
                    })
                    .ToListAsync();
                return data;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
