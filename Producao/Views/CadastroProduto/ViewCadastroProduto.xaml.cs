using Microsoft.EntityFrameworkCore;
using Producao.Views.PopUp;
using Syncfusion.UI.Xaml.Grid;
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
    /// Interação lógica para ViewCadastroProduto.xam
    /// </summary>
    public partial class ViewCadastroProduto : UserControl
    {

        public ViewCadastroProduto()
        {
            DataContext = new CadastroProdutoViewModel();
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CadastroProdutoViewModel vm = (CadastroProdutoViewModel)DataContext;
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                vm.ClasseSolicitCompras = await Task.Run( vm.GetClassSolicitComprasAsync);
                vm.FamiliaProds = await Task.Run(vm.GetFamiliaProdsAsync);
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

        private async void OnPlanilhaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CadastroProdutoViewModel vm = (CadastroProdutoViewModel)DataContext;
                //if (!dbClick)
                vm.Produtos = await Task.Run(async () => await vm.GetProdutosAsync(vm.Planilha?.planilha));
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

        private void OnAddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            CadastroProdutoViewModel vm = (CadastroProdutoViewModel)DataContext;
            ((ProdutoModel)e.NewObject).planilha = vm.Planilha.planilha;
        }

        private void OnCurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {

        }

        private void OnCurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            
        }

        private async void OnRowValidated(object sender, RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            CadastroProdutoViewModel vm = (CadastroProdutoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProdutoModel data = (ProdutoModel)e.RowData;
                data.inativo = data.inativo == null ? "0" : "-1";
                data.cadastrado_por = data.codigo == null ? Environment.UserName : data.cadastrado_por;
                data.datacadastro = data.codigo == null ? DateTime.Now : data.datacadastro;
                data.alterado_por = data.codigo == null ? null : Environment.UserName;
                data.data_altera = data.codigo == null ? null : DateTime.Now;
                data = await Task.Run(() => vm.SaveAsync(data));
                var record = sfdatagrid.View.CurrentAddItem as ProdutoModel;
                sfdatagrid.View.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.Produtos.Where(x => x.codigo == null).ToList();
                foreach (var item in toRemove)
                    vm.Produtos.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, RowValidatingEventArgs e)
        {
            ProdutoModel rowData = (ProdutoModel)e.RowData;
            //rowData.planilha == "Null" || rowData.planilha == "DbNull"
            if (rowData.planilha == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("classe_solict_compra", "Informe a CLASSE COMPRA");
                e.ErrorMessages.Add("familia", "Informe a FAMILIA COMPRAS");
                e.ErrorMessages.Add("descricao", "Informe a DESCRIÇÃO");
            }
            else if (rowData.classe_solict_compra == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("classe_solict_compra", "Informe a CLASSE COMPRA");
            }
            else if (rowData.familia == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("familia", "Informe a FAMILIA COMPRAS");
            }
            else if (rowData.descricao == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descricao", "Informe a DESCRIÇÃO");
            }
        }

        private void OnCurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class CadastroProdutoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<RelplanModel> _planilhas;
        public ObservableCollection<RelplanModel> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
        }
        private RelplanModel _planilha;
        public RelplanModel Planilha
        {
            get { return _planilha; }
            set { _planilha = value; RaisePropertyChanged("Planilha"); }
        }

        private ProdutoModel _produto;
        public ProdutoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private ObservableCollection<ProdutoModel> _produtos;
        public ObservableCollection<ProdutoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }

        private ClasseSolicitCompra _classeSolicitCompra;
        public ClasseSolicitCompra ClasseSolicitCompra
        {
            get { return _classeSolicitCompra; }
            set { _classeSolicitCompra = value; RaisePropertyChanged("ClasseSolicitCompra"); }
        }

        private ObservableCollection<ClasseSolicitCompra> _classeSolicitCompras;
        public ObservableCollection<ClasseSolicitCompra> ClasseSolicitCompras
        {
            get { return _classeSolicitCompras; }
            set { _classeSolicitCompras = value; RaisePropertyChanged("ClasseSolicitCompras"); }
        }

        private FamiliaProdModel _familiaProd;
        public FamiliaProdModel FamiliaProd
        {
            get { return _familiaProd; }
            set { _familiaProd = value; RaisePropertyChanged("FamiliaProd"); }
        }

        private ObservableCollection<FamiliaProdModel> _familiaProds;
        public ObservableCollection<FamiliaProdModel> FamiliaProds
        {
            get { return _familiaProds; }
            set { _familiaProds = value; RaisePropertyChanged("FamiliaProds"); }
        }

        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get { return rowDataCommand; }
            set { rowDataCommand = value; }
        }

        public CadastroProdutoViewModel()
        {
            rowDataCommand = new RelayCommand(ChangeCanExecute);
        }

        public void ChangeCanExecute(object obj)
        {
            try
            {
                //obj = {Syncfusion.UI.Xaml.Grid.SfDataGrid}
                var window = new CadastroAdicional(Produto);
                window.Title = $"Descrição Adicional do produto -> {Produto.descricao}";
                window.Owner = App.Current.MainWindow;
                window.Height = 450;
                window.Width = 700;
                if (window.ShowDialog() == true) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }


        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1")).ToListAsync();
                return new ObservableCollection<RelplanModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProdutoModel>> GetProdutosAsync(string? planilha)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Produtos
                    .OrderBy(c => c.descricao)
                    .Where(c => c.planilha.Equals(planilha))
                    .ToListAsync();
                return new ObservableCollection<ProdutoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<FamiliaProdModel>> GetFamiliaProdsAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.FamiliaProds
                    .OrderBy(c => c.nomefamilia)
                    .ToListAsync();
                return new ObservableCollection<FamiliaProdModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ClasseSolicitCompra>> GetClassSolicitComprasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ClasseSolicitCompras
                    .OrderBy(c => c.classe_solicit_compra)
                    .ToListAsync();
                return new ObservableCollection<ClasseSolicitCompra>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProdutoModel> SaveAsync(ProdutoModel produto)
        {
            try
            {
                using DatabaseContext db = new();
                await db.Produtos.SingleMergeAsync(produto);
                await db.SaveChangesAsync();
                return produto;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
