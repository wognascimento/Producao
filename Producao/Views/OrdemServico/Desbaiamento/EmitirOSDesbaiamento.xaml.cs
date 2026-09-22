using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Desbaiamento
{
    /// <summary>
    /// Interação lógica para EmitirOSDesbaiamento.xam
    /// </summary>
    public partial class EmitirOSDesbaiamento : UserControl
    {
        private bool _salvandoLinha;
        private Task _validacaoCodigoTask = Task.CompletedTask;
        private OsExpModel? _linhaComCodigoInvalido;

        public EmitirOSDesbaiamento()
        {
            InitializeComponent();
            this.DataContext = new EmitirOSDesbaiamentoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EmitirOSDesbaiamentoViewModel vm = (EmitirOSDesbaiamentoViewModel)DataContext;
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void RadGridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction != GridViewEditAction.Commit || _salvandoLinha)
            {
                return;
            }

            if (sender is not RadGridView grid ||
                DataContext is not EmitirOSDesbaiamentoViewModel vm ||
                e.Row?.Item is not OsExpModel data)
            {
                return;
            }

            if (LinhaNovaVazia(data))
            {
                vm.Itens?.Remove(data);
                return;
            }

            await _validacaoCodigoTask;
            if (ReferenceEquals(_linhaComCodigoInvalido, data))
            {
                return;
            }

            _salvandoLinha = true;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                data.inserido_por = global::Producao.DataBaseSettings.Instance.Username;
                data.inserido_em = DateTime.Now;
                var itemSalvo = await vm.AddOsAsync(data);

                data.n_os_desbaiamento = itemSalvo.n_os_desbaiamento;
                vm.Item = data;
            }
            catch (InvalidOperationException ex)
            {
                _linhaComCodigoInvalido = data;
                MessageBox.Show(ex.Message, "Não foi possível salvar a O.S.", MessageBoxButton.OK, MessageBoxImage.Warning);
                ReabrirEdicaoCodigo(grid, data);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                if (vm.Itens is not null)
                {
                    var toRemove = vm.Itens.Where(x => x.n_os_desbaiamento == null).ToList();
                    foreach (var item in toRemove)
                        vm.Itens.Remove(item);
                }
            }
            finally
            {
                _salvandoLinha = false;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void RadGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell?.Column?.UniqueName != "coddetalhescompl" ||
                sender is not RadGridView grid ||
                DataContext is not EmitirOSDesbaiamentoViewModel vm ||
                e.Cell.ParentRow?.Item is not OsExpModel data)
            {
                return;
            }

            _validacaoCodigoTask = ValidarCodigoDigitadoAsync(vm, data);
            await _validacaoCodigoTask;
        }

        private async Task ValidarCodigoDigitadoAsync(EmitirOSDesbaiamentoViewModel vm, OsExpModel data)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                await vm.ValidarCodDetalheAsync(data);
                _linhaComCodigoInvalido = null;
            }
            catch (InvalidOperationException ex)
            {
                data.sigla = null;
                _linhaComCodigoInvalido = data;
                MessageBox.Show(ex.Message, "COD.DET. inválido", MessageBoxButton.OK, MessageBoxImage.Warning);

                if (FindName("itens") is RadGridView grid)
                {
                    ReabrirEdicaoCodigo(grid, data);
                }
            }
            catch (Exception ex)
            {
                data.sigla = null;
                _linhaComCodigoInvalido = data;
                Producao.ErrorDialog.Show(ex, "Não foi possível validar o COD.DET.");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private static void ReabrirEdicaoCodigo(RadGridView grid, OsExpModel data)
        {
            grid.Dispatcher.BeginInvoke(new Action(() =>
            {
                grid.CurrentItem = data;
                grid.SelectedItem = data;
                grid.CurrentColumn = grid.Columns["coddetalhescompl"];
                grid.ScrollIntoView(data);
                grid.Focus();
                grid.BeginEdit();
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private static bool LinhaNovaVazia(OsExpModel data)
        {
            return data.n_os_desbaiamento is null or 0
                && data.coddetalhescompl is null
                && data.quantidade is null or 0
                && string.IsNullOrWhiteSpace(data.codvol)
                && data.data == null
                && string.IsNullOrWhiteSpace(data.resp)
                && string.IsNullOrWhiteSpace(data.solicitante)
                && string.IsNullOrWhiteSpace(data.motivo)
                && string.IsNullOrWhiteSpace(data.setor)
                && string.IsNullOrWhiteSpace(data.obs);
        }

    }

    public class ValidacaoDetalheDesbaiamentoModel
    {
        public string? sigla { get; set; }
        public bool ja_emitida { get; set; }
    }

    public class EmitirOSDesbaiamentoViewModel : INotifyPropertyChanged
    {
        static EmitirOSDesbaiamentoViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<OsExpModel>? _itens;
        public ObservableCollection<OsExpModel>? Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private OsExpModel? _item;
        public OsExpModel? Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }

        private ObservableCollection<string> _motivos = ["COMERCIAL", "VT", "PROJETOS", "ESTOQUE", "CENTRAL DE MODELOS", "PRODUÇÃO", "EXPEDIÇÃO"];
        public ObservableCollection<string> Motivos
        {
            get { return _motivos; }
            set { _motivos = value; RaisePropertyChanged("Motivos"); }
        }

        public async Task<ObservableCollection<OsExpModel>> GetItensAsync()
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    SELECT *
                    FROM expedicao.view_os_exp;
                    """;

                var data = await conn.QueryAsync<OsExpModel>(sql);
                return new ObservableCollection<OsExpModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OsExpModel> AddOsAsync(OsExpModel osExp)
        {
            try
            {
                await using var conn = CreateConnection();
                await ValidarCodDetalheAsync(conn, osExp);

                if (osExp.n_os_desbaiamento is null or 0)
                {
                    const string insertSql = """
                        INSERT INTO expedicao.tbl_os_exp
                            (antigo, codvol, data, resp, setor, quantidade, obs, coddetalhescompl,
                             solicitante, motivo, local_shopp, inserido_por, inserido_em)
                        VALUES
                            (@antigo, @codvol, @data, @resp, @setor, @quantidade, @obs, @coddetalhescompl,
                             @solicitante, @motivo, @local_shopp, @inserido_por, @inserido_em)
                        RETURNING n_os_desbaiamento;
                        """;

                    osExp.n_os_desbaiamento = await conn.ExecuteScalarAsync<long>(insertSql, osExp);
                }
                else
                {
                    const string updateSql = """
                        UPDATE expedicao.tbl_os_exp
                        SET antigo = @antigo,
                            codvol = @codvol,
                            data = @data,
                            resp = @resp,
                            setor = @setor,
                            quantidade = @quantidade,
                            obs = @obs,
                            coddetalhescompl = @coddetalhescompl,
                            solicitante = @solicitante,
                            motivo = @motivo,
                            local_shopp = @local_shopp,
                            inserido_por = @inserido_por,
                            inserido_em = @inserido_em
                        WHERE n_os_desbaiamento = @n_os_desbaiamento;
                        """;

                    await conn.ExecuteAsync(updateSql, osExp);
                }

                return osExp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ValidarCodDetalheAsync(OsExpModel osExp)
        {
            await using var conn = CreateConnection();
            await ValidarCodDetalheAsync(conn, osExp);
        }

        private static async Task ValidarCodDetalheAsync(NpgsqlConnection conn, OsExpModel osExp)
        {
            if (osExp.coddetalhescompl is null)
            {
                throw new InvalidOperationException("Informe o COD.DET.");
            }

            const string validacaoSql = """
                SELECT
                    complemento.sigla,
                    EXISTS (
                        SELECT 1
                        FROM expedicao.tbl_os_exp AS os
                        WHERE os.coddetalhescompl = detalhe.coddetalhescompl
                          AND os.n_os_desbaiamento IS DISTINCT FROM @n_os_desbaiamento
                    ) AS ja_emitida
                FROM producao.tblDetalhesComplemento AS detalhe
                INNER JOIN producao.t_complemento_chk AS complemento
                    ON detalhe.codcompl = complemento.codcompl
                WHERE detalhe.coddetalhescompl = @coddetalhescompl
                  AND complemento.sigla NOT LIKE 'SROOM%'
                LIMIT 1;
                """;

            var validacao = await conn.QueryFirstOrDefaultAsync<ValidacaoDetalheDesbaiamentoModel>(validacaoSql, osExp);
            if (validacao == null)
            {
                throw new InvalidOperationException("O COD.DET informado não existe ou não está disponível para desbaiamento.");
            }

            if (validacao.ja_emitida)
            {
                throw new InvalidOperationException("Já existe uma O.S. de desbaiamento emitida para este COD.DET.");
            }

            osExp.sigla = validacao.sigla;
        }

    }
}

