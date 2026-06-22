using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Estoque
{
    public partial class DesbloqueioAcertoEstoque : UserControl
    {
        public DesbloqueioAcertoEstoque()
        {
            InitializeComponent();
            DataContext = new DesbloqueioAcertoEstoqueViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarAsync();
        }

        private async Task CarregarAsync()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var vm = (DesbloqueioAcertoEstoqueViewModel)DataContext;
                vm.Itens = await vm.GetListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private async void itens_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction != GridViewEditAction.Commit ||
                e.EditedItem is not AcertoEstoque acerto || !acerto.desbloqueado)
            {
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var vm = (DesbloqueioAcertoEstoqueViewModel)DataContext;
                await vm.UpdateAsync(acerto);
                vm.Itens = await vm.GetListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }

    internal class DesbloqueioAcertoEstoqueViewModel : INotifyPropertyChanged
    {
        private List<AcertoEstoque> itens = [];

        public List<AcertoEstoque> Itens
        {
            get => itens;
            set
            {
                itens = value;
                RaisePropertyChanged(nameof(Itens));
            }
        }

        private static NpgsqlConnection CreateConnection() =>
            new(DataBaseSettings.Instance.ConnectionString);

        public async Task<List<AcertoEstoque>> GetListAsync()
        {
            const string sql = """
                SELECT ai.codigo, ai.cod_movimentacao, ai.processo, ai.quantidade, ai.bloqueado,
                       descricao.codcompladicional, descricao.planilha,
                       descricao.descricao_completa, descricao.unidade,
                       false AS desbloqueado
                FROM producao.tbl_controle_acerto_estoque ai
                JOIN producao.qry3descricoes descricao
                  ON descricao.codcompladicional = ai.codcompladicional
                WHERE ai.bloqueado = '-1'
                ORDER BY descricao.planilha, descricao.descricao_completa;
                """;
            await using var connection = CreateConnection();
            return (await connection.QueryAsync<AcertoEstoque>(sql)).ToList();
        }

        public async Task UpdateAsync(AcertoEstoque acerto)
        {
            const string sql = """
                UPDATE producao.tbl_controle_acerto_estoque
                SET bloqueado = '0', liberado_por = @liberadoPor, liberado_em = @liberadoEm
                WHERE codigo = @codigo;
                """;
            await using var connection = CreateConnection();
            var linhas = await connection.ExecuteAsync(sql, new
            {
                acerto.codigo,
                liberadoPor = Environment.UserName,
                liberadoEm = DateTime.Now
            });
            if (linhas != 1)
                throw new InvalidOperationException("O lancamento de estoque nao foi localizado.");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AcertoEstoque
    {
        public long? codigo { get; set; }
        public long? cod_movimentacao { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public double? quantidade { get; set; }
        public string? bloqueado { get; set; }
        public bool desbloqueado { get; set; }
        public string? processo { get; set; }
    }
}
