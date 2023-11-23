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

namespace Producao.Views.PopUp
{
    /// <summary>
    /// Lógica interna para BuscaProduto.xaml
    /// </summary>
    public partial class BuscaProduto : Window
    {
        public BuscaProduto()
        {
            InitializeComponent();
        }

        public QryDescricao descricao { get; set; }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait;});

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                this.DataContext = new BuscaProdutoViewModel();
                BuscaProdutoViewModel?  vm = (BuscaProdutoViewModel)DataContext;
                vm.Descricoes = await Task.Run(vm.GetDescricoesAsync);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            string text = ((TextBox)sender).Text;

            if (this.dgDescricores.SearchHelper.SearchText.Equals(text))
                return;

            //this.dgDescricores.SearchHelper.AllowCaseSensitiveSearch = true;
            this.dgDescricores.SearchHelper.SearchType = SearchType.Contains;
            this.dgDescricores.SearchHelper.AllowFiltering = true;
            this.dgDescricores.SearchHelper.Search(text);
        }

        private void dgDescricores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.descricao = (QryDescricao)dgDescricores.SelectedItem;
            this.DialogResult = true;
        }
    }

    public class BuscaProdutoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }
        private ObservableCollection<QryDescricao> descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return descricoes; }
            set { descricoes = value; RaisePropertyChanged("Descricoes"); }
        }

        public async Task<ObservableCollection<QryDescricao>> GetDescricoesAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Descricoes.Where(c => c.inativo.Equals("0")).ToListAsync();
                return new ObservableCollection<QryDescricao>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
