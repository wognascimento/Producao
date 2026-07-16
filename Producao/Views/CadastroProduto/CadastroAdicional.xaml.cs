using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.CadastroProduto
{
    /// <summary>
    /// Interação lógica para CadastroAdicional.xam
    /// </summary>
    public partial class CadastroAdicional : Window
    {
        private readonly ProdutoModel produto;

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

                var vm = (CadastroAdicionalViewModel)DataContext;
                vm.ProdutosAdicionais = await vm.GetDescricaoAdicionaisAsync(produto.codigo);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnAddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.NewObject = new TabelaDescAdicionalModel
            {
                codigoproduto = produto.codigo,
                inativo = "0"
            };
        }

        private async void OnInativoClick(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.CheckBox { DataContext: TabelaDescAdicionalModel adicional } ||
                adicional.coduniadicional is null or 0)
            {
                return;
            }

            var vm = (CadastroAdicionalViewModel)DataContext;
            try
            {
                adicional.inativo = NormalizarInativo(adicional.inativo);
                adicional.alteradopor = Environment.UserName;
                adicional.alteradoem = DateTime.Now;
                await vm.UpdateInativoAsync(adicional);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro ao alterar INATIVO");
            }
        }

        private async void OnRowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            if (e.Row?.Item is not TabelaDescAdicionalModel data)
                return;

            var vm = (CadastroAdicionalViewModel)DataContext;
            var isInsert = data.coduniadicional is null or 0;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                data.codigoproduto = produto.codigo;
                data.cadastradopor = isInsert ? Environment.UserName : data.cadastradopor;
                data.cadastradoem = isInsert ? DateTime.Now : data.cadastradoem;
                data.alteradopor = isInsert ? null : Environment.UserName;
                data.alteradoem = isInsert ? null : DateTime.Now;
                data.inativo = string.IsNullOrWhiteSpace(data.inativo) ? "0" : data.inativo;

                var saved = await vm.SaveAsync(data);
                data.coduniadicional = saved.coduniadicional;
                adicionais.Items.Refresh();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.ProdutosAdicionais.Where(x => x.coduniadicional is null or 0).ToList();
                foreach (var item in toRemove)
                    vm.ProdutosAdicionais.Remove(item);
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row?.Item is not TabelaDescAdicionalModel rowData)
                return;

            if (produto.codigo is null or 0)
                AddValidation(e, nameof(TabelaDescAdicionalModel.coduniadicional), "Produto não selecionado");

            if (string.IsNullOrWhiteSpace(rowData.descricao_adicional))
                AddValidation(e, nameof(TabelaDescAdicionalModel.descricao_adicional), "Informe a DESCRIÇÃO ADICIONAL");
        }

        private void OnComplementoClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CadastroAdicionalViewModel vm ||
                sender is not RadButton { DataContext: TabelaDescAdicionalModel adicional })
            {
                return;
            }

            vm.ProdutoAdicional = adicional;

            if (adicional.coduniadicional is null or 0)
            {
                RadWindow.Alert("Grave a descrição adicional antes de abrir os complementos.");
                return;
            }

            if (vm.RowDataCommand?.CanExecute(adicional) == true)
                vm.RowDataCommand.Execute(adicional);
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.IsValid = false;
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = propertyName,
                ErrorMessage = message
            });
        }

        private static string NormalizarInativo(string? valor)
        {
            return string.Equals(valor?.Trim(), "-1", StringComparison.OrdinalIgnoreCase) ? "-1" : "0";
        }
    }

    public class CadastroAdicionalViewModel : INotifyPropertyChanged
    {
        private readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private TabelaDescAdicionalModel _produtoAdicional;
        public TabelaDescAdicionalModel ProdutoAdicional
        {
            get { return _produtoAdicional; }
            set { _produtoAdicional = value; RaisePropertyChanged(nameof(ProdutoAdicional)); }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _produtosAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> ProdutosAdicionais
        {
            get { return _produtosAdicionais; }
            set { _produtosAdicionais = value; RaisePropertyChanged(nameof(ProdutosAdicionais)); }
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

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(BaseSettings.ConnectionString);
        }

        public void ChangeCanExecute(object obj)
        {
            try
            {
                var adicional = obj as TabelaDescAdicionalModel ?? ProdutoAdicional;
                if (adicional == null)
                    return;

                if (adicional.coduniadicional is null or 0)
                {
                    RadWindow.Alert("Grave a descrição adicional antes de abrir os complementos.");
                    return;
                }

                var window = new CadastroCompmento(adicional)
                {
                    Title = $"Complemento Adicional da Descrição Adicional -> {adicional.descricao_adicional}",
                    Owner = App.Current.MainWindow,
                    Height = 450,
                    Width = 900
                };
                if (window.ShowDialog() == true) { }
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        public async Task<ObservableCollection<TabelaDescAdicionalModel>> GetDescricaoAdicionaisAsync(long? codigoproduto)
        {
            using var conn = CreateConnection();
            var data = await conn.QueryAsync<TabelaDescAdicionalModel>(
                """
                SELECT *
                FROM producao.tabela_desc_adicional
                WHERE codigoproduto = @codigoproduto
                ORDER BY descricao_adicional;
                """,
                new { codigoproduto });

            return new ObservableCollection<TabelaDescAdicionalModel>(data);
        }

        public async Task<TabelaDescAdicionalModel> SaveAsync(TabelaDescAdicionalModel adicional)
        {
            using var conn = CreateConnection();

            if (adicional.coduniadicional is null or 0)
            {
                adicional.coduniadicional = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO producao.tabela_desc_adicional
                        (codigoproduto, descricao_adicional, cadastradopor, cadastradoem, revisao,
                         obsproducaoobrigatoria, obsmontagem, unidade, inativo)
                    VALUES
                        (@codigoproduto, @descricao_adicional, @cadastradopor, @cadastradoem, @revisao,
                         @obsproducaoobrigatoria, @obsmontagem, @unidade, @inativo)
                    RETURNING coduniadicional;
                    """,
                    adicional);
            }
            else
            {
                await conn.ExecuteAsync(
                    """
                    UPDATE producao.tabela_desc_adicional
                    SET descricao_adicional = @descricao_adicional,
                        revisao = @revisao,
                        obsproducaoobrigatoria = @obsproducaoobrigatoria,
                        obsmontagem = @obsmontagem,
                        unidade = @unidade,
                        inativo = @inativo,
                        alteradopor = @alteradopor,
                        alteradoem = @alteradoem
                    WHERE coduniadicional = @coduniadicional;
                    """,
                    adicional);
            }

            return adicional;
        }

        public async Task UpdateInativoAsync(TabelaDescAdicionalModel adicional)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                """
                UPDATE producao.tabela_desc_adicional
                SET inativo = @inativo,
                    alteradopor = @alteradopor,
                    alteradoem = @alteradoem
                WHERE coduniadicional = @coduniadicional;
                """,
                adicional);
        }
    }
}
