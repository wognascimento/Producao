using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.Views.kit.solucao;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CadastroProduto
{
    /// <summary>
    /// Interação lógica para ViewProdutoShopping.xam
    /// </summary>
    public partial class ViewProdutoShopping : UserControl
    {
        public ViewProdutoShopping()
        {
            InitializeComponent();
            DataContext = new ViewProdutoShoppingViewModel();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ViewProdutoShoppingViewModel vm = (ViewProdutoShoppingViewModel)DataContext;
                vm.Produtos = await Task.Run(vm.GetProdutosAsync);


                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {

            var sfdatagrid = sender as SfDataGrid;
            ViewProdutoShoppingViewModel vm = (ViewProdutoShoppingViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProdutoShoppingModel data = (ProdutoShoppingModel)e.RowData;
                vm.Produto = await Task.Run(() => vm.AddProdutoAsync(data));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
 
        }
    }

    public class ViewProdutoShoppingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ProdutoShoppingModel> _produtos;
        public ObservableCollection<ProdutoShoppingModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }
        private ProdutoShoppingModel _produto;
        public ProdutoShoppingModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        public async Task<ObservableCollection<ProdutoShoppingModel>> GetProdutosAsync()
        {
            try
            {
                using DatabaseContext db = new();
                //var data = await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1")).ToListAsync();
                var data = await db.ProdutoShopping.ToListAsync();
                return new ObservableCollection<ProdutoShoppingModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProdutoShoppingModel> AddProdutoAsync(ProdutoShoppingModel produto)
        {
            try
            {
                using DatabaseContext db = new();
                db.Entry(produto).State = produto.codcompladicional == null ? EntityState.Added : EntityState.Modified;
                //await db.ProdutoShopping.SingleMergeAsync(produto);
                await db.SaveChangesAsync();
                return produto;
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

    }
}
