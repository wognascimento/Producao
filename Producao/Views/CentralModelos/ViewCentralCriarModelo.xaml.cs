using Dapper;
using Npgsql;
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
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Interação lógica para ViewCentralCriarModelo.xam
    /// </summary>
    public partial class ViewCentralCriarModelo : UserControl
    {
        private  CentralCriarModeloViewModel? vm;
        private QryModeloModel? modelo;
        public ViewCentralCriarModelo()
        {
            InitializeComponent();
            
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = new CentralCriarModeloViewModel();
            vm = (CentralCriarModeloViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Planilhas = await vm.GetPlanilhasAsync();
                vm.Temas = await vm.GetTemasAsync();
                vm.QryModelos = await vm.GetModelos();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtPlanilha.Focus();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
        private async void OnBuscaProduto(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    string text = txtCodigoProduto.Text;
                    vm.Descricao = await vm.GetDescricaoAsync(long.Parse(text));
                    if (vm.Descricao == null)
                    {
                        MessageBox.Show("Produto não encontrado", "Busca de produto");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    txtCodigoProduto.Text = vm.Descricao.codcompladicional.ToString();
                    txtPlanilha.Text = vm.Descricao.planilha;
                    txtDescricao.Text = vm.Descricao.descricao;
                    txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                    txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                    txtTema.Focus();

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (FormatException ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                
                
            }
        }
        private void OnOpenDescricoes(object sender, RoutedEventArgs e)
        {
            var window = new BuscaProduto();
            window.Owner = App.Current.MainWindow;
            //window.ShowDialog();
            if (window.ShowDialog() == true)
            {
                vm.Descricao = window.descricao;

                txtCodigoProduto.Text = vm.Descricao.codcompladicional.ToString();
                txtPlanilha.Text = vm.Descricao.planilha;
                txtDescricao.Text = vm.Descricao.descricao;
                txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                txtTema.Focus();
            }  
        }
        private async void OnSelectedPlanilha(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                RelplanModel? planilha = txtPlanilha.SelectedItem as RelplanModel;
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

                vm.Produtos = await vm.GetProdutosAsync(planilha?.planilha);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSelectedDescricao(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ProdutoModel? produto = txtDescricao.SelectedItem as ProdutoModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(produto?.codigo);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricaoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
        private async void OnSelectedDescricaoAdicional(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TabelaDescAdicionalModel? adicional = txtDescricaoAdicional.SelectedItem as TabelaDescAdicionalModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(adicional?.coduniadicional);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnSelectedComplementoAdicional(object sender, SelectionChangedEventArgs e)
        {
            TblComplementoAdicionalModel? complemento = txtComplementoAdicional.SelectedItem as TblComplementoAdicionalModel;
            vm.Compledicional = complemento;
            txtCodigoProduto.Text = complemento?.codcompladicional.ToString();
            txtTema.Focus();
        }

        private void OnSelectedTema(object sender, SelectionChangedEventArgs e)
        {
            TemaModel? tema = txtTema.SelectedItem as TemaModel;
            vm.Tema = tema;
        }

        private void Limpar()
        {
            txtCodigoProduto.Text = string.Empty;
            txtPlanilha.Text = string.Empty;
            txtDescricao.Text = string.Empty;
            txtDescricaoAdicional.Text = string.Empty;
            txtComplementoAdicional.Text = string.Empty;
            txtTema.Text = string.Empty;
            txtObservacao.Text = string.Empty;
            txtPlanilha.Focus();
            dgModelos.FilterDescriptors.Clear();
            this.dgModelos.SelectedItems.Clear();
            modelo = null;
        }

        private void OnLimparClick(object sender, RoutedEventArgs e)
        {
            Limpar();
        }

        private async void OnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var dados = new ModeloModel
                {
                    id_modelo = modelo?.id_modelo,
                    codcompladicional = long.Parse(txtCodigoProduto.Text), 
                    tema = txtTema.Text, 
                    obs_modelo = txtObservacao.Text,
                    cadastrado_por = Environment.UserName,
                    data_cadastro = DateTime.Now
                };

                var tema = txtTema.SelectedItem as TemaModel;
                
                vm.Modelo = await vm.AddModeloAsync(dados, tema);
                if (modelo == null)
                {
                    modelo = await vm.GetModelo(vm.Modelo.id_modelo);
                    vm?.QryModelos.Add(modelo);
                    this.dgModelos.SelectedItem = modelo;
                    this.dgModelos.ScrollIntoViewAsync(modelo, null);
                }
                else
                {
                    modelo = await vm.GetModelo(vm.Modelo.id_modelo);
                    var found = vm.QryModelos.FirstOrDefault(x => x.id_modelo == modelo.id_modelo);
                    int i = vm.QryModelos.IndexOf(found);
                    vm.QryModelos[i] = modelo;

                    //var item = vm?.QryModelos.FirstOrDefault(i => i.id_modelo == modelo.id_modelo);
                    //item = modelo;

                    //dgModelos.SelectedItem = modelo;
                }
                /*
                var window = new ModeloReceita(modelo);
                window.Owner = App.Current.MainWindow;
                window.ShowDialog();
                */
                var window = new ModeloReceita(modelo)
                {
                    Owner = Application.Current.MainWindow
                };
                PosicionarJanelaSobreGrid(window, dgModelos);
                window.ShowDialog();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }


        private void OnReceitaClick(object sender, RoutedEventArgs e)
        {
            var modelo = dgModelos.SelectedItem as QryModeloModel;

            if(modelo == null)
            {
                MessageBox.Show("Precisa selecionar um modelo para ir na Receita","Receita");
                return;
            }

            var window = new ModeloReceita(modelo)
            {
                Owner = Application.Current.MainWindow
            };
            PosicionarJanelaSobreGrid(window, dgModelos);
            window.ShowDialog();
        }

        private static void PosicionarJanelaSobreGrid(Window window, FrameworkElement grid)
        {
            if (grid.ActualWidth <= 0 || grid.ActualHeight <= 0)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                return;
            }

            var origem = grid.PointToScreen(new Point(0, 0));
            var source = PresentationSource.FromVisual(grid);

            if (source?.CompositionTarget != null)
            {
                origem = source.CompositionTarget.TransformFromDevice.Transform(origem);
            }

            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.WindowState = WindowState.Normal;
            window.Left = origem.X;
            window.Top = origem.Y;
            window.Width = grid.ActualWidth;
            window.Height = grid.ActualHeight;
        }

        private void OnControleClick(object sender, RoutedEventArgs e)
        {
            modelo = (QryModeloModel)dgModelos.SelectedItem;

            if (modelo == null)
            {
                MessageBox.Show("Precisa selecionar um modelo para abrir o controle", "Controle");
                return;
            }

            var window = new ModeloControleChecklist(modelo);
            window.Owner = App.Current.MainWindow;
            window.ShowDialog();
        }

        private void OnModelosFiadaClick(object sender, RoutedEventArgs e)
        {
            var window = new ModeloFiada(modelo);
            window.Owner = App.Current.MainWindow;
            window.ShowDialog();
        }

        private async void dgModelos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            modelo = (QryModeloModel)dgModelos.SelectedItem;

            CentralCriarModeloViewModel? vm = (CentralCriarModeloViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Descricao = await vm.GetDescricaoAsync(modelo.codcompladicional);
                txtCodigoProduto.Text = vm.Descricao.codcompladicional.ToString();
                txtPlanilha.Text = vm.Descricao.planilha;
                txtDescricao.Text = vm.Descricao.descricao;
                txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                txtTema.Text = modelo.tema;
                txtObservacao.Text = modelo.obs_modelo;

                vm.Produtos = await vm.GetProdutosAsync(vm.Descricao.planilha);
                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm.Descricao.codigo);
                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm.Descricao.coduniadicional);

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (FormatException ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void dgModelos_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            //var SelectedItem = ((DetailsViewDataGrid)e.OriginalSender).SelectedItem;
        }

        private async void OnAtualizarGridClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.QryModelos = await vm.GetModelos();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class CentralCriarModeloViewModel : INotifyPropertyChanged
    {
        static CentralCriarModeloViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        private static async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private QryModeloModel qrymodelo;
        public QryModeloModel QryModelo
        {
            get { return qrymodelo; }
            set { qrymodelo = value; RaisePropertyChanged("QryModelo"); }
        }

        private ObservableCollection<QryModeloModel>? qrymodelos;
        public ObservableCollection<QryModeloModel> QryModelos
        {
            get { return qrymodelos; }
            set { qrymodelos = value; RaisePropertyChanged("QryModelos"); }
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

        private ObservableCollection<TemaModel> _temas;
        public ObservableCollection<TemaModel> Temas
        {
            get { return _temas; }
            set { _temas = value; RaisePropertyChanged("Temas"); }
        }
        private TemaModel _tema;
        public TemaModel Tema
        {
            get { return _tema; }
            set { _tema = value; RaisePropertyChanged("Tema"); }
        }
        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }

        private ModeloModel modelo;
        public ModeloModel Modelo
        {
            get { return modelo; }
            set { modelo = value; RaisePropertyChanged("Modelo"); }
        }

        public async Task<QryModeloModel> GetModelo(long? id_modelo)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qrymodelos
                    WHERE id_modelo = @id_modelo
                    LIMIT 1;
                    """;

                var data = await QueryFirstOrDefaultAsync<QryModeloModel>(sql, new { id_modelo });
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryModeloModel>> GetModelos()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM modelos.qrymodelos;
                    """;

                var data = await QueryAsync<QryModeloModel>(sql);
                return new ObservableCollection<QryModeloModel>(data);
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
                const string sql = """
                    SELECT *
                    FROM producao.relplan
                    WHERE ativo = '1'
                      AND agrupamento LIKE '%' || @agrupamento || '%'
                    ORDER BY planilha;
                    """;

                var data = await QueryAsync<RelplanModel>(sql, new { agrupamento = "CENTRAL DE MODELOS" });
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
                const string sql = """
                    SELECT *
                    FROM producao.produtos
                    WHERE planilha = @planilha
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY descricao;
                    """;

                var data = await QueryAsync<ProdutoModel>(sql, new { planilha });
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
                const string sql = """
                    SELECT *
                    FROM producao.tabela_desc_adicional
                    WHERE codigoproduto = @codigo
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY descricao_adicional;
                    """;

                var data = await QueryAsync<TabelaDescAdicionalModel>(sql, new { codigo });
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
                const string sql = """
                    SELECT *
                    FROM producao.tblcomplementoadicional
                    WHERE coduniadicional = @coduniadicional
                      AND COALESCE(inativo, '') <> '-1'
                    ORDER BY complementoadicional;
                    """;

                var data = await QueryAsync<TblComplementoAdicionalModel>(sql, new { coduniadicional });
                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task GravarAsync()
        {
            try
            {
                await Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TemaModel>> GetTemasAsync()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM comercial.temas
                    WHERE ativo = 'S'
                    ORDER BY temas;
                    """;

                var data = await QueryAsync<TemaModel>(sql);
                return new ObservableCollection<TemaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QryDescricao> GetDescricaoAsync(long? codcompladicional)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.qry3descricoes
                    WHERE codcompladicional = @codcompladicional
                    LIMIT 1;
                    """;
                    
                var data = await QueryFirstOrDefaultAsync<QryDescricao>(sql, new { codcompladicional });
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ModeloModel> AddModeloAsync(ModeloModel modelo, TemaModel tema)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                if (modelo.id_modelo is null or 0)
                {
                    const string insertSql = """
                        INSERT INTO modelos.tbl_modelos
                            (foto, tema, obs_modelo, aprovado, aprovado_por, data_aprovacao, alterado, data_alteracao,
                             liberado, liberado_por, data_liberacao, codcompladicional, cadastrado_por, data_cadastro, qtd_fiada_cascata)
                        VALUES
                            (@foto, @tema, @obs_modelo, @aprovado, @aprovado_por, @data_aprovacao, @alterado, @data_alteracao,
                             @liberado, @liberado_por, @data_liberacao, @codcompladicional, @cadastrado_por, @data_cadastro, @qtd_fiada_cascata)
                        RETURNING id_modelo;
                        """;

                    modelo.id_modelo = await conn.ExecuteScalarAsync<long>(insertSql, modelo, transaction);
                }
                else
                {
                    const string updateSql = """
                        UPDATE modelos.tbl_modelos
                        SET foto = @foto,
                            tema = @tema,
                            obs_modelo = @obs_modelo,
                            aprovado = @aprovado,
                            aprovado_por = @aprovado_por,
                            data_aprovacao = @data_aprovacao,
                            alterado = @alterado,
                            data_alteracao = @data_alteracao,
                            liberado = @liberado,
                            liberado_por = @liberado_por,
                            data_liberacao = @data_liberacao,
                            codcompladicional = @codcompladicional,
                            cadastrado_por = @cadastrado_por,
                            data_cadastro = @data_cadastro,
                            qtd_fiada_cascata = @qtd_fiada_cascata
                        WHERE id_modelo = @id_modelo;
                        """;

                    await conn.ExecuteAsync(updateSql, modelo, transaction);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return modelo;
        }

    }
}

