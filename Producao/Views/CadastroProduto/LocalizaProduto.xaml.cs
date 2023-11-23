using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SearchHelperExt = Producao.Views.Utils.SearchHelperExt;

namespace Producao.Views.CadastroProduto
{
    /// <summary>
    /// Interação lógica para LocalizaProduto.xam
    /// </summary>
    public partial class LocalizaProduto : UserControl
    {
        public LocalizaProduto(/*object DataContext*/)
        {
            InitializeComponent();
            this.DataContext = new LocalizaProdutoViewModel();

            this.dataGrid.SearchHelper = new SearchHelperExt(this.dataGrid);
            //this.txtBusca.LostFocus += TextBox_LostFocus;
            //this.txtBusca.PreviewKeyDown += TextBox_PreviewKeyDown;
            this.txtBusca.TextChanged += TextBox_TextChanged;
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PerformSearch();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PerformSearch();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtBusca.Focus();
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                LocalizaProdutoViewModel vm = (LocalizaProdutoViewModel)DataContext;
                vm.Descricoes = await Task.Run(vm.GetDescricoesAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void PerformSearch()
        {
            try
            {
                if (this.dataGrid.SearchHelper.SearchText.Equals(this.txtBusca.Text))
                    return;

                var text = txtBusca.Text;
                //AllowCaseSensitiveSearch  - true -> improves the performance when search numeric fields.
                this.dataGrid.SearchHelper.AllowCaseSensitiveSearch = false;
                this.dataGrid.SearchHelper.SearchType = SearchType.Contains;
                this.dataGrid.SearchHelper.AllowFiltering = true;
                this.dataGrid.SearchHelper.Search(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }

    }

    public class LocalizaProdutoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #region Descrição Produção
        private ObservableCollection<QryDescricao> descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return descricoes; }
            set { descricoes = value; RaisePropertyChanged("Descricoes"); }
        }
        private QryDescricao descricao;
        public QryDescricao Descricao
        {
            get { return descricao; }
            set { descricao = value; RaisePropertyChanged("Descricao"); }
        }
        #endregion

        public async Task<ObservableCollection<QryDescricao>> GetDescricoesAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Descricoes.Where(p => p.inativo != "-1").ToListAsync();
                return new ObservableCollection<QryDescricao>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
