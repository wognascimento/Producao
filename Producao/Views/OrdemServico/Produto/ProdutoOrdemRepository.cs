using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producao.Views.OrdemServico.Produto
{
    internal static class ProdutoOrdemRepository
    {
        static ProdutoOrdemRepository() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        public static Task<List<RelplanModel>> GetPlanilhasAsync()
        {
            const string sql = """
                SELECT *
                FROM producao.relplan
                WHERE ativo = '1'
                ORDER BY planilha;
                """;

            return QueryAsync<RelplanModel>(sql);
        }

        public static Task<List<SetorModel>> GetSetoresAsync()
        {
            const string sql = """
                SELECT
                    setor || ' - ' || galpao AS setor,
                    codigo_setor
                FROM producao.tbl_setor
                WHERE inativo = '0    '
                ORDER BY setor, galpao;
                """;

            return QueryAsync<SetorModel>(sql);
        }

        public static Task<List<SiglaChkListModel>> GetSiglasAsync()
        {
            const string sql = """
                SELECT *
                FROM producao.view_sigla_chkgeral
                ORDER BY sigla_serv;
                """;

            return QueryAsync<SiglaChkListModel>(sql);
        }

        public static Task<List<ProdutoModel>> GetProdutosAsync(string? planilha)
        {
            const string sql = """
                SELECT *
                FROM producao.produtos
                WHERE planilha = @planilha
                  AND COALESCE(inativo, '') <> '-1'
                ORDER BY descricao;
                """;

            return QueryAsync<ProdutoModel>(sql, new { planilha });
        }

        public static Task<List<TabelaDescAdicionalModel>> GetDescAdicionaisAsync(long? codigo)
        {
            const string sql = """
                SELECT *
                FROM producao.tabela_desc_adicional
                WHERE codigoproduto = @codigo
                  AND COALESCE(inativo, '') <> '-1'
                ORDER BY descricao_adicional;
                """;

            return QueryAsync<TabelaDescAdicionalModel>(sql, new { codigo });
        }

        public static Task<List<TblComplementoAdicionalModel>> GetCompleAdicionaisAsync(long? coduniadicional)
        {
            const string sql = """
                SELECT *
                FROM producao.tblcomplementoadicional
                WHERE coduniadicional = @coduniadicional
                  AND COALESCE(inativo, '') <> '-1'
                ORDER BY complementoadicional;
                """;

            return QueryAsync<TblComplementoAdicionalModel>(sql, new { coduniadicional });
        }

        public static async Task<QryDescricao?> GetDescricaoAsync(long codcompladicional)
        {
            await using var conn = CreateConnection();
            const string sql = """
                SELECT *
                FROM producao.qry3descricoes
                WHERE inativo = '0'
                  AND codcompladicional = @codcompladicional
                LIMIT 1;
                """;

            return await conn.QueryFirstOrDefaultAsync<QryDescricao>(sql, new { codcompladicional });
        }

        public static async Task<ProdutoOsModel> SaveProdutoOsAsync(ProdutoOsModel produtoOs)
        {
            await using var conn = CreateConnection();

            if (produtoOs.num_os_produto is null or 0)
            {
                const string insertSql = """
                    INSERT INTO producao.tbl_produto_os
                        (tipo, planilha, cod_produto, cod_desc_adicional, cod_compl_adicional, quantidade,
                         data_emissao, responsavel_emissao, id_modelo, solicitado_por, codigo_saida, cliente)
                    VALUES
                        (@tipo, @planilha, @cod_produto, @cod_desc_adicional, @cod_compl_adicional, @quantidade,
                         @data_emissao, @responsavel_emissao, @id_modelo, @solicitado_por, @codigo_saida, @cliente)
                    RETURNING num_os_produto;
                    """;

                produtoOs.num_os_produto = await conn.ExecuteScalarAsync<long>(insertSql, produtoOs);
            }
            else
            {
                const string updateSql = """
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
                    """;

                await conn.ExecuteAsync(updateSql, produtoOs);
            }

            return produtoOs;
        }

        public static async Task<ObsOsModel> SaveObsOsAsync(ObsOsModel obsOs)
        {
            await using var conn = CreateConnection();

            if (obsOs.cod_obs is null or 0)
            {
                const string insertSql = """
                    INSERT INTO producao.tbl_obs_os
                        (num_os_produto, cod_compl_adicional, num_caminho, codigo_setor, setor_caminho,
                         orientacao_caminho, distribuir_os, cliente, solicitado_por, solicitado_data,
                         emitida, produtos_servico_num_os_servico, cancelar, cancelado_por, cancelado_em, pt)
                    VALUES
                        (@num_os_produto, @cod_compl_adicional, @num_caminho, @codigo_setor, @setor_caminho,
                         @orientacao_caminho, @distribuir_os, @cliente, @solicitado_por, @solicitado_data,
                         @emitida, @produtos_servico_num_os_servico, @cancelar, @cancelado_por, @cancelado_em, @pt)
                    RETURNING cod_obs;
                    """;

                obsOs.cod_obs = await conn.ExecuteScalarAsync<long>(insertSql, obsOs);
            }
            else
            {
                const string updateSql = """
                    UPDATE producao.tbl_obs_os
                    SET num_os_produto = @num_os_produto,
                        cod_compl_adicional = @cod_compl_adicional,
                        num_caminho = @num_caminho,
                        codigo_setor = @codigo_setor,
                        setor_caminho = @setor_caminho,
                        orientacao_caminho = @orientacao_caminho,
                        distribuir_os = @distribuir_os,
                        cliente = @cliente,
                        solicitado_por = @solicitado_por,
                        solicitado_data = @solicitado_data,
                        emitida = @emitida,
                        produtos_servico_num_os_servico = @produtos_servico_num_os_servico,
                        cancelar = @cancelar,
                        cancelado_por = @cancelado_por,
                        cancelado_em = @cancelado_em,
                        pt = @pt
                    WHERE cod_obs = @cod_obs;
                    """;

                await conn.ExecuteAsync(updateSql, obsOs);
            }

            return obsOs;
        }

        public static async Task DeleteObsOsAsync(ObsOsModel obsOs)
        {
            await using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "DELETE FROM producao.tbl_obs_os WHERE cod_obs = @cod_obs;",
                new { obsOs.cod_obs });
        }

        public static Task<List<OrdemServicoEmissaoAbertaForm>> GetOrdensAbertasAsync()
        {
            const string sql = """
                SELECT *
                FROM producao.qry_os_emissao_aberta_form
                WHERE COALESCE(cancelar, false) = false;
                """;

            return QueryAsync<OrdemServicoEmissaoAbertaForm>(sql);
        }

        public static async Task<OsEmissaoProducaoImprimirModel?> GetOsEmitidaAsync(long? numOsServico)
        {
            await using var conn = CreateConnection();
            const string sql = """
                SELECT *
                FROM producao.qry_os_emissao_producao_imprimir
                WHERE num_os_servico = @numOsServico
                LIMIT 1;
                """;

            return await conn.QueryFirstOrDefaultAsync<OsEmissaoProducaoImprimirModel>(sql, new { numOsServico });
        }

        public static Task<List<ProdutoServicoModel>> GetServicosAsync(long? numOsProduto)
        {
            const string sql = """
                SELECT *
                FROM producao.tbl_produtos_servico
                WHERE num_os_produto = @numOsProduto
                ORDER BY num_os_servico;
                """;

            return QueryAsync<ProdutoServicoModel>(sql, new { numOsProduto });
        }

        public static async Task<ProdutoServicoModel> SaveProdutoServicoAsync(ProdutoServicoModel servico)
        {
            await using var conn = CreateConnection();

            if (servico.num_os_servico is null or 0)
            {
                const string insertSql = """
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

                servico.num_os_servico = await conn.ExecuteScalarAsync<long>(insertSql, servico);
            }
            else
            {
                const string updateSql = """
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
                    """;

                await conn.ExecuteAsync(updateSql, servico);
            }

            return servico;
        }

        public static async Task CancelarObsAsync(long? codObs, string canceladoPor, DateTime canceladoEm)
        {
            await using var conn = CreateConnection();
            const string sql = """
                UPDATE producao.tbl_obs_os
                SET cancelar = true,
                    cancelado_por = @canceladoPor,
                    cancelado_em = @canceladoEm
                WHERE cod_obs = @codObs;
                """;

            await conn.ExecuteAsync(sql, new { codObs, canceladoPor, canceladoEm });
        }

        public static async Task<AlteraSolicitacaoOsProducao?> GetAlteracaoSolicitacaoAsync(long numOsProduto)
        {
            await using var conn = CreateConnection();
            const string sql = """
                SELECT *
                FROM producao.qry_altera_solicitacao_os_producao
                WHERE num_os_produto = @numOsProduto
                LIMIT 1;
                """;

            return await conn.QueryFirstOrDefaultAsync<AlteraSolicitacaoOsProducao>(sql, new { numOsProduto });
        }

        public static Task<List<ObsOsModel>> GetCaminhosOsAsync(long numOsProduto)
        {
            const string sql = """
                SELECT *
                FROM producao.tbl_obs_os
                WHERE num_os_produto = @numOsProduto
                ORDER BY num_caminho;
                """;

            return QueryAsync<ObsOsModel>(sql, new { numOsProduto });
        }

        public static Task<List<OsEmissaoProducaoImprimirModel>> GetOsEmitidasAsync(long? numOsProduto, List<long?> caminhos)
        {
            var caminhosValidos = caminhos
                .Where(caminho => caminho.HasValue)
                .Select(caminho => caminho.Value)
                .ToArray();

            const string sql = """
                SELECT *
                FROM producao.qry_os_emissao_producao_imprimir
                WHERE num_os_produto = @numOsProduto
                  AND num_caminho = ANY(@caminhos)
                ORDER BY num_os_servico;
                """;

            return QueryAsync<OsEmissaoProducaoImprimirModel>(sql, new { numOsProduto, caminhos = caminhosValidos });
        }
    }
}
