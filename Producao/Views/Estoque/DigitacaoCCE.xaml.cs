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
    public partial class DigitacaoCCE : UserControl
    {
        public DigitacaoCCE()
        {
            InitializeComponent();
            DataContext = new DigitacaoCCEViewModel();
        }

        private async void OnBuscarLancamentoSemana(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (!int.TryParse(tbCodproduto.Text, out var semana))
                    throw new InvalidOperationException("Informe uma semana valida.");

                var vm = (DigitacaoCCEViewModel)DataContext;
                vm.Itens = await vm.GetItensAsync(semana);
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
            if (e.EditAction != GridViewEditAction.Commit || e.EditedItem is not ContaProcessSemanaModel data)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var vm = (DigitacaoCCEViewModel)DataContext;
                var barcode = await vm.GetBarcodeAsync(data.cod_compladicional)
                    ?? throw new InvalidOperationException("Codigo de produto nao localizado.");

                data.barcode = barcode.barcode;
                data.galpao = "JAC";
                data.digitado_por ??= Environment.UserName;
                data.digitado_data ??= DateTime.Now;
                await vm.SaveCCEAsync(data);
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

        private void itens_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            if (!int.TryParse(tbCodproduto.Text, out var semana))
                return;

            e.NewObject = new ContaProcessSemanaModel
            {
                semana = semana,
                digitado_por = Environment.UserName,
                digitado_data = DateTime.Now,
                galpao = "JAC"
            };
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }

    internal class DigitacaoCCEViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ContaProcessSemanaModel> itens = [];

        public ObservableCollection<ContaProcessSemanaModel> Itens
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

        public async Task<ObservableCollection<ContaProcessSemanaModel>> GetItensAsync(int semana)
        {
            const string sql = "SELECT * FROM producao.t_conta_process_semana WHERE semana = @semana ORDER BY cod_movimento;";
            await using var connection = CreateConnection();
            return new ObservableCollection<ContaProcessSemanaModel>(
                await connection.QueryAsync<ContaProcessSemanaModel>(sql, new { semana }));
        }

        public async Task SaveCCEAsync(ContaProcessSemanaModel cce)
        {
            const string insertSql = """
                INSERT INTO producao.t_conta_process_semana
                    (cod_compladicional, barcode, quantidade, semana, digitado_por, digitado_data, galpao)
                VALUES
                    (@cod_compladicional, @barcode, @quantidade, @semana, @digitado_por, @digitado_data, @galpao)
                RETURNING cod_movimento;
                """;
            const string updateSql = """
                UPDATE producao.t_conta_process_semana
                SET cod_compladicional = @cod_compladicional,
                    barcode = @barcode,
                    quantidade = @quantidade,
                    semana = @semana,
                    digitado_por = @digitado_por,
                    digitado_data = @digitado_data,
                    galpao = @galpao
                WHERE cod_movimento = @cod_movimento;
                """;

            await using var connection = CreateConnection();
            if (cce.cod_movimento.HasValue)
                await connection.ExecuteAsync(updateSql, cce);
            else
                cce.cod_movimento = await connection.ExecuteScalarAsync<long>(insertSql, cce);
        }

        public async Task<BarcodeModel?> GetBarcodeAsync(long? codigo)
        {
            const string sql = "SELECT * FROM producao.tbl_barcodes WHERE codigo = @codigo LIMIT 1;";
            await using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<BarcodeModel>(sql, new { codigo });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
