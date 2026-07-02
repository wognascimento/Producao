using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using Producao.Views.CentralModelos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Persistence.Core;

namespace Producao.Views.Controlado
{
    /// <summary>
    /// Interação lógica para VincularRequisicao.xam
    /// </summary>
    public partial class VincularRequisicao : UserControl
    {
        public VincularRequisicao()
        {
            InitializeComponent();
            DataContext = new VincularRequisicaoViewModel();
        }

        private async void OnBuscaProdutos(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    long requisicao = long.Parse(((TextBox)sender).Text);
                    VincularRequisicaoViewModel vm = (VincularRequisicaoViewModel)DataContext;
                    vm.Requisicao = await vm.GetRequisicaoAsync(requisicao);
                    if (vm.Requisicao == null)
                    {
                        MessageBox.Show("Requisição não encontrado", "Busca de requisição");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    vm.Produtos = await vm.GetProdutosAsync(requisicao);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    codigoProduto.Focus();
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void OnAdicionarProduto(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    long codigo = long.Parse(((TextBox)sender).Text);
                    VincularRequisicaoViewModel vm = (VincularRequisicaoViewModel)DataContext;
                    vm.Etiqueta = await vm.GetEtiquetaAsync(codigo);
                    if (vm.Etiqueta == null)
                    {
                        MessageBox.Show("Etiqueta não encontrado", "Busca de etiqueta");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }

                    vm.Barcode = await vm.GetBarcodeAsync(codigo);
                    if (vm.Barcode == null)
                    {
                        MessageBox.Show("Código de barras não encontrado", "Busca de Barcode");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }

                    var confirma = MessageBox.Show("Deseja Adicionar o produto na lista?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (confirma == MessageBoxResult.Yes)
                    {
                        await vm.AddControladoAsync(
                            new ControladoShoppingModel 
                            { 
                                barcode = vm.Barcode.barcode, 
                                inserido_por = Environment.UserName, 
                                inserido_em = DateTime.Now, 
                                num_requisicao = vm.Requisicao.num_requisicao
                            });
                    }

                    long requisicao = long.Parse(txtRequisicao.Text);
                    vm.Produtos = await vm.GetProdutosAsync(requisicao);

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    Producao.ErrorDialog.Show(ex, "Erro");
                }
            }
        }

        private async void OnBaixaRequisicaoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                VincularRequisicaoViewModel vm = (VincularRequisicaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var query = vm.Produtos
                  .GroupBy(x => new { x.codcompladicional, x.num_requisicao, x.planilha, x.descricao, x.descricao_adicional, x.complementoadicional, x.descricao_completa })
                  .Select(g => new TransformaRequisicaoModel
                  {
                      codcompladicional = g.Key.codcompladicional,
                      quantidade = g.Sum(x => x.quantidade),
                      num_requisicao = g.Key.num_requisicao,
                      planilha = g.Key.planilha,
                      descricao = g.Key.descricao,
                      descricao_adicional = g.Key.descricao_adicional,
                      complementoadicional = g.Key.complementoadicional,
                      descricao_completa = g.Key.descricao_completa,
                  }).ToList();

                foreach (var item in query)
                {
                    await vm.BaixaReceitaAsync(item);
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show("Baixa efetuada com sucesso!!!", "Baixa controlado.");
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

    }

    public class VincularRequisicaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<TransformaRequisicaoModel> _produtos;
        public ObservableCollection<TransformaRequisicaoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }

        private TransformaRequisicaoModel _produto;
        public TransformaRequisicaoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private RequisicaoModel _requisicao;
        public RequisicaoModel Requisicao
        {
            get { return _requisicao; }
            set { _requisicao = value; RaisePropertyChanged("Requisicao"); }
        }

        private ControladoZebraModel _etiqueta;
        public ControladoZebraModel Etiqueta
        {
            get { return _etiqueta; }
            set { _etiqueta = value; RaisePropertyChanged("Etiqueta"); }
        }

        private BarcodeModel _barcode;
        public BarcodeModel Barcode
        {
            get { return _barcode; }
            set { _barcode = value; RaisePropertyChanged("Barcode"); }
        }
        
        private ControladoShoppingModel _controladoShopping;
        public ControladoShoppingModel ControladoShopping
        {
            get { return _controladoShopping; }
            set { _controladoShopping = value; RaisePropertyChanged("ControladoShopping"); }
        }

        public async Task<RequisicaoModel> GetRequisicaoAsync(long num_requisicao)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                return await conn.QueryFirstOrDefaultAsync<RequisicaoModel>(
                    @"SELECT *
                      FROM producao.t_requisicao
                      WHERE num_requisicao = @num_requisicao;",
                    new { num_requisicao });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TransformaRequisicaoModel>> GetProdutosAsync(long nRequisicao)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<TransformaRequisicaoModel>(
                    @"SELECT *
                      FROM producao.qry_transforma_requisicao
                      WHERE num_requisicao = @nRequisicao;",
                    new { nRequisicao });
                return new ObservableCollection<TransformaRequisicaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControladoZebraModel> GetEtiquetaAsync(long codigo)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                return await conn.QueryFirstOrDefaultAsync<ControladoZebraModel>(
                    @"SELECT *
                      FROM producao.tbl_etiqueta_zebra
                      WHERE codigo = @codigo;",
                    new { codigo });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarcodeModel> GetBarcodeAsync(long codigo)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                return await conn.QueryFirstOrDefaultAsync<BarcodeModel>(
                    @"SELECT *
                      FROM producao.tbl_barcodes
                      WHERE codigo = @codigo;",
                    new { codigo });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddControladoAsync(ControladoShoppingModel controlado)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                await conn.ExecuteAsync(
                    @"INSERT INTO producao.tbl_controlado_shopping
                        (num_requisicao, barcode, inserido_por, inserido_em, retorno)
                      VALUES
                        (@num_requisicao, @barcode, @inserido_por, @inserido_em, @retorno)
                      ON CONFLICT (num_requisicao, barcode) DO UPDATE SET
                        inserido_por = EXCLUDED.inserido_por,
                        inserido_em = EXCLUDED.inserido_em,
                        retorno = EXCLUDED.retorno;",
                    controlado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteControladoAsync(long num_requisicao, string barcode)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                await conn.ExecuteAsync(
                    @"DELETE FROM producao.tbl_controlado_shopping
                      WHERE num_requisicao = @num_requisicao
                        AND barcode = @barcode;",
                    new { num_requisicao, barcode });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task BaixaReceitaAsync(TransformaRequisicaoModel model)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var parametros = new
                {
                    model.num_requisicao,
                    model.codcompladicional,
                    model.quantidade,
                    data = DateTime.Now,
                    alterado_por = Environment.UserName
                };

                var rows = await conn.ExecuteAsync(
                    @"UPDATE producao.t_detalhes_req
                      SET codcompladicional = @codcompladicional,
                          quantidade = @quantidade,
                          data = @data,
                          alterado_por = @alterado_por
                      WHERE num_requisicao = @num_requisicao
                        AND codcompladicional = @codcompladicional;",
                    parametros);

                if (rows == 0)
                {
                    await conn.ExecuteAsync(
                        @"INSERT INTO producao.t_detalhes_req
                            (num_requisicao, codcompladicional, quantidade, data, alterado_por)
                          VALUES
                            (@num_requisicao, @codcompladicional, @quantidade, @data, @alterado_por);",
                        parametros);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
