using Microsoft.EntityFrameworkCore;
using Producao.Views.PopUp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para SolicitacaoOrdemServicoProdutoAgrupado.xam
    /// </summary>
    public partial class SolicitacaoOrdemServicoProdutoAgrupado : UserControl
    {
        public SolicitacaoOrdemServicoProdutoAgrupado()
        {
            InitializeComponent();
            DataContext = new SolicitacaoOrdemServicoProdutoAgrupadoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
            //vm.ProdutoOSs = [];

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
                vm.ProdutoOSs = [];
                vm.ObsOSs = [];
                vm.Planilhas = await vm.GetPlanilhasAsync();
                vm.Setores = await vm.GetSetorsAsync();
                vm.Siglas = await vm.GetSiglasAsync();
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
                    SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    string text = ((TextBox)sender).Text;
                    vm.Descricao = await vm.GetDescricaoAsync(long.Parse(text));
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

        private async void OnSelectedPlanilha(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
                vm.Planilha = e.AddedItems.Count > 0 ? e.AddedItems[0] as RelplanModel : null;
                if (vm.Planilha is null)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.Produtos = [];
                txtDescricao.SelectedItem = null;
                txtDescricao.Text = string.Empty;

                vm.DescAdicionais = [];
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = [];
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                vm.Produtos = await vm.GetProdutosAsync(vm.Planilha?.planilha);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricao(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
                vm.Produto = e.AddedItems.Count > 0 ? e.AddedItems[0] as ProdutoModel : null;
                if (vm.Produto is null)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = [];
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = [];
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm.Produto?.codigo);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricaoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricaoAdicional(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
                vm.DescAdicional = e.AddedItems.Count > 0 ? e.AddedItems[0] as TabelaDescAdicionalModel : null;
                if (vm.DescAdicional is null)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm.DescAdicional?.coduniadicional);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedComplementoAdicional(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Compledicional = e.AddedItems.Count > 0 ? e.AddedItems[0] as TblComplementoAdicionalModel : null;
                if (vm.Compledicional is null)
                {
                    return;
                }

                tbCodproduto.Text = vm.Compledicional?.codcompladicional.ToString();
                vm.Descricao = await vm.GetDescricaoAsync((long)vm.Compledicional.codcompladicional);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }

        }

        private async void OnOpenDescricoes(object sender, RoutedEventArgs e)
        {
            try
            {
                SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
                var window = new BuscaProduto
                {
                    Owner = App.Current.MainWindow
                };
                if (window.ShowDialog() == true)
                {
                    vm.Descricao = window.descricao;
                    tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();
                    txtPlanilha.Text = vm.Descricao.planilha;
                    txtDescricao.Text = vm.Descricao.descricao;
                    txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                    txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgClientes_AddNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
            e.NewObject = new ProdutoOsModel
            {
                tipo = cmbTipoOs.SelectedValue?.ToString(),
                planilha = vm.Descricao?.planilha,
                cod_produto = vm.Descricao?.codigo,
                cod_desc_adicional = vm.Descricao?.coduniadicional,
                cod_compl_adicional = vm.Descricao?.codcompladicional,
                data_emissao = DateTime.Now,
                responsavel_emissao = Environment.UserName,
                solicitado_por = Environment.UserName
            };
        }

        private void FirstLevelNestedGrid_AddNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var parent = (sender as RadGridView)?.DataContext as ProdutoOsModel;
            e.NewObject = new ObsOsModel
            {
                num_os_produto = parent?.num_os_produto,
                cliente = parent?.cliente,
                cod_compl_adicional = parent?.cod_compl_adicional,
                distribuir_os = "No setor",
                solicitado_por = Environment.UserName,
                solicitado_data = DateTime.Now,
                cancelar = false,
                pt = false
            };
        }

        private async void dgClientes_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            ProdutoOsModel? primeiroCliente;
            SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProdutoOsModel data = (ProdutoOsModel)e.Row.Item;
                var ProdutoOs = await vm.SaveProdutoOsAsync(data);

                primeiroCliente = vm.ProdutoOSs.FirstOrDefault();
                foreach (var item in primeiroCliente?.Observacoes ?? [])
                {
                    ObsOsModel obsOs = new()
                    {
                        num_caminho = item.num_caminho,
                        num_os_produto = ProdutoOs.num_os_produto,
                        cliente = ProdutoOs.cliente,
                        cod_compl_adicional = ProdutoOs.cod_compl_adicional,
                        distribuir_os = "No setor",
                        solicitado_por = Environment.UserName,
                        solicitado_data = DateTime.Now,
                        cancelar = false,
                        codigo_setor = item.codigo_setor,
                        setor_caminho = item.setor_caminho,
                        orientacao_caminho = item.orientacao_caminho,
                        pt = item.pt,
                    };
                    obsOs = await vm.SaveObsOsAsync(obsOs);
                    ((ProdutoOsModel)e.Row.Item).Observacoes.Add(obsOs);
                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.ProdutoOSs.Where(x => x.num_os_produto == null).ToList();
                foreach (var item in toRemove)
                    vm.ProdutoOSs.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void dgClientes_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == GridViewEditOperationType.None || e.Row.Item is not ProdutoOsModel rowData)
            {
                return;
            }

            ProdutoOsModel? primeiroCliente;

            if (rowData.cliente == "")
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informe o cliente da O.S.", PropertyName = "cliente" });
            }
            else if (!rowData.quantidade.HasValue)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informe a quantidade da O.S.", PropertyName = "quantidade" });
            }
            else if (dgClientes.Items.Count > 0)
            {
                primeiroCliente = ((SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext).ProdutoOSs.FirstOrDefault();
                if (primeiroCliente?.Observacoes.Count == 0)
                {
                    e.IsValid = false;
                    e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Preencha todos os caminhos no primeiro cliente", PropertyName = "cliente" });
                    e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Preencha todos os caminhos no primeiro cliente", PropertyName = "quantidade" });
                }
            }

        }

        private async void FirstLevelNestedGrid_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            SolicitacaoOrdemServicoProdutoAgrupadoViewModel vm = (SolicitacaoOrdemServicoProdutoAgrupadoViewModel)DataContext;
            try
            {
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ObsOsModel data = (ObsOsModel)e.Row.Item;
                data.setor_caminho = vm.Setores.Where(x => x.codigo_setor == data.codigo_setor).Select(setor => setor.setor).FirstOrDefault();
                vm.ObsOs = await vm.SaveObsOsAsync(data);
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

        private void FirstLevelNestedGrid_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == GridViewEditOperationType.None || e.Row.Item is not ObsOsModel rowData)
            {
                return;
            }

            if (!rowData.num_os_produto.HasValue)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Não foi criado O.S para incluir o(s) caminho(s).", PropertyName = string.Empty });
            }
            else if (!rowData.num_caminho.HasValue)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informe a ordem do caminho da O.S.", PropertyName = "num_caminho" });
            }
            else if (!rowData.codigo_setor.HasValue)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Seleciona o Setor da O.S.", PropertyName = "codigo_setor" });
            }
            else if (rowData.orientacao_caminho == "")
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informe uma orientação para o Setor.", PropertyName = "orientacao_caminho" });
            }
            else if (rowData.cliente == "")
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informe o cliente da O.S.", PropertyName = "cliente" });
            }
        }

    }

    class SolicitacaoOrdemServicoProdutoAgrupadoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

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

        private List<string> _distribuirOS = new List<string> { "No setor", "No Solicitante", "No Encarregado" };
        public List<string> DistribuirOS
        {
            get { return _distribuirOS; }
            set { _distribuirOS = value; RaisePropertyChanged("DistribuirOS"); }
        }

        private List<string> _ipoOS = new List<string> { "PEÇA NOVA", "RECUPERAÇÃO", "RETRABALHO", "KIT", "PREPARAÇÃO" };
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

        public async Task<ObservableCollection<SetorModel>> GetSetorsAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await (from s in db.SetorProducaos where s.inativo == "0    " orderby s.setor, s.galpao select new SetorModel { setor = s.setor + " - " + s.galpao, codigo_setor = s.codigo_setor }).ToListAsync();
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

        public async Task<ProdutoOsModel> SaveProdutoOsAsync(ProdutoOsModel produtoOs)
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

        public async Task<ObsOsModel> SaveObsOsAsync(ObsOsModel obsOs)
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

        public async Task DeleteObsOsAsync(ObsOsModel obsOs)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ObsOs.SingleDeleteAsync(obsOs);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
