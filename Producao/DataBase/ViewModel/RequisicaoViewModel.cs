using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Producao
{
    class RequisicaoViewModel : INotifyPropertyChanged
    {
        static RequisicaoViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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

        private ObservableCollection<ProdutoModel> _produtos;
        public ObservableCollection<ProdutoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }
        private ProdutoModel _produto;
        public ProdutoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _descAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
        {
            get { return _descAdicionais; }
            set { _descAdicionais = value; RaisePropertyChanged("DescAdicionais"); }
        }
        private TabelaDescAdicionalModel _descAdicional;
        public TabelaDescAdicionalModel DescAdicional
        {
            get { return _descAdicional; }
            set { _descAdicional = value; RaisePropertyChanged("DescAdicional"); }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _compleAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
        {
            get { return _compleAdicionais; }
            set { _compleAdicionais = value; RaisePropertyChanged("CompleAdicionais"); }
        }
        private TblComplementoAdicionalModel _compledicional;
        public TblComplementoAdicionalModel Compledicional
        {
            get { return _compledicional; }
            set { _compledicional = value; RaisePropertyChanged("Compledicional"); }
        }

        private TGlobalModel _tGlobal;
        public TGlobalModel TGlobal
        {
            get { return _tGlobal; }
            set { _tGlobal = value; RaisePropertyChanged("TGlobal"); }
        }

        private ProdutoServicoModel _produtoServico;
        public ProdutoServicoModel ProdutoServico
        {
            get { return _produtoServico; }
            set { _produtoServico = value; RaisePropertyChanged("ProdutoServico"); }
        }
        private ObservableCollection<ProdutoServicoModel> _produtoServicos;
        public ObservableCollection<ProdutoServicoModel> ProdutoServicos
        {
            get { return _produtoServicos; }
            set { _produtoServicos = value; RaisePropertyChanged("ProdutoServicos"); }
        }

        private RequisicaoModel _requisicao;
        public RequisicaoModel Requisicao
        {
            get { return _requisicao; }
            set { _requisicao = value; RaisePropertyChanged("Requisicao"); }
        }
        private ObservableCollection<RequisicaoModel> _requisicoes;
        public ObservableCollection<RequisicaoModel> Requisicoes
        {
            get { return _requisicoes; }
            set { _requisicoes = value; RaisePropertyChanged("Requisicoes"); }
        }

        private DetalheRequisicaoModel _requisicaoDetalhe;
        public DetalheRequisicaoModel RequisicaoDetalhe
        {
            get { return _requisicaoDetalhe; }
            set { _requisicaoDetalhe = value; RaisePropertyChanged("RequisicaoDetalhe"); }
        }
        private ObservableCollection<DetalheRequisicaoModel> _requisicaoDetalhes;
        public ObservableCollection<DetalheRequisicaoModel> RequisicaoDetalhes
        {
            get { return _requisicaoDetalhes; }
            set { _requisicaoDetalhes = value; RaisePropertyChanged("RequisicaoDetalhes"); }
        }

        private QryRequisicaoDetalheModel _qryRequisicaoDetalhe;
        public QryRequisicaoDetalheModel QryRequisicaoDetalhe
        {
            get { return _qryRequisicaoDetalhe; }
            set { _qryRequisicaoDetalhe = value; RaisePropertyChanged("QryRequisicaoDetalhe"); }
        }
        private ObservableCollection<QryRequisicaoDetalheModel> _qryRequisicaoDetalhes;
        public ObservableCollection<QryRequisicaoDetalheModel> QryRequisicaoDetalhes
        {
            get { return _qryRequisicaoDetalhes; }
            set { _qryRequisicaoDetalhes = value; RaisePropertyChanged("QryRequisicaoDetalhes"); }
        }

        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }
        private ObservableCollection<QryDescricao> _descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return _descricoes; }
            set { _descricoes = value; RaisePropertyChanged("Descricoes"); }
        }

        private ObservableCollection<ReqDetalhesModel> _reqDetalhes;
        public ObservableCollection<ReqDetalhesModel> ReqDetalhes
        {
            get { return _reqDetalhes; }
            set { _reqDetalhes = value; RaisePropertyChanged("ReqDetalhes"); }
        }

        public RequisicaoViewModel()
        {
            Requisicao = new RequisicaoModel();
        }

        public async Task<QryDescricao> GetDescricaoAsync(long codcompladicional)
        {//e.FirstName.StartsWith(employeeName) || e.LastName.StartsWith(employeeName)
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.qry3descricoes
                    WHERE inativo = '0'
                      AND codcompladicional = @codcompladicional
                    LIMIT 1;
                    """;
                return await QueryFirstOrDefaultAsync<QryDescricao>(sql, new { codcompladicional });
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
                    ORDER BY planilha;
                    """;
                return new ObservableCollection<RelplanModel>(await QueryAsync<RelplanModel>(sql));
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
                Produtos = new ObservableCollection<ProdutoModel>();
                const string sql = """
                    SELECT *
                    FROM producao.produtos
                    WHERE planilha = @planilha
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY descricao;
                    """;
                var data = await QueryAsync<ProdutoModel>(sql, new { planilha });
                //Produtos = new ObservableCollection<ProdutoModel>(data);

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
                DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                const string sql = """
                    SELECT *
                    FROM producao.tabela_desc_adicional
                    WHERE codigoproduto = @codigo
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY descricao_adicional;
                    """;
                var data = await QueryAsync<TabelaDescAdicionalModel>(sql, new { codigo });
                //DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>(data);
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
                //CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>(data);
                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RequisicaoModel> GetByRequisicaoAsync(long? num_requisicao)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.t_requisicao
                    WHERE num_requisicao = @num_requisicao
                    LIMIT 1;
                    """;
                return await QueryFirstOrDefaultAsync<RequisicaoModel>(sql, new { num_requisicao });
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
                return await QueryFirstOrDefaultAsync<RequisicaoModel>(sql, new { num_os_servico });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DetalheRequisicaoModel> AddProdutoRequisicaoAsync(DetalheRequisicaoModel requisicaoDetalhe)
        {
            try
            {
                await using var conn = CreateConnection();
                if (requisicaoDetalhe.cod_det_req is null or 0)
                {
                    const string insertSql = """
                        INSERT INTO producao.t_detalhes_req
                            (num_requisicao, quantidade, data, alterado_por, ok, data_ok, ok_expedido, observacao, voltagem, local_shop, complemento_chk, codcompladicional, volume, dividir_qtd_volume, agupar)
                        VALUES
                            (@num_requisicao, @quantidade, @data, @alterado_por, @ok, @data_ok, @ok_expedido, @observacao, @voltagem, @local_shop, @complemento_chk, @codcompladicional, @volume, @dividir_qtd_volume, @agupar)
                        RETURNING cod_det_req;
                        """;
                    requisicaoDetalhe.cod_det_req = await conn.ExecuteScalarAsync<long>(insertSql, requisicaoDetalhe);
                }
                else
                {
                    const string updateSql = """
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
                        """;
                    await conn.ExecuteAsync(updateSql, requisicaoDetalhe);
                }

                return requisicaoDetalhe;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ReqDetalhesModel>> GetByRequisicaoDetalhesAsync(long? num_requisicao)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qry_req_detalhes
                    WHERE num_requisicao = @num_requisicao
                    ORDER BY volume;
                    """;
                var data = await QueryAsync<ReqDetalhesModel>(sql, new { num_requisicao });
                return new ObservableCollection<ReqDetalhesModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DetalheRequisicaoModel> GetItemRequisicaoAsync(long? codDetReq)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.t_detalhes_req
                    WHERE cod_det_req = @codDetReq
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<DetalheRequisicaoModel>(sql, new { codDetReq });
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

        public async Task GravarItensReceitaAsync(long? num_requisicao)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            RequisicaoModel requisicao = await conn.QueryFirstOrDefaultAsync<RequisicaoModel>(
                "SELECT * FROM producao.t_requisicao WHERE num_requisicao = @num_requisicao LIMIT 1;",
                new { num_requisicao });
            ProdutoServicoModel produtoServico = await conn.QueryFirstOrDefaultAsync<ProdutoServicoModel>(
                "SELECT * FROM producao.tbl_produtos_servico WHERE num_os_servico = @num_os_servico LIMIT 1;",
                new { requisicao.num_os_servico });
            ProdutoOsModel produtoOs = await conn.QueryFirstOrDefaultAsync<ProdutoOsModel>(
                "SELECT * FROM producao.tbl_produto_os WHERE num_os_produto = @num_os_produto LIMIT 1;",
                new { produtoServico.num_os_produto });
            double? quantidade = 0;
            if (produtoServico.cod_detalhe_compl != null)
            {
                ComplementoCheckListModel? complementoCheckList = await conn.QueryFirstOrDefaultAsync<ComplementoCheckListModel>(
                    "SELECT * FROM producao.t_complemento_chk WHERE codcompl = @cod_detalhe_compl LIMIT 1;",
                    new { produtoServico.cod_detalhe_compl });
                quantidade = complementoCheckList.qtd;
            }
            else 
            {
                quantidade = produtoServico.quantidade;
            }

            if (produtoOs.cod_desc_adicional != null) 

            await using (var transaction = await conn.BeginTransactionAsync())
            {
                try
                {

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
                            quantidade = item.quantidade * quantidade,
                            data = DateTime.Now,
                            alterado_por = Environment.UserName
                        };

                        var encontrado = await conn.QueryFirstOrDefaultAsync<DetalheRequisicaoModel>(
                            """
                            SELECT *
                            FROM producao.t_detalhes_req
                            WHERE num_requisicao = @num_requisicao
                              AND codcompladicional = @codcompladicional
                            LIMIT 1;
                            """,
                            new { ReqDetalhe.num_requisicao, ReqDetalhe.codcompladicional },
                            transaction);
                        if (encontrado == null)
                        {
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
                                   
                        
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<ChecklistPrdutoRequisicaoModel> GetPrdutoRequisicaoAsync(long? num_requisicao)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.qry_checklist_prduto_requisicao
                    WHERE num_requisicao = @num_requisicao
                    LIMIT 1;
                    """;
                var data = await QueryFirstOrDefaultAsync<ChecklistPrdutoRequisicaoModel>(sql, new { num_requisicao });
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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}

