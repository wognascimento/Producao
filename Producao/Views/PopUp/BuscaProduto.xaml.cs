using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Producao.Views.PopUp
{
    /// <summary>
    /// Logica interna para BuscaProduto.xaml
    /// </summary>
    public partial class BuscaProduto : Window
    {
        private ICollectionView? descricoesView;

        public BuscaProduto()
        {
            InitializeComponent();
        }

        public QryDescricao descricao { get; set; }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                DataContext = new BuscaProdutoViewModel();
                BuscaProdutoViewModel? vm = (BuscaProdutoViewModel)DataContext;
                vm.Descricoes = await vm.GetDescricoesAsync();
                descricoesView = CollectionViewSource.GetDefaultView(vm.Descricoes);
                dgDescricores.ItemsSource = descricoesView;
                AplicarFiltroDescricao();
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtDescricao.SelectAll();
            txtDescricao.Focus();
        }

        private void txtDescricao_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltroDescricao();
        }

        private void AplicarFiltroDescricao()
        {
            var text = txtDescricao.Text?.Trim() ?? string.Empty;
            if (descricoesView is null)
            {
                if (DataContext is not BuscaProdutoViewModel vm || vm.Descricoes is null)
                    return;

                descricoesView = CollectionViewSource.GetDefaultView(vm.Descricoes);
                dgDescricores.ItemsSource = descricoesView;
            }

            if (descricoesView is null)
                return;

            descricoesView.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(text))
                    return true;

                if (item is not QryDescricao descricao)
                    return false;

                return Contains(descricao.codcompladicional?.ToString(), text)
                    || Contains(descricao.planilha, text)
                    || Contains(descricao.descricao_completa, text)
                    || Contains(descricao.unidade, text);
            };
            descricoesView.Refresh();
        }

        private static bool Contains(string? value, string text) =>
            value?.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0;

        private void dgDescricores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgDescricores.SelectedItem is not QryDescricao item)
                return;

            descricao = item;
            DialogResult = true;
        }
    }

    public class BuscaProdutoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private QryDescricao? _descricao;
        public QryDescricao? Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged(nameof(Descricao)); }
        }

        private ObservableCollection<QryDescricao>? descricoes;
        public ObservableCollection<QryDescricao>? Descricoes
        {
            get { return descricoes; }
            set { descricoes = value; RaisePropertyChanged(nameof(Descricoes)); }
        }

        public async Task<ObservableCollection<QryDescricao>> GetDescricoesAsync()
        {
            const string sql = @"
                SELECT *
                FROM producao.qry3descricoes
                WHERE inativo = '0'
                ORDER BY descricao_completa;";

            await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            var data = await conn.QueryAsync<QryDescricao>(sql);
            return new ObservableCollection<QryDescricao>(data);
        }
    }
}
