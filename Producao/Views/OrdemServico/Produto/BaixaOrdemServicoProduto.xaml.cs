using Dapper;
using Npgsql;
using Producao.Views.CentralModelos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para BaixaOrdemServicoProduto.xam
    /// </summary>
    public partial class BaixaOrdemServicoProduto : UserControl
    {
        public BaixaOrdemServicoProduto()
        {
            InitializeComponent();
            this.DataContext = new BaixaOrdemServicoProdutoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                BaixaOrdemServicoProdutoViewModel vm = (BaixaOrdemServicoProdutoViewModel)DataContext;
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

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                BaixaOrdemServicoProdutoViewModel vm = (BaixaOrdemServicoProdutoViewModel)DataContext;
                BaixaOsProducaoModel data = (BaixaOsProducaoModel)e.Row.Item;
                await vm.BaixaAsync(data);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private void ItensGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = FindVisualParent<GridViewRow>(e.OriginalSource as DependencyObject);
            if (row?.Item is BaixaOsProducaoModel item)
                itensGrid.SelectedItem = item;
        }

        private async void OnCancelarClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not BaixaOrdemServicoProdutoViewModel vm ||
                itensGrid.SelectedItem is not BaixaOsProducaoModel item)
            {
                MessageBox.Show(
                    "Selecione uma O.S para cancelar.",
                    "Cancelar O.S",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var confirmar = MessageBox.Show(
                $"Deseja cancelar a O.S {item.num_os_servico}?",
                "Cancelar O.S",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmar != MessageBoxResult.Yes)
                return;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                item.cancelada_os = "-1";
                await vm.CancelarAsync(item);
                vm.Itens = await vm.GetItensAsync();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private static T? FindVisualParent<T>(DependencyObject? element)
            where T : DependencyObject
        {
            while (element is not null)
            {
                if (element is T typed)
                    return typed;

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }

    public class BaixaOrdemServicoProdutoViewModel : INotifyPropertyChanged
    {
        static BaixaOrdemServicoProdutoViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private ObservableCollection<BaixaOsProducaoModel>? _itens;
        public ObservableCollection<BaixaOsProducaoModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private BaixaOsProducaoModel? _item;
        public BaixaOsProducaoModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }

        public async Task<ObservableCollection<BaixaOsProducaoModel>> GetItensAsync()
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    SELECT *
                    FROM ht.qry_baixa_os_producao;
                    """;

                var data = await conn.QueryAsync<BaixaOsProducaoModel>(sql);
                return new ObservableCollection<BaixaOsProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task BaixaAsync(BaixaOsProducaoModel baixa)
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    UPDATE producao.tbl_produtos_servico
                    SET recebido_setor_data = @recebido_setor_data,
                        concluida_os_data = @concluida_os_data
                    WHERE num_os_servico = @num_os_servico;
                    """;

                await conn.ExecuteAsync(sql, baixa);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CancelarAsync(BaixaOsProducaoModel baixa)
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    UPDATE producao.tbl_produtos_servico
                    SET cancelada_os = @cancelada_os
                    WHERE num_os_servico = @num_os_servico;
                    """;

                await conn.ExecuteAsync(sql, baixa);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }

    public static class ContextMenuCommandsBaixaOrdemServicoProduto
    {
        static ICommand? cancelarOS;
        public static ICommand CancelarOS
        {
            get
            {
                cancelarOS ??= new RelayCommand(OnCancelarOSClicked);
                return cancelarOS;
            }
        }

        private static async void OnCancelarOSClicked(object obj)
        {
            var grid = obj as RadGridView;
            if (grid is null)
            {
                return;
            }

            var item = grid.SelectedItem as BaixaOsProducaoModel;
            if (item is null)
            {
                return;
            }

            BaixaOrdemServicoProdutoViewModel vm = (BaixaOrdemServicoProdutoViewModel)grid.DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                item.cancelada_os = "-1";
                await vm.CancelarAsync(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }
}

