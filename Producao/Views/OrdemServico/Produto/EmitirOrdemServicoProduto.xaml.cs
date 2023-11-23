using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                vm.OSsAberta = await Task.Run(vm.GetOSsEmAbertasAsync);
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
        static BaseCommand? emitirTodas;
        public static BaseCommand EmitirTodas
        {
            get
            {
                emitirTodas ??= new BaseCommand(OnEmitirTodasClicked);
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
                    var grid = ((GridColumnContextMenuInfo)obj).DataGrid;
                    EmitirOrdemServicoProdutoViewModel vm = (EmitirOrdemServicoProdutoViewModel)grid.DataContext;
                    var filteredResult = grid.View.Records.Select(recordentry => recordentry.Data);
                    var itens = grid.View.Records.Count;
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
                                                        }
                                                        select produtoServicoModel)
                    {
                        await db.ProdutoServicos.SingleMergeAsync(produtoServicoModel);
                        await db.SaveChangesAsync();
                        var servico = await Task.Run(() => vm.GetOsEmitidas(produtoServicoModel.num_os_servico));
                        servicos.Add(servico);
                    }
                    await Task.Run(() => ImprimpirOS(servicos, vm));
                    //transaction.Commit();
                    vm.OSsAberta = await Task.Run(vm.GetOSsEmAbertasAsync);
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

        static BaseCommand? emitir;
        public static BaseCommand Emitir
        {
            get
            {
                emitir ??= new BaseCommand(OnEmitirClicked);
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
                    var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
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

        static BaseCommand? cancelar;
        public static BaseCommand Cancelar
        {
            get
            {
                cancelar ??= new BaseCommand(OnCancelarClicked);
                return cancelar;
            }
        }

        private async static void OnCancelarClicked(object obj)
        {
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            EmitirOrdemServicoProdutoViewModel vm = (EmitirOrdemServicoProdutoViewModel)grid.DataContext;
            var item = grid.SelectedItem as OrdemServicoEmissaoAbertaForm;
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

                vm.OSsAberta = await Task.Run(vm.GetOSsEmAbertasAsync);
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
                //ModeloSetoresOrdemServicoViewModel vm = (ModeloSetoresOrdemServicoViewModel)DataContext;
                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open("Modelos/ORDEM_SERVICO_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];

                IRange range = worksheet[27, 23, 53, 23];
                if (servicos.Count == 1)
                    worksheet.ShowRange(range, false);

                var pagina = 1;
                var tot = 0;
                foreach (var servico in servicos)
                {

                    if (pagina == 1)
                    {
                        worksheet.Range["E2"].Text = servico.cliente;
                        worksheet.Range["G2"].Text = servico.num_os_produto.ToString();
                        worksheet.Range["I2"].Text = servico.num_os_servico.ToString();
                        worksheet.Range["B4"].Text = servico.tipo;
                        worksheet.Range["D4"].Text = $"{servico.data_inicio:dd/MM/yy}";
                        worksheet.Range["F4"].Text = $"{servico.data_fim:dd/MM/yy}";
                        worksheet.Range["B5"].Text = servico.setor_caminho;
                        worksheet.Range["F5"].Text = servico.solicitado_por;
                        worksheet.Range["G4"].Text = $"META HT: {servico.meta_peca_hora}";
                        worksheet.Range["B6"].Text = servico.planilha;
                        worksheet.Range["B7"].Text = servico.descricao_completa;
                        worksheet.Range["G7"].Text = $"{servico.data_de_expedicao:dd/MM/yy}";
                        worksheet.Range["B9"].Text = servico.quantidade.ToString();
                        worksheet.Range["D9"].Text = servico.nivel.ToString();
                        worksheet.Range["B10"].Text = servico.setor_caminho_proximo;
                        worksheet.Range["B11"].Text = servico.tema;
                        worksheet.Range["A13"].Text = servico.orientacao_caminho;
                        worksheet.Range["A17"].Text = servico.acabamento_construcao;
                        worksheet.Range["A19"].Text = servico.acabamento_fibra;
                        worksheet.Range["A21"].Text = servico.acabamento_moveis;
                        worksheet.Range["A23"].Text = servico.laco;
                        worksheet.Range["A25"].Text = servico.obs_iluminacao;

                        var setores = await Task.Run(() => vm.GetServicos(servico.num_os_produto));
                        var idexSetor = 9;
                        worksheet.Range["G9"].Text = "";
                        worksheet.Range["G10"].Text = "";
                        worksheet.Range["G11"].Text = "";
                        worksheet.Range["G12"].Text = "";
                        worksheet.Range["G13"].Text = "";
                        worksheet.Range["G14"].Text = "";
                        worksheet.Range["G15"].Text = "";
                        worksheet.Range["G16"].Text = "";
                        foreach (var setor in setores)
                        {
                            worksheet.Range[$"G{idexSetor}"].Text = setor.setor_caminho;
                            idexSetor++;
                            if (idexSetor == 17)
                                break;
                        }
                        pagina = 2;
                        worksheet.ShowRange(range, false);
                        workbook.SaveAs(@"Impressos\ORDEM_SERVICO_MODELO.xlsx");
                        tot++;

                        if (tot == servicos.Count)
                        {
                            Process.Start(
                            new ProcessStartInfo(@"Impressos\ORDEM_SERVICO_MODELO.xlsx")
                            {
                                Verb = "Print",
                                UseShellExecute = true,
                            });
                        }
                    }
                    else if (pagina == 2)
                    {
                        worksheet.Range["E30"].Text = servico.cliente;
                        worksheet.Range["G30"].Text = servico.num_os_produto.ToString();
                        worksheet.Range["I30"].Text = servico.num_os_servico.ToString();
                        worksheet.Range["B32"].Text = servico.tipo;
                        worksheet.Range["D32"].Text = $"{servico.data_inicio:dd/MM/yy}";
                        worksheet.Range["F32"].Text = $"{servico.data_fim:dd/MM/yy}";
                        worksheet.Range["B33"].Text = servico.setor_caminho;
                        worksheet.Range["F33"].Text = servico.solicitado_por;
                        worksheet.Range["G32"].Text = $"META HT: {servico.meta_peca_hora}";
                        worksheet.Range["B34"].Text = servico.planilha;
                        worksheet.Range["B35"].Text = servico.descricao_completa;
                        worksheet.Range["G35"].Text = $"{servico.data_de_expedicao:dd/MM/yy}";
                        worksheet.Range["B37"].Text = servico.quantidade.ToString();
                        worksheet.Range["D37"].Text = servico.nivel.ToString();
                        worksheet.Range["B38"].Text = servico.setor_caminho_proximo;
                        worksheet.Range["B39"].Text = servico.tema;
                        worksheet.Range["A41"].Text = servico.orientacao_caminho;
                        worksheet.Range["A45"].Text = servico.acabamento_construcao;
                        worksheet.Range["A47"].Text = servico.acabamento_fibra;
                        worksheet.Range["A49"].Text = servico.acabamento_moveis;
                        worksheet.Range["A51"].Text = servico.laco;
                        worksheet.Range["A53"].Text = servico.obs_iluminacao;

                        var setores = await Task.Run(() => vm.GetServicos(servico.num_os_produto));
                        var idexSetor = 37;
                        worksheet.Range["G37"].Text = "";
                        worksheet.Range["G38"].Text = "";
                        worksheet.Range["G39"].Text = "";
                        worksheet.Range["G40"].Text = "";
                        worksheet.Range["G41"].Text = "";
                        worksheet.Range["G42"].Text = "";
                        worksheet.Range["G43"].Text = "";
                        worksheet.Range["G44"].Text = "";
                        foreach (var setor in setores)
                        {
                            worksheet.Range[$"G{idexSetor}"].Text = setor.setor_caminho;
                            idexSetor++;
                            if (idexSetor == 17)
                                break;
                        }
                        pagina = 1;
                        tot++;
                        worksheet.ShowRange(range, true);
                        workbook.SaveAs(@"Impressos\ORDEM_SERVICO_MODELO.xlsx");
                        Process.Start(
                            new ProcessStartInfo(@"Impressos\ORDEM_SERVICO_MODELO.xlsx")
                            {
                                Verb = "Print",
                                UseShellExecute = true,
                            });
                    } 
                }

                //worksheet.ShowRange(range, false);
                /*workbook.SaveAs(@"Impressos\ORDEM_SERVICO_MODELO.xlsx");
                worksheet.Clear();
                workbook.Close();

                Process.Start(
                    new ProcessStartInfo(@"Impressos\ORDEM_SERVICO_MODELO.xlsx")
                    {
                        Verb = "Print",
                        UseShellExecute = true,
                    });
                */
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}
