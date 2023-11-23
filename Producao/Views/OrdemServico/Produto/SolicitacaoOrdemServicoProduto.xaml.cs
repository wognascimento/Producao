using Microsoft.EntityFrameworkCore;
using Producao.Views.PopUp;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para SolicitacaoOrdemServicoProduto.xam
    /// </summary>
    public partial class SolicitacaoOrdemServicoProduto : UserControl
    {
        public SolicitacaoOrdemServicoProduto()
        {
            InitializeComponent();
            DataContext = new SolicitacaoOrdemServicoProdutoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //ObsOSs

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
                vm.ObsOSs = new ObservableCollection<ObsOsModel>();
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                vm.Setores = await Task.Run(vm.GetSetorsAsync);
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnBuscaProduto(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                try
                {
                    SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    string text = ((TextBox)sender).Text;
                    vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync(long.Parse(text)));
                    if (vm.Descricao == null)
                    {
                        MessageBox.Show("Produto não encontrado", "Busca de produto");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();
                    txtPlanilha.Text = vm.Descricao.planilha;
                    txtDescricao.Text = vm.Descricao.descricao;
                    txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                    txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                    txtQuantidade.Focus();

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (FormatException ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private void OnOpenDescricoes(object sender, RoutedEventArgs e)
        {
            try
            {
                SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
                var window = new BuscaProduto();
                window.Owner = App.Current.MainWindow;
                if (window.ShowDialog() == true)
                {
                    vm.Descricao = window.descricao;
                    tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();
                    txtPlanilha.Text = vm.Descricao.planilha;
                    txtDescricao.Text = vm.Descricao.descricao;
                    txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                    txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                    txtQuantidade.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedPlanilha(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
                vm.Planilha = (RelplanModel)e.NewValue;
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

                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(vm.Planilha?.planilha));
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
                SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
                vm.Produto = (ProdutoModel)e.NewValue;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(vm.Produto?.codigo));
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
                SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
                vm.DescAdicional = (TabelaDescAdicionalModel)e.NewValue;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm.DescAdicional?.coduniadicional));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSelectedComplementoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
            vm.Compledicional = (TblComplementoAdicionalModel)e.NewValue;
            //vm.Compledicional = complemento;
            tbCodproduto.Text = vm.Compledicional?.codcompladicional.ToString();
            txtQuantidade.Focus();
        }

        private async void OnCriarOS(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
                string text = tbCodproduto.Text;
                vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync(long.Parse(text)));

                var dado = new ProdutoOsModel
                {
                    tipo = cmbTipoOs.SelectedValue.ToString(), 
                    planilha = vm.Descricao.planilha, 
                    cod_produto = vm.Descricao.codigo, 
                    cod_desc_adicional = vm.Descricao.coduniadicional, 
                    cod_compl_adicional = vm.Descricao.codcompladicional, 
                    quantidade = Convert.ToDouble(txtQuantidade.Text), 
                    data_emissao = DateTime.Now, 
                    responsavel_emissao = Environment.UserName,
                    solicitado_por = Environment.UserName
                };

                vm.ProdutoOs = await Task.Run(() => vm.AddProdutoOsAsync(dado));
                
                btnNovo.Visibility = Visibility.Visible;
                btnCriar.Visibility = Visibility.Collapsed;
                caminhos.Visibility = Visibility.Visible;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnCriarNovaOS(object sender, RoutedEventArgs e)
        {
            btnNovo.Visibility = Visibility.Collapsed;
            btnCriar.Visibility = Visibility.Visible;
            caminhos.Visibility = Visibility.Collapsed;

            cmbTipoOs.SelectedValue = null;
            tbCodproduto.Text = null;
            txtPlanilha.SelectedItem = null;
            txtDescricao.SelectedItem = null;
            txtDescricaoAdicional.SelectedItem = null;
            txtComplementoAdicional.SelectedItem = null;
            txtPlanilha.Text = null;
            txtDescricao.Text = null;
            txtDescricaoAdicional.Text = null;
            txtComplementoAdicional.Text = null;
            txtQuantidade.Text = null;

            SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
            vm.ObsOSs = new ObservableCollection<ObsOsModel>();
            //vm.ObsOs
        }

        private void caminhos_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
            ((ObsOsModel)e.NewObject).num_os_produto = vm.ProdutoOs?.num_os_produto;
            ((ObsOsModel)e.NewObject).cod_compl_adicional = vm.ProdutoOs?.cod_compl_adicional;
            ((ObsOsModel)e.NewObject).cancelar = false;
        }

        private async void caminhos_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            SolicitacaoOrdemServicoProdutoViewModel vm = (SolicitacaoOrdemServicoProdutoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ObsOsModel data = (ObsOsModel)e.RowData;
                data.solicitado_por = Environment.UserName;
                data.solicitado_data = DateTime.Now;
                vm.ObsOs = await Task.Run(() => vm.SaveProdutoOsAsync(data));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.ObsOSs.Where(x => x.cod_obs == null).ToList();
                foreach (var item in toRemove)
                    vm.ObsOSs.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void caminhos_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            ObsOsModel rowData = (ObsOsModel)e.RowData;
            if (!rowData.num_os_produto.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("num_caminho", "Não foi criado O.S para incluir o(s) caminho(s).");
                e.ErrorMessages.Add("codigo_setor", "Não foi criado O.S para incluir o(s) caminho(s).");
                e.ErrorMessages.Add("orientacao_caminho", "Não foi criado O.S para incluir o(s) caminho(s).");
                e.ErrorMessages.Add("distribuir_os", "Não foi criado O.S para incluir o(s) caminho(s).");
                e.ErrorMessages.Add("cliente", "Não foi criado O.S para incluir o(s) caminho(s).");
            }
            else if (!rowData.num_caminho.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("num_caminho", "Informe a ordem do caminho da O.S.");
            }
            else if (!rowData.codigo_setor.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("codigo_setor", "Seleciona o Setor da O.S.");
            }
            else if (rowData.orientacao_caminho == "")
            {
                e.IsValid = false;
                e.ErrorMessages.Add("orientacao_caminho", "Informe uma orientação para o Setor.");
            }
            else if (rowData.orientacao_caminho == "")
            {
                e.IsValid = false;
                e.ErrorMessages.Add("distribuir_os", "Informe como será distribuida a O.S.");
            }
            else if (rowData.cliente == "")
            {
                e.IsValid = false;
                e.ErrorMessages.Add("cliente", "Informe o cliente da O.S.");
            }
        }

        private void caminhos_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            int rowIndex = caminhos.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
            ObsOsModel record;

            if (rowIndex == -1)
                record = (ObsOsModel)caminhos.View.CurrentAddItem;
            //record = new();
            else
                record = (ObsOsModel)(caminhos.View.Records[rowIndex] as RecordEntry).Data;

            if (e.RowColumnIndex.ColumnIndex == 2)
            {
                record.setor_caminho = ((SetorModel)e.SelectedItem).setor; 
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class SolicitacaoOrdemServicoProdutoViewModel : INotifyPropertyChanged
    {

        private ObsOsModel _obsOs;
        public ObsOsModel ObsOs
        {
            get { return _obsOs; }
            set { _obsOs = value; RaisePropertyChanged("ObsOs"); }
        }
        private ObservableCollection<ObsOsModel> _obsOSs;
        public ObservableCollection<ObsOsModel> ObsOSs
        {
            get { return _obsOSs; }
            set { _obsOSs = value; RaisePropertyChanged("ObsOSs"); }
        }

        private SetorModel _setor;
        public SetorModel Setor
        {
            get { return _setor; }
            set { _setor = value; RaisePropertyChanged("Setor"); }
        }
        private ObservableCollection<SetorModel> _setores;
        public ObservableCollection<SetorModel> Setores
        {
            get { return _setores; }
            set { _setores = value; RaisePropertyChanged("Setores"); }
        }

        private List<string> _distribuirOS = new List<string> { "No setor", "No Solicitante", "No Encarregado"};
        public List<string> DistribuirOS
        {
            get { return _distribuirOS; }
            set { _distribuirOS = value; RaisePropertyChanged("DistribuirOS"); }
        }

        private List<string> _ipoOS = new List<string> { "PEÇA NOVA", "RECUPERAÇÃO", "RETRABALHO", "KIT"};
        public List<string> TpoOS
        {
            get { return _ipoOS; }
            set { _ipoOS = value; RaisePropertyChanged("TpoOS"); }
        }

        private SiglaChkListModel _sigla;
        public SiglaChkListModel Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged("Sigla"); }
        }

        private ObservableCollection<SiglaChkListModel> _siglas;
        public ObservableCollection<SiglaChkListModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }

        private RelplanModel _planilha;
        public RelplanModel Planilha
        {
            get { return _planilha; }
            set { _planilha = value; RaisePropertyChanged("Planilha"); }
        }

        private ObservableCollection<RelplanModel> _planilhas;
        public ObservableCollection<RelplanModel> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
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

        private TabelaDescAdicionalModel _descAdicional;
        public TabelaDescAdicionalModel DescAdicional
        {
            get { return _descAdicional; }
            set { _descAdicional = value; RaisePropertyChanged("DescAdicional"); }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _descAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
        {
            get { return _descAdicionais; }
            set { _descAdicionais = value; RaisePropertyChanged("DescAdicionais"); }
        }

        private TblComplementoAdicionalModel _compledicional;
        public TblComplementoAdicionalModel Compledicional
        {
            get { return _compledicional; }
            set { _compledicional = value; RaisePropertyChanged("Compledicional"); }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _compleAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
        {
            get { return _compleAdicionais; }
            set { _compleAdicionais = value; RaisePropertyChanged("CompleAdicionais"); }
        }

        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }
        private ObservableCollection<QryDescricao> _descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return _descricoes; }
            set { _descricoes = value; RaisePropertyChanged("Descricoes"); }
        }

        private ProdutoOsModel _produtoOs;
        public ProdutoOsModel ProdutoOs
        {
            get { return _produtoOs; }
            set { _produtoOs = value; RaisePropertyChanged("ProdutoOs"); }
        }
        private ObservableCollection<ProdutoOsModel> _produtoOSs;
        public ObservableCollection<ProdutoOsModel> ProdutoOSs
        {
            get { return _produtoOSs; }
            set { _produtoOSs = value; RaisePropertyChanged("ProdutoOSs"); }
        }

        public async Task<ObservableCollection<SetorModel>> GetSetorsAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await (from s in db.SetorProducaos where s.inativo == "0    " select new SetorModel { setor = s.setor + " - " + s.galpao, codigo_setor = s.codigo_setor }).ToListAsync();
                return new ObservableCollection<SetorModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<SiglaChkListModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Siglas.OrderBy(c => c.sigla_serv).ToListAsync();
                return new ObservableCollection<SiglaChkListModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                return new ObservableCollection<RelplanModel>(await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1")).ToListAsync());
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
                //CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>(data);
                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QryDescricao> GetDescricaoAsync(long codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.Descricoes.Where(d => d.inativo.Equals("0") && d.codcompladicional == codcompladicional).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProdutoOsModel> AddProdutoOsAsync(ProdutoOsModel produtoOs)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ProdutoOs.SingleMergeAsync(produtoOs);
                await db.SaveChangesAsync();
                return produtoOs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObsOsModel> SaveProdutoOsAsync(ObsOsModel obsOs)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ObsOs.SingleMergeAsync(obsOs);
                await db.SaveChangesAsync();
                return obsOs;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
