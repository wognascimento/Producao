using Producao.Views.CentralModelos.Compat;
using Producao.Views.OrdemServico;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interação lógica para EmitirOrdemServicoProduto.xam
    /// </summary>
    public partial class EmitirOrdemServicoProduto : UserControl
    {

        public EmitirOrdemServicoProduto()
        {
            InitializeComponent();
            DataContext = new EmitirOrdemServicoProdutoViewModel();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                EmitirOrdemServicoProdutoViewModel vm = (EmitirOrdemServicoProdutoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.OSsAberta = await vm.GetOSsEmAbertasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class EmitirOrdemServicoProdutoViewModel : INotifyPropertyChanged
    {

        private OrdemServicoEmissaoAbertaForm _osAberta;
        public OrdemServicoEmissaoAbertaForm OsAberta
        {
            get { return _osAberta; }
            set { _osAberta = value; RaisePropertyChanged("OsAberta"); }
        }
        private ObservableCollection<OrdemServicoEmissaoAbertaForm> _ossAberta;
        public ObservableCollection<OrdemServicoEmissaoAbertaForm> OSsAberta
        {
            get { return _ossAberta; }
            set { _ossAberta = value; RaisePropertyChanged("OSsAberta"); }
        }

        public async Task<ObservableCollection<OrdemServicoEmissaoAbertaForm>> GetOSsEmAbertasAsync()
        {
            try
            {
                var data = await ProdutoOrdemRepository.GetOrdensAbertasAsync();
                return new ObservableCollection<OrdemServicoEmissaoAbertaForm>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OsEmissaoProducaoImprimirModel> GetOsEmitidas(long? num_os_servico)
        {
            try
            {
                var data = await ProdutoOrdemRepository.GetOsEmitidaAsync(num_os_servico);
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProdutoServicoModel>> GetServicos(long? num_os_produto)
        {
            try
            {
                var data = await ProdutoOrdemRepository.GetServicosAsync(num_os_produto);
                return new ObservableCollection<ProdutoServicoModel>(data);
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

    public static class ContextMenuCommands
    {

        static DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        static ICommand? emitirTodas;
        public static ICommand EmitirTodas
        {
            get
            {
                emitirTodas ??= new RelayCommand(OnEmitirTodasClicked);
                return emitirTodas;
            }
        }

        private async static void OnEmitirTodasClicked(object obj)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var grid = obj as RadGridView;
                if (grid is null)
                    return;

                EmitirOrdemServicoProdutoViewModel vm = (EmitirOrdemServicoProdutoViewModel)grid.DataContext;
                var filteredResult = grid.Items.OfType<OrdemServicoEmissaoAbertaForm>().ToList();
                var servicos = new ObservableCollection<OsEmissaoProducaoImprimirModel>();
                foreach (var produtoServicoModel in from OrdemServicoEmissaoAbertaForm item in filteredResult
                                                    let produtoServicoModel = new ProdutoServicoModel
                                                    {
                                                        num_os_produto = item.num_os_produto,
                                                        tipo = item.tipo,
                                                        codigo_setor = item.codigo_setor,
                                                        setor_caminho = item.setor_caminho,
                                                        quantidade = item.quantidade,
                                                        data_inicio = DateTime.Now,
                                                        data_fim = DateTime.Now.AddDays(15),
                                                        cliente = item.cliente,
                                                        tema = item.tema,
                                                        orientacao_caminho = item.orientacao_caminho,
                                                        codigo_setor_proximo = 39,
                                                        setor_caminho_proximo = "FINAL - TODOS",
                                                        fase = "PRODUÇÃO",
                                                        responsavel_emissao_os = Environment.UserName,
                                                        emitida_por = Environment.UserName,
                                                        emitida_data = DateTime.Now,
                                                        turno = "DIURNO",
                                                        id_modelo = item.id_modelo,
                                                        pt = item.pt,
                                                    }
                                                    select produtoServicoModel)
                {
                    await ProdutoOrdemRepository.SaveProdutoServicoAsync(produtoServicoModel);
                    var servico = await vm.GetOsEmitidas(produtoServicoModel.num_os_servico);
                    if (servico is not null)
                        servicos.Add(servico);
                }
                await ImprimpirOS(servicos, vm);
                vm.OSsAberta = await vm.GetOSsEmAbertasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            
        }

        static ICommand? emitir;
        public static ICommand Emitir
        {
            get
            {
                emitir ??= new RelayCommand(OnEmitirClicked);
                return emitir;
            }
        }

        private async static void OnEmitirClicked(object obj)
        {
            var grid = obj as RadGridView;
            if (grid is null)
                return;

            var item = grid.SelectedItem as OrdemServicoEmissaoAbertaForm;
            if (item is null)
                return;
        }

        static ICommand? cancelar;
        public static ICommand Cancelar
        {
            get
            {
                cancelar ??= new RelayCommand(OnCancelarClicked);
                return cancelar;
            }
        }

        private async static void OnCancelarClicked(object obj)
        {
            var grid = obj as RadGridView;
            if (grid is null)
                return;

            EmitirOrdemServicoProdutoViewModel vm = (EmitirOrdemServicoProdutoViewModel)grid.DataContext;
            var item = grid.SelectedItem as OrdemServicoEmissaoAbertaForm;
            if (item is null)
                return;
            try
            {
                var mensage = MessageBox.Show("Deseja cancelar essa solicitação?", "Cacelar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mensage == MessageBoxResult.No)
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await ProdutoOrdemRepository.CancelarObsAsync(item.cod_obs, Environment.UserName, DateTime.Now);

                vm.OSsAberta = await vm.GetOSsEmAbertasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }

        }


        private static async Task ImprimpirOS(ObservableCollection<OsEmissaoProducaoImprimirModel> servicos, EmitirOrdemServicoProdutoViewModel vm)
        {
            try
            {
                var printer = new OrdemServicoModeloPrinter();
                await printer.ImprimirAsync(servicos, async numOsProduto => await vm.GetServicos(numOsProduto));
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}

