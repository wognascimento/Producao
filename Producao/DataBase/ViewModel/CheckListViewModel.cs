using Dapper;
using Npgsql;
using Producao.Views.OrdemServico.Requisicao;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao
{
    public class CheckListViewModel : INotifyPropertyChanged
    {
        static CheckListViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private sealed class LocalShoppingRow
        {
            public string? local_shoppings { get; set; }
        }

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


            rowDataCommand = new RelayCommand(ChangeCanExecute);
        }
        public async Task<ObservableCollection<SetorProducaoModel>> GetSetoresAsync()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.tbl_setor
                    WHERE inativo = '0'
                    ORDER BY setor;
                    """;
                var data = await QueryAsync<SetorProducaoModel>(sql);
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

                this.SetoresProducao = await GetSetoresAsync();
                var window = new Window();
                var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                var comboSetor = new RadComboBox
                {
                    Height = 38,
                    IsEditable = true,
                    IsFilteringEnabled = true,
                    OpenDropDownOnFocus = true,
                    DisplayMemberPath = "setor",
                    SelectedValuePath = "codigo_setor",
                    ItemsSource = SetoresProducao
                };
                stackPanel.Children.Add(comboSetor);
                Button btn = new Button();
                btn.Content = "OK";
                btn.Click += async (s, e) =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        SetorProducao = comboSetor.SelectedItem as SetorProducaoModel;
                        if (SetorProducao == null)
                        {
                            MessageBox.Show("Selecione um setor.");
                            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                            return;
                        }
                        var produtoServico = await CriateOsChklistAsync(SetorProducao); 
                        var tGlobal = await GetTGlobalAsync(produtoServico.num_os_servico); 
                        window.Close();
                        //RequisicaoMaterial detailsWindow = new RequisicaoMaterial(produtoServico);
                        RequisicaoMaterial detailsWindow = new RequisicaoMaterial(tGlobal);
                        detailsWindow.Owner = Window.GetWindow((DependencyObject)obj); //Window.GetWindow((DependencyObject)obj)
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        detailsWindow.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        Producao.ErrorDialog.Show(ex, "Erro");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    } 
                };

                var produtoServico = await GetProdutoServicoAsync(CheckListGeralComplemento.coddetalhescompl);
                
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
                    var tGlobal = await GetTGlobalAsync(produtoServico.num_os_servico);
                    Requisicao = await GetRequisicaoAsync(produtoServico.num_os_servico);
                    Requisicao ??= await CriateRequisicaoChklistAsync(produtoServico.num_os_servico, CheckListGeralComplemento.codcompladicional);
                    QryRequisicaoDetalhes = await GetRequisicaoDetalhesAsync(Requisicao.num_requisicao);
                    //RequisicaoMaterial detailsWindow = new RequisicaoMaterial(produtoServico); //ProdutoServico
                    RequisicaoMaterial detailsWindow = new RequisicaoMaterial(tGlobal); //ProdutoServico
                    detailsWindow.Owner = Window.GetWindow((DependencyObject)obj);  //(Window)obj;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    detailsWindow.ShowDialog();
                }
                

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            
        }


        public async Task<ProdutoServicoModel> CriateOsChklistAsync(SetorProducaoModel setorProducao)
        {
            ProdutoServicoModel produtoServico = new();
            await using var conn = CreateConnection();
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();
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

                produtoOs.num_os_produto = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO producao.tbl_produto_os
                        (tipo, planilha, cod_produto, cod_desc_adicional, cod_compl_adicional, quantidade, data_emissao, responsavel_emissao, solicitado_por)
                    VALUES
                        (@tipo, @planilha, @cod_produto, @cod_desc_adicional, @cod_compl_adicional, @quantidade, @data_emissao, @responsavel_emissao, @solicitado_por)
                    RETURNING num_os_produto;
                    """,
                    produtoOs,
                    transaction);

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

                produtoServico.num_os_servico = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO producao.tbl_produtos_servico
                        (num_os_produto, tipo, codigo_setor, setor_caminho, quantidade, data_inicio, data_fim, cliente, tema, orientacao_caminho,
                         codigo_setor_proximo, setor_caminho_proximo, fase, responsavel_emissao_os, emitida_por, emitida_data, retrabalho, impresso, cod_detalhe_compl)
                    VALUES
                        (@num_os_produto, @tipo, @codigo_setor, @setor_caminho, @quantidade, @data_inicio, @data_fim, @cliente, @tema, @orientacao_caminho,
                         @codigo_setor_proximo, @setor_caminho_proximo, @fase, @responsavel_emissao_os, @emitida_por, @emitida_data, @retrabalho, @impresso, @cod_detalhe_compl)
                    RETURNING num_os_servico;
                    """,
                    produtoServico,
                    transaction);

                var requisicao = new RequisicaoModel
                {
                    num_os_servico = produtoServico.num_os_servico,
                    data = DateTime.Now,
                    alterado_por = Environment.UserName
                };

                requisicao.num_requisicao = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO producao.t_requisicao
                        (num_os_servico, data, alterado_por)
                    VALUES
                        (@num_os_servico, @data, @alterado_por)
                    RETURNING num_requisicao;
                    """,
                    requisicao,
                    transaction);

                var receita = await conn.QueryAsync<RequisicaoReceitaModel>(
                    "SELECT * FROM producao.tbl_requisicao_receita WHERE codcompladicional_produto = @cod_compl_adicional;",
                    new { produtoOs.cod_compl_adicional },
                    transaction);

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

                    await conn.ExecuteAsync(
                        """
                        INSERT INTO producao.t_detalhes_req
                            (num_requisicao, codcompladicional, quantidade, data, alterado_por)
                        VALUES
                            (@num_requisicao, @codcompladicional, @quantidade, @data, @alterado_por);
                        """,
                        ReqDetalhe,
                        transaction);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return produtoServico;
        }


        public async Task<RequisicaoModel> CriateRequisicaoChklistAsync(long? num_os_servico, long? cod_compl_adicional)
        {
            RequisicaoModel requisicao = new();
            await using var conn = CreateConnection();
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                requisicao = new RequisicaoModel
                {
                    num_os_servico = num_os_servico,
                    data = DateTime.Now,
                    alterado_por = Environment.UserName
                };

                requisicao.num_requisicao = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO producao.t_requisicao
                        (num_os_servico, data, alterado_por)
                    VALUES
                        (@num_os_servico, @data, @alterado_por)
                    RETURNING num_requisicao;
                    """,
                    requisicao,
                    transaction);

                var receita = await conn.QueryAsync<RequisicaoReceitaModel>(
                    "SELECT * FROM producao.tbl_requisicao_receita WHERE codcompladicional_produto = @cod_compl_adicional;",
                    new { cod_compl_adicional },
                    transaction);

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

                    await conn.ExecuteAsync(
                        """
                        INSERT INTO producao.t_detalhes_req
                            (num_requisicao, codcompladicional, quantidade, data, alterado_por)
                        VALUES
                            (@num_requisicao, @codcompladicional, @quantidade, @data, @alterado_por);
                        """,
                        ReqDetalhe,
                        transaction);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return requisicao;
        }



        public async Task<ProdutoOsModel> CriarOsProdutoAsync(ProdutoOsModel ProdutoOs)
        {
            try
            {
                await using var conn = CreateConnection();
                if (ProdutoOs.num_os_produto is null or 0)
                {
                    ProdutoOs.num_os_produto = await conn.ExecuteScalarAsync<long>(
                        """
                        INSERT INTO producao.tbl_produto_os
                            (tipo, planilha, cod_produto, cod_desc_adicional, cod_compl_adicional, quantidade, data_emissao, responsavel_emissao, id_modelo, solicitado_por, codigo_saida, cliente)
                        VALUES
                            (@tipo, @planilha, @cod_produto, @cod_desc_adicional, @cod_compl_adicional, @quantidade, @data_emissao, @responsavel_emissao, @id_modelo, @solicitado_por, @codigo_saida, @cliente)
                        RETURNING num_os_produto;
                        """,
                        ProdutoOs);
                }
                else
                {
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.tbl_produto_os
                        SET tipo = @tipo,
                            planilha = @planilha,
                            cod_produto = @cod_produto,
                            cod_desc_adicional = @cod_desc_adicional,
                            cod_compl_adicional = @cod_compl_adicional,
                            quantidade = @quantidade,
                            data_emissao = @data_emissao,
                            responsavel_emissao = @responsavel_emissao,
                            id_modelo = @id_modelo,
                            solicitado_por = @solicitado_por,
                            codigo_saida = @codigo_saida,
                            cliente = @cliente
                        WHERE num_os_produto = @num_os_produto;
                        """,
                        ProdutoOs);
                }

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
                await using var conn = CreateConnection();
                if (ProdutoServico.num_os_servico is null or 0)
                {
                    ProdutoServico.num_os_servico = await conn.ExecuteScalarAsync<long>(
                        """
                        INSERT INTO producao.tbl_produtos_servico
                            (num_os_produto, tipo, codigo_setor, setor_caminho, quantidade, data_inicio, data_fim, cliente, tema, orientacao_caminho,
                             codigo_setor_proximo, setor_caminho_proximo, fase, responsavel_emissao_os, emitida_por, emitida_data, meta_data, turno,
                             ajuste_projeto, cancelada_os, retrabalho, recebido_setor_data, concluida_os_data, impresso, cod_detalhe_compl, id_modelo,
                             alterado_por, alterado_data, status, data_status, status_por, motivo_cancelamento, aprovado, aprovado_por, aprovado_em,
                             programacao_ordem, programacao_status, programacao_observacao, programacao_inserido_por, programacao_inserido_data,
                             meta_lider, pagina, pt)
                        VALUES
                            (@num_os_produto, @tipo, @codigo_setor, @setor_caminho, @quantidade, @data_inicio, @data_fim, @cliente, @tema, @orientacao_caminho,
                             @codigo_setor_proximo, @setor_caminho_proximo, @fase, @responsavel_emissao_os, @emitida_por, @emitida_data, @meta_data, @turno,
                             @ajuste_projeto, @cancelada_os, @retrabalho, @recebido_setor_data, @concluida_os_data, @impresso, @cod_detalhe_compl, @id_modelo,
                             @alterado_por, @alterado_data, @status, @data_status, @status_por, @motivo_cancelamento, @aprovado, @aprovado_por, @aprovado_em,
                             @programacao_ordem, @programacao_status, @programacao_observacao, @programacao_inserido_por, @programacao_inserido_data,
                             @meta_lider, @pagina, @pt)
                        RETURNING num_os_servico;
                        """,
                        ProdutoServico);
                }
                else
                {
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.tbl_produtos_servico
                        SET num_os_produto = @num_os_produto,
                            tipo = @tipo,
                            codigo_setor = @codigo_setor,
                            setor_caminho = @setor_caminho,
                            quantidade = @quantidade,
                            data_inicio = @data_inicio,
                            data_fim = @data_fim,
                            cliente = @cliente,
                            tema = @tema,
                            orientacao_caminho = @orientacao_caminho,
                            codigo_setor_proximo = @codigo_setor_proximo,
                            setor_caminho_proximo = @setor_caminho_proximo,
                            fase = @fase,
                            responsavel_emissao_os = @responsavel_emissao_os,
                            emitida_por = @emitida_por,
                            emitida_data = @emitida_data,
                            meta_data = @meta_data,
                            turno = @turno,
                            ajuste_projeto = @ajuste_projeto,
                            cancelada_os = @cancelada_os,
                            retrabalho = @retrabalho,
                            recebido_setor_data = @recebido_setor_data,
                            concluida_os_data = @concluida_os_data,
                            impresso = @impresso,
                            cod_detalhe_compl = @cod_detalhe_compl,
                            id_modelo = @id_modelo,
                            alterado_por = @alterado_por,
                            alterado_data = @alterado_data,
                            status = @status,
                            data_status = @data_status,
                            status_por = @status_por,
                            motivo_cancelamento = @motivo_cancelamento,
                            aprovado = @aprovado,
                            aprovado_por = @aprovado_por,
                            aprovado_em = @aprovado_em,
                            programacao_ordem = @programacao_ordem,
                            programacao_status = @programacao_status,
                            programacao_observacao = @programacao_observacao,
                            programacao_inserido_por = @programacao_inserido_por,
                            programacao_inserido_data = @programacao_inserido_data,
                            meta_lider = @meta_lider,
                            pagina = @pagina,
                            pt = @pt
                        WHERE num_os_servico = @num_os_servico;
                        """,
                        ProdutoServico);
                }

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
                const string sql = """
                    SELECT *
                    FROM producao.tbl_produtos_servico
                    WHERE cod_detalhe_compl = @coddetalhescompl
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<ProdutoServicoModel>(sql, new { coddetalhescompl });
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TGlobalModel> GetTGlobalAsync(long? num_os)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM ht.t_global
                    WHERE num_os = @num_os
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<TGlobalModel>(sql, new { num_os });
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
                const string sql = """
                    SELECT *
                    FROM producao.tbl_requisicao_receita
                    WHERE codcompladicional_produto = @codcompladicional;
                    """;
                var data = await QueryAsync<RequisicaoReceitaModel>(sql, new { codcompladicional });
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
                await using var conn = CreateConnection();
                if (Requisicao.num_requisicao is null or 0)
                {
                    Requisicao.num_requisicao = await conn.ExecuteScalarAsync<long>(
                        """
                        INSERT INTO producao.t_requisicao
                            (num_os_servico, data, alterado_por, concluida)
                        VALUES
                            (@num_os_servico, @data, @alterado_por, @concluida)
                        RETURNING num_requisicao;
                        """,
                        Requisicao);
                }
                else
                {
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.t_requisicao
                        SET num_os_servico = @num_os_servico,
                            data = @data,
                            alterado_por = @alterado_por,
                            concluida = @concluida
                        WHERE num_requisicao = @num_requisicao;
                        """,
                        Requisicao);
                }

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
                await using var conn = CreateConnection();
                if (RequisicaoDetalhe.cod_det_req is null or 0)
                {
                    RequisicaoDetalhe.cod_det_req = await conn.ExecuteScalarAsync<long>(
                        """
                        INSERT INTO producao.t_detalhes_req
                            (num_requisicao, quantidade, data, alterado_por, ok, data_ok, ok_expedido, observacao, voltagem, local_shop, complemento_chk, codcompladicional, volume, dividir_qtd_volume, agupar)
                        VALUES
                            (@num_requisicao, @quantidade, @data, @alterado_por, @ok, @data_ok, @ok_expedido, @observacao, @voltagem, @local_shop, @complemento_chk, @codcompladicional, @volume, @dividir_qtd_volume, @agupar)
                        RETURNING cod_det_req;
                        """,
                        RequisicaoDetalhe);
                }
                else
                {
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.t_detalhes_req
                        SET num_requisicao = @num_requisicao,
                            quantidade = @quantidade,
                            data = @data,
                            alterado_por = @alterado_por,
                            ok = @ok,
                            data_ok = @data_ok,
                            ok_expedido = @ok_expedido,
                            observacao = @observacao,
                            voltagem = @voltagem,
                            local_shop = @local_shop,
                            complemento_chk = @complemento_chk,
                            codcompladicional = @codcompladicional,
                            volume = @volume,
                            dividir_qtd_volume = @dividir_qtd_volume,
                            agupar = @agupar
                        WHERE cod_det_req = @cod_det_req;
                        """,
                        RequisicaoDetalhe);
                }

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
                const string sql = """
                    SELECT *
                    FROM producao.t_requisicao
                    WHERE num_os_servico = @num_os_servico
                    ORDER BY num_requisicao DESC
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<RequisicaoModel>(sql, new { num_os_servico });
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
                const string sql = """
                    SELECT *
                    FROM producao.qry_req_detalhes_relatorio_os_chk
                    WHERE num_requisicao = @num_requisicao;
                    """;
                var data = await QueryAsync<QryRequisicaoDetalheModel>(sql, new { num_requisicao });
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
                const string sql = """
                    SELECT *
                    FROM producao.view_sigla_chkgeral
                    ORDER BY sigla_serv;
                    """;
                var data = await QueryAsync<SiglaChkListModel>(sql);
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
                const string sql = """
                    SELECT *
                    FROM producao.relplan
                    WHERE ativo = '1'
                      AND planilha NOT LIKE '%ESTOQUE%'
                      AND planilha NOT LIKE '%ALMOX%'
                    ORDER BY planilha;
                    """;
                var data = await QueryAsync<RelplanModel>(sql);
                return new ObservableCollection<RelplanModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<RelplanModel>> GetTodasPlanilhasAsync()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.relplan
                    WHERE ativo = '1'
                    ORDER BY planilha;
                    """;
                var data = await QueryAsync<RelplanModel>(sql);
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
                const string sql = """
                    SELECT *
                    FROM producao.produtos
                    WHERE planilha = @planilha
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY descricao;
                    """;
                var data = await QueryAsync<ProdutoModel>(sql, new { planilha });
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
                const string sql = """
                    SELECT *
                    FROM producao.tabela_desc_adicional
                    WHERE codigoproduto = @codigo
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY descricao_adicional;
                    """;
                var data = await QueryAsync<TabelaDescAdicionalModel>(sql, new { codigo });

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
                const string sql = """
                    SELECT *
                    FROM producao.tblcomplementoadicional
                    WHERE coduniadicional = @coduniadicional
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY complementoadicional;
                    """;
                var data = await QueryAsync<TblComplementoAdicionalModel>(sql, new { coduniadicional });

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
                const string sql = """
                    SELECT DISTINCT local_shoppings
                    FROM producao.t_complemento_chk
                    WHERE id_aprovado = @id_aprovado
                    ORDER BY local_shoppings;
                    """;
                var data = await QueryAsync<LocalShoppingRow>(sql, new { id_aprovado });
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
                const string sql = """
                    SELECT *
                    FROM producao.qrychkgeral
                    WHERE id_aprovado = @id_aprovado
                      AND kp IS NULL
                    ORDER BY id;
                    """;
                var data = await QueryAsync<QryCheckListGeralModel>(sql, new { id_aprovado });
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
                await using var conn = CreateConnection();
                if (ComplementoCheckList.codcompl is null)
                {
                    ComplementoCheckList.codcompl = await conn.ExecuteScalarAsync<long>(
                        """
                        INSERT INTO producao.t_complemento_chk
                            (ordem, sigla, local_shoppings, codproduto, obs, dataalteracaodesc, alteradopor, orient_montagem, item_memorial, datainclusaodesc,
                             incluidopordesc, kp, kp2, orient_desmont, qtd, coduniadicional, dataalteradescadic, alteradopordescadic, nivel, carga,
                             class_solucao, id_aprovado, historico, agrupar, motivos, inserido_por, inserido_em, alterado_por, alterado_em)
                        VALUES
                            (@ordem, @sigla, @local_shoppings, @codproduto, @obs, @dataalteracaodesc, @alteradopor, @orient_montagem, @item_memorial, @datainclusaodesc,
                             @incluidopordesc, @kp, @kp2, @orient_desmont, @qtd, @coduniadicional, @dataalteradescadic, @alteradopordescadic, @nivel, @carga,
                             @class_solucao, @id_aprovado, @historico, @agrupar, @motivos, @inserido_por, @inserido_em, @alterado_por, @alterado_em)
                        RETURNING codcompl;
                        """,
                        ComplementoCheckList);
                }
                else
                {
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.t_complemento_chk
                        SET ordem = @ordem,
                            sigla = @sigla,
                            local_shoppings = @local_shoppings,
                            codproduto = @codproduto,
                            obs = @obs,
                            dataalteracaodesc = @dataalteracaodesc,
                            alteradopor = @alteradopor,
                            orient_montagem = @orient_montagem,
                            item_memorial = @item_memorial,
                            datainclusaodesc = @datainclusaodesc,
                            incluidopordesc = @incluidopordesc,
                            kp = @kp,
                            kp2 = @kp2,
                            orient_desmont = @orient_desmont,
                            qtd = @qtd,
                            coduniadicional = @coduniadicional,
                            dataalteradescadic = @dataalteradescadic,
                            alteradopordescadic = @alteradopordescadic,
                            nivel = @nivel,
                            carga = @carga,
                            class_solucao = @class_solucao,
                            id_aprovado = @id_aprovado,
                            historico = @historico,
                            agrupar = @agrupar,
                            motivos = @motivos,
                            inserido_por = @inserido_por,
                            inserido_em = @inserido_em,
                            alterado_por = @alterado_por,
                            alterado_em = @alterado_em
                        WHERE codcompl = @codcompl;
                        """,
                        ComplementoCheckList);
                }

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
                if (compChkList != null)
                {
                    await using var conn = CreateConnection();
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.t_complemento_chk
                        SET obs = CASE WHEN @obs <> '' THEN @obs ELSE obs END,
                            orient_montagem = CASE WHEN @orient_montagem <> '' THEN @orient_montagem ELSE orient_montagem END,
                            orient_desmont = CASE WHEN @orient_desmont <> '' THEN @orient_desmont ELSE orient_desmont END,
                            ordem = CASE WHEN @ordem <> '' THEN @ordem ELSE ordem END,
                            local_shoppings = CASE WHEN @local_shoppings <> '' THEN @local_shoppings ELSE local_shoppings END,
                            item_memorial = CASE WHEN @item_memorial <> '' THEN @item_memorial ELSE item_memorial END,
                            alterado_por = CASE WHEN @alterado_por <> '' THEN @alterado_por ELSE alterado_por END,
                            alterado_em = COALESCE(@alterado_em, alterado_em)
                        WHERE codcompl = @codcompl;
                        """,
                        compChkList);
                }
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task CargaCaminhaoListAsync(ComplementoCheckListModel compChkList)
        {
            try
            {
                if (compChkList != null)
                {
                    if (compChkList.carga != "")
                    {
                        await using var conn = CreateConnection();
                        await conn.ExecuteAsync(
                            """
                            UPDATE producao.t_complemento_chk
                            SET carga = @carga
                            WHERE codcompl = @codcompl;
                            """,
                            compChkList);
                    }
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
                const string sql = """
                    SELECT *
                    FROM producao.qrychkgeral
                    WHERE codcompl = @CodCompl
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<QryCheckListGeralModel>(sql, new { CodCompl });
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
                const string sql = """
                    SELECT *
                    FROM producao.qrychkgeral_complemento
                    WHERE codcompl = @codcompl
                    ORDER BY coddetalhescompl;
                    """;
                var data = await QueryAsync<QryCheckListGeralComplementoModel>(sql, new { codcompl });

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
                await using var conn = CreateConnection();
                if (detCompl.coddetalhescompl is null or 0)
                {
                    detCompl.coddetalhescompl = await conn.ExecuteScalarAsync<long>(
                        """
                        INSERT INTO producao.tbldetalhescomplemento
                            (codcompladicional, qtd, data_alteracao, alterado_por, codcompl, id_modelo, confirmado, os, req, transf, local_producao,
                             justificativa, producao, supermercado, enviado_baia, obs_planilheiro, resp_prod, confirmado_por, confirmado_data,
                             desabilitado_confirmado_por, desabilitado_confirmado_data, transf_galpao, terceiro, em_producao, meta_producao,
                             num_os_produto, data_inserido, inserido_por, status_producao, status_transferencia, status_atualizado_por, status_atualizado_em)
                        VALUES
                            (@codcompladicional, @qtd, @data_alteracao, @alterado_por, @codcompl, @id_modelo, @confirmado, @os, @req, @transf, @local_producao,
                             @justificativa, @producao, @supermercado, @enviado_baia, @obs_planilheiro, @resp_prod, @confirmado_por, @confirmado_data,
                             @desabilitado_confirmado_por, @desabilitado_confirmado_data, @transf_galpao, @terceiro, @em_producao, @meta_producao,
                             @num_os_produto, @data_inserido, @inserido_por, @status_producao, @status_transferencia, @status_atualizado_por, @status_atualizado_em)
                        RETURNING coddetalhescompl;
                        """,
                        detCompl);
                }
                else
                {
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.tbldetalhescomplemento
                        SET codcompladicional = @codcompladicional,
                            qtd = @qtd,
                            data_alteracao = @data_alteracao,
                            alterado_por = @alterado_por,
                            codcompl = @codcompl,
                            id_modelo = @id_modelo,
                            confirmado = @confirmado,
                            os = @os,
                            req = @req,
                            transf = @transf,
                            local_producao = @local_producao,
                            justificativa = @justificativa,
                            producao = @producao,
                            supermercado = @supermercado,
                            enviado_baia = @enviado_baia,
                            obs_planilheiro = @obs_planilheiro,
                            resp_prod = @resp_prod,
                            confirmado_por = @confirmado_por,
                            confirmado_data = @confirmado_data,
                            desabilitado_confirmado_por = @desabilitado_confirmado_por,
                            desabilitado_confirmado_data = @desabilitado_confirmado_data,
                            transf_galpao = @transf_galpao,
                            terceiro = @terceiro,
                            em_producao = @em_producao,
                            meta_producao = @meta_producao,
                            num_os_produto = @num_os_produto,
                            data_inserido = @data_inserido,
                            inserido_por = @inserido_por,
                            status_producao = @status_producao,
                            status_transferencia = @status_transferencia,
                            status_atualizado_por = @status_atualizado_por,
                            status_atualizado_em = @status_atualizado_em
                        WHERE coddetalhescompl = @coddetalhescompl;
                        """,
                        detCompl);
                }

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
                var det = await QueryFirstOrDefaultAsync<DetalhesComplemento>(
                    """
                    SELECT *
                    FROM producao.tbldetalhescomplemento
                    WHERE coddetalhescompl = @coddetalhescompl
                    LIMIT 1;
                    """,
                    new { detCompl.coddetalhescompl });
                if (det != null)
                {
                    det.confirmado = detCompl.confirmado;
                    det.confirmado_data = detCompl.confirmado_data;
                    det.confirmado_por = detCompl.confirmado_por;
                    det.desabilitado_confirmado_data = detCompl.desabilitado_confirmado_data;
                    det.desabilitado_confirmado_por = detCompl.desabilitado_confirmado_por;

                    await using var conn = CreateConnection();
                    await conn.ExecuteAsync(
                        """
                        UPDATE producao.tbldetalhescomplemento
                        SET confirmado = @confirmado,
                            confirmado_data = @confirmado_data,
                            confirmado_por = @confirmado_por,
                            desabilitado_confirmado_data = @desabilitado_confirmado_data,
                            desabilitado_confirmado_por = @desabilitado_confirmado_por
                        WHERE coddetalhescompl = @coddetalhescompl;
                        """,
                        det);
                }

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
                const string sql = """
                    SELECT
                        item_memorial,
                        local_shoppings,
                        planilha,
                        descricao_dd,
                        unidade,
                        qtd,
                        custo_unitario,
                        custo_total,
                        orient_montagem,
                        coddetalhescompl,
                        caminhao
                    FROM producao.qrychkgeral_relatorio
                    WHERE id_aprovado = @id_aprovado
                    ORDER BY ordem;
                    """;
                await using var conn = CreateConnection();
                var data = (await conn.QueryAsync(sql, new { id_aprovado })).ToList();
                return data;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteCheckListAsync(long codcompl)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                await conn.ExecuteAsync(
                    "DELETE FROM producao.tbldetalhescomplemento WHERE codcompl = @codcompl;",
                    new { codcompl },
                    transaction);

                await conn.ExecuteAsync(
                    "DELETE FROM producao.t_complemento_chk WHERE codcompl = @codcompl;",
                    new { codcompl },
                    transaction);

                await transaction.CommitAsync();
            }
            catch (NpgsqlException)
            {
                await transaction.RollbackAsync();
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

