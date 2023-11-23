using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Producao.Views.PopUp;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para SolicitacaoOrdemServicoProduto.xam
    /// </summary>
    public partial class AlterarSolicitacaoOrdemServicoProduto : UserControl
    {
        public AlterarSolicitacaoOrdemServicoProduto()
        {
            InitializeComponent();
            DataContext = new AlterarSolicitacaoOrdemServicoProdutoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                AlterarSolicitacaoOrdemServicoProdutoViewModel vm = (AlterarSolicitacaoOrdemServicoProdutoViewModel)DataContext;
                vm.ObsOSs = new ObservableCollection<ObsOsModel>();
                vm.Setores = await Task.Run(vm.GetSetorsAsync);
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                //vm.SelectObsOSs = new ObservableCollection<ObsOsModel>();
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
                    AlterarSolicitacaoOrdemServicoProdutoViewModel vm = (AlterarSolicitacaoOrdemServicoProdutoViewModel)DataContext;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    string text = ((TextBox)sender).Text;
                    vm.OrdemServico = await Task.Run(() => vm.GetOrdemServicoAsync(long.Parse(text)));
                    if (vm.OrdemServico == null)
                    {
                        MessageBox.Show("O.S não encontrada", "Busca de produto");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    //tbCodproduto.Text = vm.OrdemServico.cod_compl_adicional.ToString();
                    cmbTipoOs.Text = vm.OrdemServico.tipo;
                    txtPlanilha.Text = vm.OrdemServico.planilha;
                    txtDescricao.Text = vm.OrdemServico.descricao;
                    txtDescricaoAdicional.Text = vm.OrdemServico.descricao_adicional;
                    txtComplementoAdicional.Text = vm.OrdemServico.complementoadicional;
                    txtQuantidade.Text = vm.OrdemServico.quantidade.ToString();
                    vm.ObsOSs = await Task.Run(() => vm.GetCaminhosOSAsync(long.Parse(text)));
                    

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private void caminhos_AddNewRowInitiating(object sender, AddNewRowInitiatingEventArgs e)
        {
            AlterarSolicitacaoOrdemServicoProdutoViewModel vm = (AlterarSolicitacaoOrdemServicoProdutoViewModel)DataContext;
            ((ObsOsModel)e.NewObject).num_os_produto = vm.OrdemServico?.num_os_produto;
            ((ObsOsModel)e.NewObject).cod_compl_adicional = vm.OrdemServico?.cod_compl_adicional;
            ((ObsOsModel)e.NewObject).cancelar = false;
        }

        private async void caminhos_RowValidated(object sender, RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            AlterarSolicitacaoOrdemServicoProdutoViewModel vm = (AlterarSolicitacaoOrdemServicoProdutoViewModel)DataContext;
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

        private void caminhos_CurrentCellDropDownSelectionChanged(object sender, CurrentCellDropDownSelectionChangedEventArgs e)
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

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AlterarSolicitacaoOrdemServicoProdutoViewModel vm = (AlterarSolicitacaoOrdemServicoProdutoViewModel)DataContext;

                if (caminhos.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Precisa selecionar o caminho para imprimir");
                    return;
                }
                //[0] = {Producao.ObsOsModel}

                /*List<long?> list = new List<long?>();
                foreach (ObsOsModel item in caminhos.SelectedItems.Cast<ObsOsModel>())
                    list.Add(item.num_caminho);*/


                List<long?> list = caminhos.SelectedItems.Cast<ObsOsModel>().Select(c => c.num_caminho).Distinct().ToList();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open("Modelos/ORDEM_SERVICO_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];

                var servicos = await Task.Run(() => vm.GetOsEmitidas(vm.OrdemServico.num_os_produto, list));

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

                        var setores = await Task.Run(() => vm.GetServicos(vm.OrdemServico.num_os_produto));
                        var idexSetor = 9;
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

                        var setores = await Task.Run(() => vm.GetServicos(vm.OrdemServico.num_os_produto));
                        var idexSetor = 37;
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

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                //worksheet.ShowRange(range, false);
                /*
                workbook.SaveAs(@"Impressos\ORDEM_SERVICO_MODELO.xlsx");
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

    class AlterarSolicitacaoOrdemServicoProdutoViewModel : INotifyPropertyChanged
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

        private ObservableCollection<ObsOsModel> _selectobsOSs;
        public ObservableCollection<ObsOsModel> SelectObsOSs
        {
            get { return _selectobsOSs; }
            set { _selectobsOSs = value; RaisePropertyChanged("SelectObsOSs"); }
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

        private AlteraSolicitacaoOsProducao _ordemServico;
        public AlteraSolicitacaoOsProducao OrdemServico
        {
            get { return _ordemServico; }
            set { _ordemServico = value; RaisePropertyChanged("OrdemServico"); }
        }
        private ObservableCollection<AlteraSolicitacaoOsProducao> _ordemServicos;
        public ObservableCollection<AlteraSolicitacaoOsProducao> OrdemServicos
        {
            get { return _ordemServicos; }
            set { _ordemServicos = value; RaisePropertyChanged("OrdemServicos"); }
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

        public async Task<AlteraSolicitacaoOsProducao> GetOrdemServicoAsync(long num_os_produto)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.AlteraSolicitacaoOsProducaos.Where(c => c.num_os_produto == num_os_produto).FirstOrDefaultAsync();
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

        public async Task<ObservableCollection<ObsOsModel>> GetCaminhosOSAsync(long num_os_produto)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ObsOs.OrderBy(c => c.num_caminho).Where(c => c.num_os_produto == num_os_produto).ToListAsync();
                return new ObservableCollection<ObsOsModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OsEmissaoProducaoImprimirModel>> GetOsEmitidas(long? num_os_produto, List<long?> list)
        {
            //List<long?> list = new List<long?>();
            //var customerIds = deserializedCustomers.Select(x => x.CustomerID).ToList();
            //var customers = context.Customers.Where(x => customerIds.Contains(x.CustomerID)).ToList();
            //&& ids.Contains(i.num_caminho)
            try
            {
                using DatabaseContext db = new();
                var data = await db.ImprimirOsS.OrderBy(o => o.num_os_servico).Where(i => i.num_os_produto == num_os_produto && list.Contains(i.num_caminho)).ToListAsync();
                return new ObservableCollection<OsEmissaoProducaoImprimirModel>(data);
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
                var data = await db.ProdutoServicos.Where(i => i.num_os_produto == num_os_produto).ToListAsync();
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
}
