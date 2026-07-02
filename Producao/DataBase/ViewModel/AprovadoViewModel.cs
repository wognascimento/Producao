using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Producao
{
    internal class AprovadoViewModel
    {
        private static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
        }

        public async Task<ObservableCollection<AprovadoModel>> GetAprovados()
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<AprovadoModel>(
                    @"SELECT *
                      FROM producao.qry_aprovados
                      ORDER BY ordem;");
                return new ObservableCollection<AprovadoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveAsync(AprovadoModel aprovado)
        {
            try
            {
                if (aprovado.id_aprovado == null)
                    return;

                var setColumns = typeof(AprovadoModel)
                    .GetProperties()
                    .Where(p => p.Name != nameof(AprovadoModel.id_aprovado))
                    .Select(p => $"{p.Name} = @{p.Name}");

                using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    $@"UPDATE producao.qry_aprovados
                       SET {string.Join(", ", setColumns)}
                       WHERE id_aprovado = @id_aprovado;",
                    aprovado);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
