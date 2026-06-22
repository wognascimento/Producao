using Microsoft.EntityFrameworkCore;
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
                MessageBox.Show(ex.Message);
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
                using DatabaseContext db = new();
                var data = await db.OrdemServicoEmissaoAbertas.Where(x => x.cancelar == false).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.ImprimirOsS
                    .Where(i => i.num_os_servico == num_os_servico)
                    .FirstOrDefaultAsync();
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
                using DatabaseContext db = new();
                var data = await db.ProdutoServicos.OrderBy(s => s.num_os_servico).Where(i => i.num_os_produto == num_os_produto).ToListAsync();
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
            using DatabaseContext db = new();
            //var strategy = db.Database.CreateExecutionStrategy();
            //await strategy.ExecuteAsync(async () => 
            //{
                //using var transaction = db.Database.BeginTransaction();
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
                        await db.ProdutoServicos.SingleMergeAsync(produtoServicoModel);
                        await db.SaveChangesAsync();
                        var servico = await vm.GetOsEmitidas(produtoServicoModel.num_os_servico);
                        servicos.Add(servico);
                    }
                    await ImprimpirOS(servicos, vm);
                    //transaction.Commit();
                    vm.OSsAberta = await vm.GetOSsEmAbertasAsync();
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    //transaction.Rollback();
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            //});
            
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
            using DatabaseContext db = new();
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => 
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var grid = obj as RadGridView;
                    if (grid is null)
                        return;

                    var item = grid.SelectedItem as OrdemServicoEmissaoAbertaForm;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show(ex.Message);
                }
            });
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
                using DatabaseContext db = new();
                ObsOsModel? obs = await db.ObsOs.FindAsync(item.cod_obs); //Where(x => x.num_os_produto == item.num_os_produto || x.num_caminho == item.num_caminho).FirstOrDefaultAsync();
                obs.cancelar = true;
                obs.cancelado_por = Environment.UserName;
                obs.cancelado_em = DateTime.Now;

                db.Entry(obs).Property(p => p.cancelar).IsModified = true;
                db.Entry(obs).Property(p => p.cancelado_por).IsModified = true;
                db.Entry(obs).Property(p => p.cancelado_em).IsModified = true;

                await db.SaveChangesAsync();

                vm.OSsAberta = await vm.GetOSsEmAbertasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
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
