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
                vm.DetalhesComplemento = await vm.GetDetalhesComplementoAsync();
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
            if (e.EditAction != GridViewEditAction.Commit)
            {
                return;
            }

            if (sender is not RadGridView grid ||
                DataContext is not EmitirOSDesbaiamentoViewModel vm ||
                e.Row?.Item is not OsExpModel data)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                data.inserido_por = Environment.UserName;
                data.inserido_em = DateTime.Now;
                var itemSalvo = await vm.AddOsAsync(data);

                data.n_os_desbaiamento = itemSalvo.n_os_desbaiamento;
                vm.Item = data;
                grid.Rebind();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
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
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void RadGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell?.Column?.UniqueName != "coddetalhescompl" ||
                e.Cell.DataContext is not OsExpModel item ||
                DataContext is not EmitirOSDesbaiamentoViewModel vm)
            {
                return;
            }

            var detalhe = vm.DetalhesComplemento.FirstOrDefault(x => x.coddetalhescompl == item.coddetalhescompl);
            item.sigla = detalhe?.sigla;

            if (sender is RadGridView grid)
                grid.Rebind();
        }

    }

    public class DetalheComplementoDesbaiamentoModel
    {
        public long? coddetalhescompl { get; set; }
        public string? sigla { get; set; }
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

        private ObservableCollection<DetalheComplementoDesbaiamentoModel> _detalhesComplemento = [];
        public ObservableCollection<DetalheComplementoDesbaiamentoModel> DetalhesComplemento
        {
            get { return _detalhesComplemento; }
            set { _detalhesComplemento = value; RaisePropertyChanged("DetalhesComplemento"); }
        }

        public async Task<ObservableCollection<DetalheComplementoDesbaiamentoModel>> GetDetalhesComplementoAsync()
        {
            await using var conn = CreateConnection();
            const string sql = """
                SELECT
                    producao.tblDetalhesComplemento.coddetalhescompl,
                    producao.t_complemento_chk.sigla
                FROM
                    producao.tblDetalhesComplemento
                    INNER JOIN producao.t_complemento_chk ON producao.tblDetalhesComplemento.codcompl = producao.t_complemento_chk.codcompl
                GROUP BY
                    producao.tblDetalhesComplemento.coddetalhescompl,
                    producao.t_complemento_chk.sigla
                HAVING
                    producao.t_complemento_chk.sigla NOT LIKE 'SROOM%'
                ORDER BY
                    producao.tblDetalhesComplemento.coddetalhescompl;
                """;

            var data = await conn.QueryAsync<DetalheComplementoDesbaiamentoModel>(sql);
            return new ObservableCollection<DetalheComplementoDesbaiamentoModel>(data);
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

    }
}

