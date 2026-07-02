using Dapper;
using Npgsql;
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
                vm.Planilhas = await vm.GetPlanilhasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSelectedPlanilha(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                PlanilhaConstrucaoModel? planilha = txtPlanilha.SelectedItem as PlanilhaConstrucaoModel;
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

                vm.Produtos = await vm.GetProdutosAsync(planilha?.planilha);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnSelectedDescricao(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                ProdutoModel? produto = txtDescricao.SelectedItem as ProdutoModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                //txtUnidade.Text = string.Empty;

                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(produto?.codigo);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricaoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnSelectedDescricaoAdicional(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                TabelaDescAdicionalModel? adicional = txtDescricaoAdicional.SelectedItem as TabelaDescAdicionalModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                //txtUnidade.Text = string.Empty;

                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(adicional?.coduniadicional);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnSelectedComplementoAdicional(object sender, SelectionChangedEventArgs e)
        {
            CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
            TblComplementoAdicionalModel? complemento = txtComplementoAdicional.SelectedItem as TblComplementoAdicionalModel;
            vm.Compledicional = complemento;
            vm.Detalhes = await vm.GetDetalhesAsync(complemento?.codcompladicional);
        }

        private void itens_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
            if (e.NewObject is ConstrucaoDetalheModel novo)
            {
                novo.codcompladicional = vm.Compledicional?.codcompladicional;
            }
        }

        private void itens_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row?.Item is not ConstrucaoDetalheModel rowData)
            {
                return;
            }

            if (!rowData.codcompladicional.HasValue)
            {
                AddValidation(e, nameof(ConstrucaoDetalheModel.item), "Não é possível adicionar peça na construção.");
                AddValidation(e, nameof(ConstrucaoDetalheModel.descricao_peca), "Não é possível adicionar peça na construção.");
                AddValidation(e, nameof(ConstrucaoDetalheModel.volume), "Não é possível adicionar peça na construção.");
            }
            else if (!rowData.item.HasValue)
            {
                AddValidation(e, nameof(ConstrucaoDetalheModel.item), "Informe a sequência ordinal da peça.");
            }
            else if (string.IsNullOrWhiteSpace(rowData.descricao_peca))
            {
                AddValidation(e, nameof(ConstrucaoDetalheModel.descricao_peca), "Informe a descrição da peça.");
            }
            else if (!rowData.volume.HasValue)
            {
                AddValidation(e, nameof(ConstrucaoDetalheModel.volume), "Informe o agrupamento dos itens.");
            }
        }

        private async void itens_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                if (e.Row?.Item is not ConstrucaoDetalheModel data)
                {
                    return;
                }

                vm.Detalhe = await vm.SaveConstrucaoDetalheAsync(data);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.Detalhes.Where(x => x.id_contrucao_detalhes == null).ToList();
                foreach (var item in toRemove)
                    vm.Detalhes.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void itens_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            try
            {
                CadastroPecaViewModel vm = (CadastroPecaViewModel)DataContext;
                var confirma = MessageBox.Show("Deseja Deletar esta item?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                var item = e.Items.OfType<ConstrucaoDetalheModel>().FirstOrDefault();
                if (confirma == MessageBoxResult.Yes)
                {
                    if (item?.id_contrucao_detalhes is long id)
                    {
                        await vm.DeleteControladoAsync(id);
                    }

                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            
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

    public class CadastroPecaViewModel : INotifyPropertyChanged
    {
        private static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
        }

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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<PlanilhaConstrucaoModel>(
                    @"SELECT planilha
                      FROM projetos.tbl_planilhas_construcao
                      ORDER BY planilha;");
                return new ObservableCollection<PlanilhaConstrucaoModel>(data);
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<ProdutoModel>(
                    @"SELECT *
                      FROM producao.produtos
                      WHERE planilha = @planilha
                        AND COALESCE(inativo, '') <> '-1'
                      ORDER BY descricao;",
                    new { planilha });
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<TabelaDescAdicionalModel>(
                    @"SELECT *
                      FROM producao.tabela_desc_adicional
                      WHERE codigoproduto = @codigo
                        AND COALESCE(inativo, '') <> '-1'
                      ORDER BY descricao_adicional;",
                    new { codigo });
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<TblComplementoAdicionalModel>(
                    @"SELECT *
                      FROM producao.tblcomplementoadicional
                      WHERE coduniadicional = @coduniadicional
                        AND COALESCE(inativo, '') <> '-1'
                      ORDER BY complementoadicional;",
                    new { coduniadicional });
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
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<ConstrucaoDetalheModel>(
                    @"SELECT *
                      FROM projetos.tbl_construcao_detalhes
                      WHERE codcompladicional = @codcompladicional
                      ORDER BY item;",
                    new { codcompladicional });
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
                using var conn = CreateConnection();
                if (construcaoDetalhe.id_contrucao_detalhes.HasValue)
                {
                    await conn.ExecuteAsync(
                        @"UPDATE projetos.tbl_construcao_detalhes
                          SET codcompladicional = @codcompladicional,
                              nome_fantasia = @nome_fantasia,
                              item = @item,
                              descricao_peca = @descricao_peca,
                              volume = @volume
                          WHERE id_contrucao_detalhes = @id_contrucao_detalhes;",
                        construcaoDetalhe);
                }
                else
                {
                    construcaoDetalhe.id_contrucao_detalhes = await conn.ExecuteScalarAsync<long>(
                        @"INSERT INTO projetos.tbl_construcao_detalhes
                            (codcompladicional, nome_fantasia, item, descricao_peca, volume)
                          VALUES
                            (@codcompladicional, @nome_fantasia, @item, @descricao_peca, @volume)
                          RETURNING id_contrucao_detalhes;",
                        construcaoDetalhe);
                }

                return construcaoDetalhe;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteControladoAsync(long idContrucaoDetalhes)
        {
            try
            {
                using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    @"DELETE FROM projetos.tbl_construcao_detalhes
                      WHERE id_contrucao_detalhes = @idContrucaoDetalhes;",
                    new { idContrucaoDetalhes });
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
