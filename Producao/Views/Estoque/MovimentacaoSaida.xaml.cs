using Microsoft.EntityFrameworkCore;
using Producao.Views.PopUp;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.Estoque
{
    /// <summary>
    /// Interação lógica para MovimentacaoSaida.xam
    /// </summary>
    public partial class MovimentacaoSaida : UserControl
    {
        public MovimentacaoSaida()
        {
            InitializeComponent();
            DataContext = new MovimentacaoSaidaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
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
            MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;

            if (e.Key == Key.Enter)
            {
                try
                {
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

        private async void OnOpenDescricoes(object sender, RoutedEventArgs e)
        {
            MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;
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

        private async void OnSelectedPlanilha(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;
                RelplanModel? planilha = e.NewValue as RelplanModel;
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

                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(planilha?.planilha));
                //vm.Itens = await Task.Run(() => vm.GetItensAsync(planilha?.planilha));
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
                MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;
                ProdutoModel? produto = e.NewValue as ProdutoModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

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
                MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;
                TabelaDescAdicionalModel? adicional = e.NewValue as TabelaDescAdicionalModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

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

        private void OnSelectedComplementoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;
            TblComplementoAdicionalModel? complemento = e.NewValue as TblComplementoAdicionalModel;
            vm.Compledicional = complemento;
            tbCodproduto.Text = complemento?.codcompladicional.ToString();
            txtQuantidade.Focus();
        }

        private async void OnAdicionarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MovimentacaoSaidaViewModel vm = (MovimentacaoSaidaViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var saida = new SaidaEstoqueModel
                {
                    quantidade = Convert.ToDouble(txtQuantidade.Text),
                    destino = procedencias.SelectedItem.ToString(),
                    saida_data = DateTime.Now,
                    saida_por = Environment.UserName,
                    codcompladicional = long.Parse(tbCodproduto.Text),
                    processado = processamento.IsChecked.Value ? "-1" : "0",
                };


                if (saida.destino == "ACERTO ESTOQUE")
                {
                    var controle = await Task.Run(() => vm.ControleAcertoAsync(saida.codcompladicional));
                    if (controle != null)
                    {
                        MessageBox.Show("PRODUTO BLOQUEADO PARA ACERTO DE ESTOQUE.");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                }

                saida = await Task.Run(() => vm.SaveAsync(saida));
                if (saida.destino == "ACERTO ESTOQUE")
                {
                    var acerto = new ControleAcertoEstoque
                    {
                        cod_movimentacao = saida.codigo_saida,
                        processado = saida.processado,
                        codcompladicional = saida.codcompladicional,
                        quantidade = saida.quantidade,
                        data = DateTime.Now,
                        hora = new TimeSpan(),
                        operacao = saida.destino,
                        processo = "ENTRADA",
                        incluido_por = Environment.UserName,
                        incluido_data = DateTime.Now,
                        bloqueado = "-1"
                    };
                    acerto = await Task.Run(() => vm.SaveAcertoAsync(acerto));
                }
                vm.Itens.Add( await Task.Run(() => vm.GetItensAsync(saida.codigo_saida)) );
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class MovimentacaoSaidaViewModel : INotifyPropertyChanged
    {
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
        private IList _itens;
        public IList Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private ObservableCollection<string> _procedencias = new() { "ACERTO ESTOQUE", "ACERTO INCÊNDIO", "TRANSFORMAÇÃO", "DESCARTE" , "DESCARTE CUPIM", "MARKETING", "KIT SOLUÇÃO", "ACERTO DE REQUISIÇÃO", "INVENTÁRIO ROTATIVO" };
        public ObservableCollection<string> Procedencias
        {
            get { return _procedencias; }
            set { _procedencias = value; RaisePropertyChanged("Procedencias"); }
        }

        public MovimentacaoSaidaViewModel()
        {
            Itens = new ObservableCollection<object>();
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

        public async Task<object> GetItensAsync(long? codigo_saida)
        {
            try
            {
                using DatabaseContext db = new();

                var resultado = db.Saidas
                    .Join(db.Descricoes, saidas => saidas.codcompladicional, descricao => descricao.codcompladicional, (saidas, descricao) => new { saidas, descricao })
                    .Where(x => x.saidas.codigo_saida == codigo_saida)
                    .Select(x => new
                    {
                        x.saidas.codigo_saida,
                        x.saidas.codcompladicional,
                        x.saidas.quantidade,
                        x.saidas.destino,
                        x.saidas.processado,
                        x.saidas.saida_data,
                        x.saidas.saida_por,
                        x.descricao.descricao_completa,
                        x.descricao.unidade
                    });

                //var resultadoToList = await resultado.ToListAsync();

                return await resultado.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SaidaEstoqueModel> SaveAsync(SaidaEstoqueModel saida)
        {
            try
            {
                using DatabaseContext db = new();
                await db.Saidas.SingleMergeAsync(saida);
                await db.SaveChangesAsync();
                return saida;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControleAcertoEstoque> SaveAcertoAsync(ControleAcertoEstoque acerto)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ControleAcertoEstoques.SingleMergeAsync(acerto);
                await db.SaveChangesAsync();
                return acerto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControleAcertoEstoque> ControleAcertoAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var controle = await db.ControleAcertoEstoques.Where(x => x.codcompladicional == codcompladicional && x.bloqueado == "-1").FirstOrDefaultAsync();
                return controle;
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
