using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Estoque
{
    public partial class BaixaRequisicao : UserControl
    {
        public BaixaRequisicao()
        {
            InitializeComponent();
            DataContext = new BaixaRequisicaoViewModel();
        }

        private async void OnBuscaRequisicao(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (!long.TryParse(tbCodproduto.Text, out var requisicao))
                    throw new InvalidOperationException("Informe um numero de requisicao valido.");

                var vm = (BaixaRequisicaoViewModel)DataContext;
                vm.Itens = await vm.GetItensAsync(requisicao);
                if (vm.Itens.Count == 0)
                    MessageBox.Show("Nao existem itens nesta requisicao.", "Busca de requisicao");
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
            if (e.EditAction != GridViewEditAction.Commit || e.EditedItem is not BaixaEstoqueRequisicaoModel data)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var vm = (BaixaRequisicaoViewModel)DataContext;
                var saida = new SaidaEstoqueModel
                {
                    codigo_saida = data.codigo_saida,
                    quantidade = data.qtd_baixa,
                    destino = "PRODUCAO",
                    saida_data = DateTime.Now,
                    saida_por = Environment.UserName,
                    codcompladicional = data.codcompladicional,
                    processado = "-1",
                    num_requisicao = long.Parse(tbCodproduto.Text),
                    caminho = "R"
                };

                await vm.SaveSaidaAsync(saida);
                data.codigo_saida = saida.codigo_saida;
                data.saida_data = saida.saida_data;
                data.saida_por = saida.saida_por;
                itens.Rebind();
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }

    internal class BaixaRequisicaoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BaixaEstoqueRequisicaoModel> itens = [];

        public ObservableCollection<BaixaEstoqueRequisicaoModel> Itens
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

        public async Task<ObservableCollection<BaixaEstoqueRequisicaoModel>> GetItensAsync(long numRequisicao)
        {
            const string sql = "SELECT * FROM producao.qry_baixa_estoque_requisicao WHERE num_requisicao = @numRequisicao;";
            await using var connection = CreateConnection();
            return new ObservableCollection<BaixaEstoqueRequisicaoModel>(
                await connection.QueryAsync<BaixaEstoqueRequisicaoModel>(sql, new { numRequisicao }));
        }

        public async Task SaveSaidaAsync(SaidaEstoqueModel saida)
        {
            const string insertSql = """
                INSERT INTO producao.t_saida
                    (quantidade, destino, saida_data, saida_por, observacao, codcompladicional,
                     local_galpao, num_requisicao, caminho, endereco, quantidade_fisica, processado)
                VALUES
                    (@quantidade, @destino, @saida_data, @saida_por, @observacao, @codcompladicional,
                     @local_galpao, @num_requisicao, @caminho, @endereco, @quantidade_fisica, @processado)
                RETURNING codigo_saida;
                """;
            const string updateSql = """
                UPDATE producao.t_saida
                SET quantidade = @quantidade, destino = @destino, saida_data = @saida_data,
                    saida_por = @saida_por, observacao = @observacao,
                    codcompladicional = @codcompladicional, local_galpao = @local_galpao,
                    num_requisicao = @num_requisicao, caminho = @caminho, endereco = @endereco,
                    quantidade_fisica = @quantidade_fisica, processado = @processado
                WHERE codigo_saida = @codigo_saida;
                """;

            await using var connection = CreateConnection();
            if (saida.codigo_saida.HasValue)
                await connection.ExecuteAsync(updateSql, saida);
            else
                saida.codigo_saida = await connection.ExecuteScalarAsync<long>(insertSql, saida);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
