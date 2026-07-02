using Dapper;
using Npgsql;
using Producao.Views.PopUp;
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
                vm.Planilhas = await vm.GetPlanilhasAsync();
                vm.ClasseSolicitCompras = await vm.GetClassSolicitComprasAsync();
                vm.FamiliaProds = await vm.GetFamiliaProdsAsync();
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

        private async void OnPlanilhaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CadastroProdutoViewModel vm = (CadastroProdutoViewModel)DataContext;
                //if (!dbClick)
                vm.Produtos = await vm.GetProdutosAsync(vm.Planilha?.planilha);
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

        private void OnAddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            CadastroProdutoViewModel vm = (CadastroProdutoViewModel)DataContext;
            if (e.NewObject is ProdutoModel produto)
            {
                produto.planilha = vm.Planilha?.planilha;
            }
        }

        private async void OnRowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var grid = sender as RadGridView;
            CadastroProdutoViewModel vm = (CadastroProdutoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                if (e.Row?.Item is not ProdutoModel data)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                data.inativo = data.inativo == null ? "0" : "-1";
                data.cadastrado_por = data.codigo == null ? Environment.UserName : data.cadastrado_por;
                data.datacadastro = data.codigo == null ? DateTime.Now : data.datacadastro;
                data.alterado_por = data.codigo == null ? null : Environment.UserName;
                data.data_altera = data.codigo == null ? null : DateTime.Now;
                data = await vm.SaveAsync(data);
                grid?.Items.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.Produtos.Where(x => x.codigo == null).ToList();
                foreach (var item in toRemove)
                    vm.Produtos.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row?.Item is not ProdutoModel rowData)
            {
                return;
            }

            //rowData.planilha == "Null" || rowData.planilha == "DbNull"
            if (rowData.planilha == null)
            {
                AddValidation(e, nameof(ProdutoModel.classe_solict_compra), "Informe a CLASSE COMPRA");
                AddValidation(e, nameof(ProdutoModel.familia), "Informe a FAMILIA COMPRAS");
                AddValidation(e, nameof(ProdutoModel.descricao), "Informe a DESCRIÇÃO");
            }
            else if (rowData.classe_solict_compra == null)
            {
                AddValidation(e, nameof(ProdutoModel.classe_solict_compra), "Informe a CLASSE COMPRA");
            }
            else if (rowData.familia == null)
            {
                AddValidation(e, nameof(ProdutoModel.familia), "Informe a FAMILIA COMPRAS");
            }
            else if (rowData.descricao == null)
            {
                AddValidation(e, nameof(ProdutoModel.descricao), "Informe a DESCRIÇÃO");
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                ErrorMessage = message,
                PropertyName = propertyName
            });
        }
    }

    public class CadastroProdutoViewModel : INotifyPropertyChanged
    {
        private static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
        }

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
                var produtoSelecionado = obj as ProdutoModel ?? this.Produto;
                if (produtoSelecionado == null)
                    return;

                var window = new CadastroAdicional(produtoSelecionado);
                window.Title = $"Descrição Adicional do produto -> {produtoSelecionado.descricao}";
                window.Owner = App.Current.MainWindow;
                window.Height = 450;
                window.Width = 700;
                if (window.ShowDialog() == true) { }
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }


        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<RelplanModel>(
                    @"SELECT *
                      FROM producao.relplan
                      WHERE ativo = '1'
                      ORDER BY planilha;");
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<ProdutoModel>(
                    @"SELECT *
                      FROM producao.produtos
                      WHERE planilha = @planilha
                      ORDER BY descricao;",
                    new { planilha });
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<FamiliaProdModel>(
                    @"SELECT *
                      FROM compras.tblfamiliaprod
                      ORDER BY nomefamilia;");
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<ClasseSolicitCompra>(
                    @"SELECT *
                      FROM producao.tbl_classe_solicit_compra
                      ORDER BY classe_solicit_compra;");
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
                using var conn = CreateConnection();
                if (produto.codigo.HasValue)
                {
                    await conn.ExecuteAsync(
                        @"UPDATE producao.produtos
                          SET descricao = @descricao,
                              planilha = @planilha,
                              cadastrado_por = @cadastrado_por,
                              datacadastro = @datacadastro,
                              familia = @familia,
                              classe_solict_compra = @classe_solict_compra,
                              alterado_por = @alterado_por,
                              data_altera = @data_altera,
                              inativo = @inativo
                          WHERE codigo = @codigo;",
                        produto);
                }
                else
                {
                    produto.codigo = await conn.ExecuteScalarAsync<long>(
                        @"INSERT INTO producao.produtos
                            (descricao, planilha, cadastrado_por, datacadastro, familia,
                             classe_solict_compra, alterado_por, data_altera, inativo)
                          VALUES
                            (@descricao, @planilha, @cadastrado_por, @datacadastro, @familia,
                             @classe_solict_compra, @alterado_por, @data_altera, @inativo)
                          RETURNING codigo;",
                        produto);
                }

                return produto;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
