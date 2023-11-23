using Microsoft.EntityFrameworkCore;
using Syncfusion.Data;
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
    /// Interação lógica para CadastroAdicional.xam
    /// </summary>
    public partial class CadastroAdicional : Window
    {
        ProdutoModel produto;
        public CadastroAdicional(ProdutoModel produto)
        {
            InitializeComponent();
            DataContext = new CadastroAdicionalViewModel();
            this.produto = produto;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CadastroAdicionalViewModel vm = (CadastroAdicionalViewModel)DataContext;
                vm.ProdutosAdicionais = await Task.Run(()=> vm.GetDescricaoAdicionaisAsync(produto.codigo));
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
            CadastroAdicionalViewModel vm = (CadastroAdicionalViewModel)DataContext;
            ((TabelaDescAdicionalModel)e.NewObject).codigoproduto = produto.codigo;
        }

        private void OnCurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {

        }

        private async void OnCurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            /*
            int columnindex = adicionais.ResolveToGridVisibleColumnIndex(args.RowColumnIndex.ColumnIndex);
            var column = adicionais.Columns[columnindex];
            if (column is GridCheckBoxColumn)
            {
                var rowIndex = this.adicionais.ResolveToRecordIndex(args.RowColumnIndex.RowIndex);
                RecordEntry record = null;
                if (this.adicionais.GroupColumnDescriptions.Count == 0)
                {
                    record = this.adicionais.View.Records[rowIndex] as RecordEntry;
                }
                else
                {
                    record = (RecordEntry)adicionais.View.TopLevelGroup.DisplayElements[rowIndex];
                }
                //Checkbox property changed value is stored here.
                var value = ((TabelaDescAdicionalModel)record.Data).inativo;
            }
            */

            CadastroAdicionalViewModel vm = (CadastroAdicionalViewModel)DataContext;
            SfDataGrid? grid = sender as SfDataGrid;
            int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            var column = grid.Columns[columnindex];
            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "inativo")
            {
                var rowIndex = grid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
                var record = grid.View.Records[rowIndex].Data as TabelaDescAdicionalModel;
                var value = record.inativo;

                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    await Task.Run(() => vm.SaveAsync(record));
                    grid.View.Refresh();
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }

                
            }

            /*
            SfDataGrid grid = (SfDataGrid)sender;
            int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            var column = grid.Columns[columnindex];
            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "inativo")
            {
                try
                {
                    var rowIndex = grid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
                    if (rowIndex > -1)
                    {
                        var record = (ExpedModel)grid.View.Records[rowIndex].Data;
                        var value = record.BaiaVirtual;
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        ExpedicaoProdutoViewModel vm = (ExpedicaoProdutoViewModel)DataContext;
                        ExpedModel expedModel = await Task.Run(() => vm.AddExpedAsync(record));
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
            */
        }

        private async void OnRowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            CadastroAdicionalViewModel vm = (CadastroAdicionalViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                TabelaDescAdicionalModel data = (TabelaDescAdicionalModel)e.RowData;
                //data.codigoproduto
                //data.descricao_adicional
                data.cadastradopor = data.coduniadicional == null ? Environment.UserName : data.cadastradopor;
                data.cadastradoem = data.coduniadicional == null ? DateTime.Now : data.cadastradoem;
                data.alteradopor = data.coduniadicional == null ? null : Environment.UserName;
                data.alteradoem = data.coduniadicional == null ? null : DateTime.Now;
                //data.revisao
                //data.obsproducaoobrigatoria
                //data.obsmontagem
                //data.unidade = produto
                data.inativo = data.inativo == null ? "0" : data.inativo;


                data = await Task.Run(() => vm.SaveAsync(data));
                var record = sfdatagrid.View.CurrentAddItem as ProdutoModel;
                sfdatagrid.View.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.ProdutosAdicionais.Where(x => x.coduniadicional == null).ToList();
                foreach (var item in toRemove)
                    vm.ProdutosAdicionais.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            TabelaDescAdicionalModel rowData = (TabelaDescAdicionalModel)e.RowData;
            if (rowData.codigoproduto == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("coduniadicional", "Produto não selecionado");
                e.ErrorMessages.Add("descricao_adicional", "Informe a DESCRIÇÃO ADICIONAL");
            }
            else if (rowData.descricao_adicional == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descricao_adicional", "Informe a DESCRIÇÃO ADICIONAL");
            }
        }

    }

    public class CadastroAdicionalViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private TabelaDescAdicionalModel _produtoAdicional;
        public TabelaDescAdicionalModel ProdutoAdicional
        {
            get { return _produtoAdicional; }
            set { _produtoAdicional = value; RaisePropertyChanged("ProdutoAdicional"); }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _produtosAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> ProdutosAdicionais
        {
            get { return _produtosAdicionais; }
            set { _produtosAdicionais = value; RaisePropertyChanged("ProdutosAdicionais"); }
        }

        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get { return rowDataCommand; }
            set { rowDataCommand = value; }
        }

        public CadastroAdicionalViewModel()
        {
            rowDataCommand = new RelayCommand(ChangeCanExecute);
        }


        public void ChangeCanExecute(object obj)
        {
            try
            {
                //obj = {Syncfusion.UI.Xaml.Grid.SfDataGrid}
                var window = new CadastroCompmento(ProdutoAdicional);
                window.Title = $"Complemento Adicional da Descrição Adicional -> {ProdutoAdicional.descricao_adicional}";
                window.Owner = App.Current.MainWindow;
                window.Height = 450;
                window.Width = 900;
                if (window.ShowDialog() == true) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        public async Task<ObservableCollection<TabelaDescAdicionalModel>> GetDescricaoAdicionaisAsync(long? codigoproduto)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.DescAdicionais
                    .OrderBy(c => c.descricao_adicional)
                    .Where(c => c.codigoproduto == codigoproduto)
                    .ToListAsync();
                return new ObservableCollection<TabelaDescAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TabelaDescAdicionalModel> SaveAsync(TabelaDescAdicionalModel adicional)
        {
            try
            {
                using DatabaseContext db = new();
                await db.DescAdicionais.SingleMergeAsync(adicional);
                await db.SaveChangesAsync();
                return adicional;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
