using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.Construcao
{
    /// <summary>
    /// Interação lógica para CadastroPeca.xam
    /// </summary>
    public partial class CadastroPeca : UserControl
    {
        public CadastroPeca()
        {
            InitializeComponent();
            DataContext = new CadastroPecaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSelectedPlanilha(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                PlanilhaConstrucaoModel? planilha = e.NewValue as PlanilhaConstrucaoModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.Produtos = new ObservableCollection<ProdutoModel>();
                txtDescricao.SelectedItem = null;
                txtDescricao.Text = string.Empty;

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                //txtUnidade.Text = string.Empty;

                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(planilha?.planilha));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricao(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                ProdutoModel? produto = e.NewValue as ProdutoModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                //txtUnidade.Text = string.Empty;

                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(produto?.codigo));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricaoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricaoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                TabelaDescAdicionalModel? adicional = e.NewValue as TabelaDescAdicionalModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                //txtUnidade.Text = string.Empty;

                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(adicional?.coduniadicional));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedComplementoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
            TblComplementoAdicionalModel? complemento = e.NewValue as TblComplementoAdicionalModel;
            vm.Compledicional = complemento;
            vm.Detalhes = await Task.Run(() => vm.GetDetalhesAsync(complemento?.codcompladicional));
        }

        private void itens_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
            ((ConstrucaoDetalheModel)e.NewObject).codcompladicional = vm.Compledicional?.codcompladicional;
        }

        private void itens_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            ConstrucaoDetalheModel rowData = (ConstrucaoDetalheModel)e.RowData;
            if (!rowData.codcompladicional.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("item", "Não é possível adicionar peça na construção.");
                e.ErrorMessages.Add("descricao_peca", "Não é possível adicionar peça na construção.");
                e.ErrorMessages.Add("volume", "Não é possível adicionar peça na construção.");
            }
            else if (!rowData.item.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("item", "Informe a sequência ordinal da Peça.");
            }
            else if (rowData.descricao_peca == "")
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descricao_peca", "Informe a descrição da Peça.");
            }
            else if (!rowData.volume.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("volume", "Informe o agrupamento dos itens");
            }
        }

        private async void itens_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ConstrucaoDetalheModel data = (ConstrucaoDetalheModel)e.RowData;

                vm.Detalhe = await Task.Run(() => vm.SaveConstrucaoDetalheAsync(data));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.Detalhes.Where(x => x.id_contrucao_detalhes == null).ToList();
                foreach (var item in toRemove)
                    vm.Detalhes.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class CadastroPecaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<PlanilhaConstrucaoModel> _planilhas;
        public ObservableCollection<PlanilhaConstrucaoModel> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
        }
        private PlanilhaConstrucaoModel _planilha;
        public PlanilhaConstrucaoModel Planilha
        {
            get { return _planilha; }
            set { _planilha = value; RaisePropertyChanged("Planilha"); }
        }

        private ObservableCollection<ProdutoModel> _produtos;
        public ObservableCollection<ProdutoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }
        private ProdutoModel _produto;
        public ProdutoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _descAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
        {
            get { return _descAdicionais; }
            set { _descAdicionais = value; RaisePropertyChanged("DescAdicionais"); }
        }
        private TabelaDescAdicionalModel _descAdicional;
        public TabelaDescAdicionalModel DescAdicional
        {
            get { return _descAdicional; }
            set { _descAdicional = value; RaisePropertyChanged("DescAdicional"); }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _compleAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
        {
            get { return _compleAdicionais; }
            set { _compleAdicionais = value; RaisePropertyChanged("CompleAdicionais"); }
        }
        private TblComplementoAdicionalModel _compledicional;
        public TblComplementoAdicionalModel Compledicional
        {
            get { return _compledicional; }
            set { _compledicional = value; RaisePropertyChanged("Compledicional"); }
        }
        
        private ObservableCollection<ConstrucaoDetalheModel> _detalhes;
        public ObservableCollection<ConstrucaoDetalheModel> Detalhes
        {
            get { return _detalhes; }
            set { _detalhes = value; RaisePropertyChanged("Detalhes"); }
        }
        private ConstrucaoDetalheModel _detalhe;
        public ConstrucaoDetalheModel Detalhe
        {
            get { return _detalhe; }
            set { _detalhe = value; RaisePropertyChanged("Detalhe"); }
        }

        

        public async Task<ObservableCollection<PlanilhaConstrucaoModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                return new ObservableCollection<PlanilhaConstrucaoModel>(await db.PlanilhasConstrucao.OrderBy(c => c.planilha).ToListAsync());
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
                Produtos = new ObservableCollection<ProdutoModel>();
                using DatabaseContext db = new();
                var data = await db.Produtos
                    .OrderBy(c => c.descricao)
                    .Where(c => c.planilha.Equals(planilha))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                return new ObservableCollection<ProdutoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TabelaDescAdicionalModel>> GetDescAdicionaisAsync(long? codigo)
        {
            try
            {
                DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                using DatabaseContext db = new();
                var data = await db.DescAdicionais
                    .OrderBy(c => c.descricao_adicional)
                    .Where(c => c.codigoproduto.Equals(codigo))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                return new ObservableCollection<TabelaDescAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TblComplementoAdicionalModel>> GetCompleAdicionaisAsync(long? coduniadicional)
        {
            try
            {
                CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                using DatabaseContext db = new();
                var data = await db.ComplementoAdicionais
                    .OrderBy(c => c.complementoadicional)
                    .Where(c => c.coduniadicional.Equals(coduniadicional))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<ObservableCollection<ConstrucaoDetalheModel>> GetDetalhesAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ConstrucaoDetalhes
                    .OrderBy(c => c.item)
                    .Where(c => c.codcompladicional == codcompladicional)
                    .ToListAsync();
                return new ObservableCollection<ConstrucaoDetalheModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ConstrucaoDetalheModel> SaveConstrucaoDetalheAsync(ConstrucaoDetalheModel construcaoDetalhe)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ConstrucaoDetalhes.SingleMergeAsync(construcaoDetalhe);
                await db.SaveChangesAsync();
                return construcaoDetalhe;
            }
            catch (Exception)
            {
                throw;
            }
        }

        

    }
}
