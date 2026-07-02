using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Producao.Views.CadastroProduto
{
    /// <summary>
    /// Interação lógica para LocalizaProduto.xam
    /// </summary>
    public partial class LocalizaProduto : UserControl
    {
        public LocalizaProduto()
        {
            InitializeComponent();
            DataContext = new LocalizaProdutoViewModel();
            txtBusca.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PerformSearch();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtBusca.Focus();
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var vm = (LocalizaProdutoViewModel)DataContext;
                vm.Descricoes = await vm.GetDescricoesAsync();
                ConfigureFilter();
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

        private void ConfigureFilter()
        {
            var view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            if (view != null)
                view.Filter = FilterDescricao;
        }

        private bool FilterDescricao(object obj)
        {
            if (obj is not QryDescricao item)
                return false;

            var text = txtBusca.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return Contains(item.planilha, text)
                || Contains(item.descricao_completa, text)
                || Contains(item.unidade, text);
        }

        private static bool Contains(object value, string text)
        {
            return Convert.ToString(value, CultureInfo.CurrentCulture)?.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void PerformSearch()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dataGrid.ItemsSource)?.Refresh();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }

    public class LocalizaProdutoViewModel : INotifyPropertyChanged
    {
        private readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<QryDescricao> descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return descricoes; }
            set { descricoes = value; RaisePropertyChanged(nameof(Descricoes)); }
        }

        private QryDescricao descricao;
        public QryDescricao Descricao
        {
            get { return descricao; }
            set { descricao = value; RaisePropertyChanged(nameof(Descricao)); }
        }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(BaseSettings.ConnectionString);
        }

        public async Task<ObservableCollection<QryDescricao>> GetDescricoesAsync()
        {
            using var conn = CreateConnection();
            var data = await conn.QueryAsync<QryDescricao>(
                """
                SELECT *
                FROM producao.qry3descricoes
                WHERE COALESCE(inativo, '') <> '-1';
                """);

            return new ObservableCollection<QryDescricao>(data);
        }
    }
}
