using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using Producao.Views.OrdemServico.Produto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            var grid = sender as RadGridView;
            EmitirOSDesbaiamentoViewModel vm = (EmitirOSDesbaiamentoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                OsExpModel data = (OsExpModel)e.Row.Item;
                data.inserido_por = Environment.UserName;
                data.inserido_em = DateTime.Now;
                vm.Item = await vm.AddOsAsync(data);

                ((OsExpModel)e.Row.Item).n_os_desbaiamento = vm.Item.n_os_desbaiamento;
                grid?.Items.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.Itens.Where(x => x.n_os_desbaiamento == null).ToList();
                foreach (var item in toRemove)
                    vm.Itens.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

    }

    public class EmitirOSDesbaiamentoViewModel : INotifyPropertyChanged
    {
        static EmitirOSDesbaiamentoViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<OsExpModel>? _itens;
        public ObservableCollection<OsExpModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private OsExpModel? _item;
        public OsExpModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }

        public async Task<ObservableCollection<OsExpModel>> GetItensAsync()
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    SELECT *
                    FROM expedicao.tbl_os_exp;
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
                             solicitante, local_shopp, inserido_por, inserido_em)
                        VALUES
                            (@antigo, @codvol, @data, @resp, @setor, @quantidade, @obs, @coddetalhescompl,
                             @solicitante, @local_shopp, @inserido_por, @inserido_em)
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

