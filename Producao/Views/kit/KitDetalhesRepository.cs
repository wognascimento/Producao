using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producao
{
    public static class KitDetalhesRepository
    {
        static KitDetalhesRepository() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        public static Task<SiglaChkListModel?> GetSiglaAsync(string sigla)
        {
            const string sql = """
                SELECT *
                FROM producao.view_sigla_chkgeral
                WHERE sigla_serv = @sigla
                LIMIT 1;
                """;

            return QueryFirstOrDefaultAsync<SiglaChkListModel>(sql, new { sigla });
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

        public static Task<List<QryCheckListGeralModel>> GetCheckListGeralAsync(long? kp)
        {
            const string sql = """
                SELECT *
                FROM producao.qrychkgeral
                WHERE kp = @kp
                ORDER BY id;
                """;

            return QueryAsync<QryCheckListGeralModel>(sql, new { kp });
        }

        public static Task<List<string>> GetClassificacoesAsync()
        {
            const string sql = """
                SELECT classificacao
                FROM producao.tbl_classificacao_solucao
                GROUP BY classificacao
                ORDER BY classificacao;
                """;

            return QueryAsync<string>(sql);
        }

        public static Task<List<string>> GetMotivosAsync(string classificacao)
        {
            const string sql = """
                SELECT motivo
                FROM producao.tbl_classificacao_solucao
                WHERE classificacao = @classificacao
                ORDER BY motivo;
                """;

            return QueryAsync<string>(sql, new { classificacao });
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

        public static Task<List<QryCheckListGeralComplementoModel>> GetCheckListGeralComplementoAsync(long? codcompl)
        {
            const string sql = """
                SELECT *
                FROM producao.qrychkgeral_complemento
                WHERE codcompl = @codcompl
                ORDER BY coddetalhescompl;
                """;

            return QueryAsync<QryCheckListGeralComplementoModel>(sql, new { codcompl });
        }

        public static Task<List<KitChkGeralModel>> GetKitCheckListGeralAsync(long? os)
        {
            const string sql = """
                SELECT *
                FROM kitsolucao.qry_kit_chkgeral
                WHERE os = @os
                ORDER BY planilha, descricao_completa;
                """;

            return QueryAsync<KitChkGeralModel>(sql, new { os });
        }

        public static async Task EditComplementoCheckListAsync(ComplementoCheckListModel compChkList)
        {
            await using var conn = CreateConnection();
            const string sql = """
                UPDATE producao.t_complemento_chk
                SET obs = CASE WHEN COALESCE(@obs, '') <> '' THEN @obs ELSE obs END,
                    orient_montagem = CASE WHEN COALESCE(@orient_montagem, '') <> '' THEN @orient_montagem ELSE orient_montagem END,
                    orient_desmont = CASE WHEN COALESCE(@orient_desmont, '') <> '' THEN @orient_desmont ELSE orient_desmont END,
                    ordem = CASE WHEN COALESCE(@ordem, '') <> '' THEN @ordem ELSE ordem END,
                    qtd = COALESCE(@qtd, qtd),
                    alterado_por = COALESCE(@alterado_por, alterado_por),
                    alterado_em = COALESCE(@alterado_em, alterado_em)
                WHERE codcompl = @codcompl;
                """;

            await conn.ExecuteAsync(sql, compChkList);
        }

        public static async Task<ComplementoCheckListModel> AddComplementoCheckListAsync(ComplementoCheckListModel item)
        {
            await using var conn = CreateConnection();

            if (item.codcompl is null or 0)
            {
                const string insertSql = """
                    INSERT INTO producao.t_complemento_chk
                        (ordem, sigla, local_shoppings, codproduto, obs, dataalteracaodesc, alteradopor,
                         orient_montagem, item_memorial, datainclusaodesc, incluidopordesc, kp, kp2,
                         orient_desmont, qtd, coduniadicional, dataalteradescadic, alteradopordescadic,
                         nivel, carga, class_solucao, id_aprovado, historico, agrupar, motivos,
                         inserido_por, inserido_em, alterado_por, alterado_em)
                    VALUES
                        (@ordem, @sigla, @local_shoppings, @codproduto, @obs, @dataalteracaodesc, @alteradopor,
                         @orient_montagem, @item_memorial, @datainclusaodesc, @incluidopordesc, @kp, @kp2,
                         @orient_desmont, @qtd, @coduniadicional, @dataalteradescadic, @alteradopordescadic,
                         @nivel, @carga, @class_solucao, @id_aprovado, @historico, @agrupar, @motivos,
                         @inserido_por, @inserido_em, @alterado_por, @alterado_em)
                    RETURNING codcompl;
                    """;

                item.codcompl = await conn.ExecuteScalarAsync<long>(insertSql, item);
            }
            else
            {
                const string updateSql = """
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
                    """;

                await conn.ExecuteAsync(updateSql, item);
            }

            return item;
        }

        public static async Task<DetalhesComplemento> AddDetalhesComplementoCheckListAsync(DetalhesComplemento item)
        {
            await using var conn = CreateConnection();

            if (item.coddetalhescompl is null or 0)
            {
                const string insertSql = """
                    INSERT INTO producao.tbldetalhescomplemento
                        (codcompladicional, qtd, data_alteracao, alterado_por, codcompl, id_modelo, confirmado,
                         os, req, transf, local_producao, justificativa, producao, supermercado, enviado_baia,
                         obs_planilheiro, resp_prod, confirmado_por, confirmado_data, desabilitado_confirmado_por,
                         desabilitado_confirmado_data, transf_galpao, terceiro, em_producao, meta_producao,
                         num_os_produto, data_inserido, inserido_por, status_producao, status_transferencia,
                         status_atualizado_por, status_atualizado_em)
                    VALUES
                        (@codcompladicional, @qtd, @data_alteracao, @alterado_por, @codcompl, @id_modelo, @confirmado,
                         @os, @req, @transf, @local_producao, @justificativa, @producao, @supermercado, @enviado_baia,
                         @obs_planilheiro, @resp_prod, @confirmado_por, @confirmado_data, @desabilitado_confirmado_por,
                         @desabilitado_confirmado_data, @transf_galpao, @terceiro, @em_producao, @meta_producao,
                         @num_os_produto, @data_inserido, @inserido_por, @status_producao, @status_transferencia,
                         @status_atualizado_por, @status_atualizado_em)
                    RETURNING coddetalhescompl;
                    """;

                item.coddetalhescompl = await conn.ExecuteScalarAsync<long>(insertSql, item);
            }
            else
            {
                const string updateSql = """
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
                    """;

                await conn.ExecuteAsync(updateSql, item);
            }

            return item;
        }

        private static async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
        }
    }
}
