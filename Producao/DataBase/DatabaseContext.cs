using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using System;

namespace Producao
{
    public class DatabaseContext : DbContext
    {
        private DataBaseSettings BaseSettings  = DataBaseSettings.Instance;
        public DbSet<RelplanModel> Relplans { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<TabelaDescAdicionalModel> DescAdicionais { get; set; }
        public DbSet<TblComplementoAdicionalModel> ComplementoAdicionais { get; set; }
        public DbSet<ProdutoShoppingModel> ProdutoShopping { get; set; }
        public DbSet<AprovadoModel> Aprovados { get; set; }
        public DbSet<SiglaChkListModel> Siglas { get; set; }
        public DbSet<ComplementoCheckListModel> ComplementoCheckLists { get; set; }
        public DbSet<QryCheckListGeralModel> CheckListGerals { get; set; }
        public DbSet<HistoricoCheckListModel> HistoricoCheckList { get; set; }
        public DbSet<DetalhesComplemento> DetalhesComplementos { get; set; }
        public DbSet<QryCheckListGeralComplementoModel> CheckListGeralComplementos { get; set; }
        public DbSet<ChkGeralRelatorioModel> ChkGeralRelatorios { get; set; }
        public DbSet<PendenciaProducaoModel> PendenciaProducaos { get; set; }
        public DbSet<DetalhesPendenciaProducaoModel> DetalhesPendenciaProducao { get; set; }
        public DbSet<ControlePlanilhaGrupoModel> ControlePlanilhaGrupos { get; set; }
        public DbSet<SetorProducaoModel> SetorProducaos { get; set; }
        public DbSet<ObsOsModel> ObsOs { get; set; }
        public DbSet<TblServicoModel> tblServicos { get; set; }
        public DbSet<TblTipoOs> tblTipoOs { get; set; }
        public DbSet<ProdutoOsModel> ProdutoOs { get; set; }
        public DbSet<ProdutoServicoModel> ProdutoServicos { get; set; }
        public DbSet<TGlobalModel> Globais { get; set; }
        public DbSet<OrdemServicoAbertaEmissaoAgrupadoModel> OrdemServicoAbertas { get; set; }
        public DbSet<OrdemServicoEmissaoAbertaForm> OrdemServicoEmissaoAbertas { get; set; }
        public DbSet<AlteraSolicitacaoOsProducao> AlteraSolicitacaoOsProducaos { get; set; }
        public DbSet<RequisicaoModel> Requisicoes { get; set; }
        public DbSet<DetalheRequisicaoModel> RequisicaoDetalhes { get; set; }
        public DbSet<RequisicaoReceitaModel> RequisicaoReceitas { get; set; }
        public DbSet<GeralRequisicaoProducaoModel> RequisicoesProducao { get; set; }
        public DbSet<QryRequisicaoDetalheModel> QryRequisicaoDetalhes { get; set; }
        public DbSet<ReqDetalhesModel> ReqDetalhes { get; set; }
        public DbSet<QryDescricao> Descricoes { get; set; }
        public DbSet<ControleMemorialModel> ControleMemorials { get; set; }
        public DbSet<RevisorModel> Revisores { get; set; }
        public DbSet<BaseAnaliseClienteModel> BaseAnaliseClientes { get; set; }
        public DbSet<BaseAnalisePlanModel> BaseAnalisePlans { get; set; }
        public DbSet<EtiquetaCheckListModel> EtiquetaCheckLists { get; set; }
        public DbSet<EtiquetaProducaoModel> EtiquetaProducaos { get; set; }
        public DbSet<EtiquetaEmitidaModel> EtiquetaEmitidas { get; set; }
        public DbSet<QryModeloModel> qryModelos { get; set; }
        public DbSet<ModeloModel> Modelos { get; set; }
        public DbSet<ModeloFiadaModel> ModelosFiada { get; set; }
        public DbSet<QryReceitaDetalheCriadoModel> qryReceitas  { get; set; }
        public DbSet<ModeloReceitaModel> ReceitaModelos  { get; set; }
        public DbSet<ModeloReceitaAnoAnterior> ModelosAnoAnterior { get; set; }
        public DbSet<ControleModeloBaixa> ModelosControle { get; set; }
        public DbSet<HistoricoModelo> HistoricosModelo { get; set; }
        public DbSet<DetalhesModeloModel> DetalhesModelo { get; set; }
        public DbSet<ModeloTabelaPAModel> TabelaPAs { get; set; }
        public DbSet<ModeloTabelaConversaoModel> TabelaConversoes { get; set; }
        public DbSet<ModeloControleOsModel> modeloControleOs { get; set; }
        public DbSet<ModeloGerarOsModel> ModeloGerarOs { get; set; }
        public DbSet<HistoricoModeloCompletaModel> HistoricoModeloCompletas { get; set; }
        public DbSet<DetalhesModeloFitasModel> DetalhesModeloFitas { get; set; }
        public DbSet<ExportEnfeitesInfClienteModel> exportEnfeitesInfClientes { get; set; }
        public DbSet<HistoricoSetorModel> HistoricosSetor { get; set; }
        public DbSet<DistribuicaoPAModel> DistribuicaoPAs { get; set; }
        public DbSet<OsEmissaoProducaoImprimirModel> ImprimirOsS { get; set; }
        public DbSet<StatusChkGeralCentralModel> statusChkGeralCentrals { get; set; }
        public DbSet<ClasseSolicitCompra> ClasseSolicitCompras { get; set; }
        public DbSet<UnidadeModel> Unidades { get; set; }
        public DbSet<TemaModel> Temas { get; set; }
        public DbSet<FamiliaProdModel> FamiliaProds { get; set; }
        public DbSet<EntradaEstoqueModel> Entradas { get; set; }
        public DbSet<SaidaEstoqueModel> Saidas { get; set; }
        public DbSet<ControleAcertoEstoque> ControleAcertoEstoques { get; set; }
        public DbSet<BaixaEstoqueRequisicaoModel> BaixaEstoqueRequisicoes { get; set; }
        public DbSet<PlanejamentoEstoqueModel> PlanejamentoEstoques { get; set; }
        public DbSet<SaldoDetalhadoModel> SaldoDetalhados { get; set; }
        public DbSet<BarcodeModel> Barcodes { get; set; }
        public DbSet<ContaProcessSemanaModel> ContaProcessSemanas { get; set; }
        public DbSet<DetalhesProcessamentoSemanaModel> DetalhesProcessamentoSemanas { get; set; }
        public DbSet<ChklistNaoCompletadoModel> ChklistNaoCompletados { get; set; }
        public DbSet<BaixaOsProducaoModel> BaixaOsProducoes { get; set; }
        public DbSet<OrdemServicoEmitidaModel> OrdemServicoEmitidas { get; set; }
        public DbSet<ProgramacaoProducaoModel> ProgramacaoProducoes { get; set; }
        public DbSet<ControladoEtiquetaModel> ControladoEtiquetas { get; set; }
        public DbSet<TransformaRequisicaoModel> TransformaRequisicoes { get; set; }
        public DbSet<EtiquetaZebraModel> EtiquetasZebra { get; set; }
        public DbSet<ControladoShoppingModel> ControladoShoppings { get; set; }
        public DbSet<PlanilhaConstrucaoModel> PlanilhasConstrucao { get; set; }
        public DbSet<ConstrucaoDetalheModel> ConstrucaoDetalhes { get; set; }
        public DbSet<ConstrucaoPecaModel> ConstrucaoPecas { get; set; }
        public DbSet<ChecklistPrdutoConstrucaoModel> ChecklistPrdutoConstrucaos { get; set; }
        public DbSet<ChecklistPrdutoRequisicaoModel> ChecklistPrdutooRequisicoes { get; set; }
        public DbSet<PropostaFechaSiglaModel> PropostaFechaSiglas { get; set; }
        public DbSet<PropostaFechaTemaModel> PropostaFechaTemas { get; set; }
        public DbSet<ViewFechaModel> ViewFechas { get; set; }
        public DbSet<FechaLinkModel> FechaLinks { get; set; }
        public DbSet<OsKitSolucaoModel> OsKitSolucaos { get; set; }
        public DbSet<ClassificacaoSolucaoModel> ClassificacaoSolucaos { get; set; }
        public DbSet<KitChkGeralModel> KitChkGerals { get; set; }
        public DbSet<KitSolicaoGeralModel> KitSolicaoGeral { get; set; }
        public DbSet<ControleSolicaoGeralModel> ControleSolicaoGeral { get; set; }
        public DbSet<ControleEnvioModel> ControleEnvio { get; set; }
        public DbSet<ControladoRetornoGeralModel> ControladoRetornoGeral { get; set; }
        public DbSet<ControladoRecebidoModel> ControladoRecebido { get; set; }
        public DbSet<ProdutoControladoRecebimentoModel> ProdutoControladoRecebimento { get; set; }

        
        static DatabaseContext() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*
            optionsBuilder.UseNpgsql(
                $"host={BaseSettings.Host};" +
                $"user id={BaseSettings.Username};" +
                $"password={BaseSettings.Password};" +
                $"database={BaseSettings.Database};" //+
                //$"Pooling=false;" +
                //$"Timeout=300;" +
                //$"CommandTimeout=300;"
                );
            */
            optionsBuilder.UseNpgsql(
                $"host={BaseSettings.Host};" +
                $"user id={BaseSettings.Username};" +
                $"password={BaseSettings.Password};" +
                $"database={BaseSettings.Database};" +
                $"Application Name=SIG Producao <{BaseSettings.Database}>;" ,
                options => { options.EnableRetryOnFailure(); }
                );
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ControladoShoppingModel>()
                .HasKey(a => new { a.num_requisicao, a.barcode });

            modelBuilder.Entity<ConstrucaoPecaModel>()
                .HasKey(a => new { a.id_detalhes, a.codcompladicional });

            modelBuilder.Entity<ControladoRecebidoModel>()
                .HasKey(a => new { a.id_aprovado, a.codcompladicional });
        }
    }
}
