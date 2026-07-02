using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producao.Views.OrdemServico.Servicos
{
    internal static class ServicoOrdemRepository
    {
        static ServicoOrdemRepository() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        public static Task<List<TblServicoModel>> GetServicosAsync()
        {
            const string sql = """
                SELECT *
                FROM producao.tbl_servicos
                ORDER BY num_os;
                """;

            return QueryAsync<TblServicoModel>(sql);
        }

        public static Task<List<string>> GetTiposAsync()
        {
            const string sql = """
                SELECT tipo_servico
                FROM producao.tbl_tipo_os
                GROUP BY tipo_servico
                ORDER BY tipo_servico;
                """;

            return QueryAsync<string>(sql);
        }

        public static Task<List<SetorProducaoModel>> GetSetoresAsync()
        {
            const string sql = """
                SELECT
                    codigo_setor,
                    setor || ' - ' || galpao AS setor,
                    localizacao,
                    galpao,
                    responsavel,
                    lider,
                    alterado_por,
                    data_altera,
                    login_resp,
                    relatorio_noturno,
                    permissao_vaga,
                    inativo
                FROM producao.tbl_setor
                WHERE inativo = '0'
                ORDER BY setor;
                """;

            return QueryAsync<SetorProducaoModel>(sql);
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

        public static Task<List<SiglaChkListModel>> GetSiglasAsync()
        {
            const string sql = """
                SELECT *
                FROM producao.view_sigla_chkgeral
                ORDER BY sigla_serv;
                """;

            return QueryAsync<SiglaChkListModel>(sql);
        }

        public static async Task<TblServicoModel?> GetServicoAsync(long? numOs)
        {
            await using var conn = CreateConnection();
            const string sql = """
                SELECT *
                FROM producao.tbl_servicos
                WHERE num_os = @numOs
                LIMIT 1;
                """;

            return await conn.QueryFirstOrDefaultAsync<TblServicoModel>(sql, new { numOs });
        }

        public static async Task<TblServicoModel> SaveServicoAsync(TblServicoModel model)
        {
            await using var conn = CreateConnection();

            if (model.num_os is null or 0)
            {
                const string insertSql = """
                    INSERT INTO producao.tbl_servicos
                        (tipo, codigo_setor, descricao_setor, descricao_servico, orientacao, quantidade,
                         data_emissao, emitido_por, planilha, cliente, data_conclusao, cancelar,
                         codigo_servico, emitido_por_data, data_cancelamento, data_conclusao_efetiva,
                         cancelado_por, sigla, pt)
                    VALUES
                        (@tipo, @codigo_setor, @descricao_setor, @descricao_servico, @orientacao, @quantidade,
                         @data_emissao, @emitido_por, @planilha, @cliente, @data_conclusao, @cancelar,
                         @codigo_servico, @emitido_por_data, @data_cancelamento, @data_conclusao_efetiva,
                         @cancelado_por, @sigla, @pt)
                    RETURNING num_os;
                    """;

                model.num_os = await conn.ExecuteScalarAsync<long>(insertSql, model);
            }
            else
            {
                const string updateSql = """
                    UPDATE producao.tbl_servicos
                    SET tipo = @tipo,
                        codigo_setor = @codigo_setor,
                        descricao_setor = @descricao_setor,
                        descricao_servico = @descricao_servico,
                        orientacao = @orientacao,
                        quantidade = @quantidade,
                        data_emissao = @data_emissao,
                        emitido_por = @emitido_por,
                        planilha = @planilha,
                        cliente = @cliente,
                        data_conclusao = @data_conclusao,
                        cancelar = @cancelar,
                        codigo_servico = @codigo_servico,
                        emitido_por_data = @emitido_por_data,
                        data_cancelamento = @data_cancelamento,
                        data_conclusao_efetiva = @data_conclusao_efetiva,
                        cancelado_por = @cancelado_por,
                        sigla = @sigla,
                        pt = @pt
                    WHERE num_os = @num_os;
                    """;

                await conn.ExecuteAsync(updateSql, model);
            }

            return model;
        }

        public static async Task BaixarAsync(TblServicoModel baixa)
        {
            await using var conn = CreateConnection();
            const string sql = """
                UPDATE producao.tbl_servicos
                SET data_conclusao = @data_conclusao,
                    data_conclusao_efetiva = @data_conclusao_efetiva
                WHERE num_os = @num_os;
                """;

            await conn.ExecuteAsync(sql, baixa);
        }

        public static async Task CancelarAsync(TblServicoModel baixa)
        {
            await using var conn = CreateConnection();
            const string sql = """
                UPDATE producao.tbl_servicos
                SET cancelar = @cancelar,
                    cancelado_por = @cancelado_por,
                    data_cancelamento = @data_cancelamento
                WHERE num_os = @num_os;
                """;

            await conn.ExecuteAsync(sql, baixa);
        }
    }
}
