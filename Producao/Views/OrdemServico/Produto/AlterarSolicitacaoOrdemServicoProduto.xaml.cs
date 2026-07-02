using Producao.Views.CentralModelos.Compat;
using Producao.Views.OrdemServico;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para SolicitacaoOrdemServicoProduto.xam
    /// </summary>
    public partial class AlterarSolicitacaoOrdemServicoProduto : UserControl
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

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
                vm.Setores = await vm.GetSetorsAsync();
                vm.Siglas = await vm.GetSiglasAsync();
                //vm.SelectObsOSs = new ObservableCollection<ObsOsModel>();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
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
                    vm.OrdemServico = await vm.GetOrdemServicoAsync(long.Parse(text));
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
                    vm.ObsOSs = await vm.GetCaminhosOSAsync(long.Parse(text));
                    

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private void caminhos_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            AlterarSolicitacaoOrdemServicoProdutoViewModel vm = (AlterarSolicitacaoOrdemServicoProdutoViewModel)DataContext;
            e.NewObject = new ObsOsModel
            {
                num_os_produto = vm.OrdemServico?.num_os_produto,
                cod_compl_adicional = vm.OrdemServico?.cod_compl_adicional,
                cancelar = false
            };
        }

        private async void caminhos_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction != GridViewEditAction.Commit)
                return;

            AlterarSolicitacaoOrdemServicoProdutoViewModel vm = (AlterarSolicitacaoOrdemServicoProdutoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ObsOsModel data = (ObsOsModel)e.Row.Item;
                if (data.codigo_setor.HasValue)
                    data.setor_caminho = vm.Setores?.FirstOrDefault(s => s.codigo_setor == data.codigo_setor)?.setor;

                if (!ValidarCaminho(data))
                    return;

                data.solicitado_por = Environment.UserName;
                data.solicitado_data = DateTime.Now;
                vm.ObsOs = await vm.SaveProdutoOsAsync(data);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.ObsOSs.Where(x => x.cod_obs == null).ToList();
                foreach (var item in toRemove)
                    vm.ObsOSs.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private static bool ValidarCaminho(ObsOsModel rowData)
        {
            if (!rowData.num_os_produto.HasValue)
            {
                MessageBox.Show("Não foi criado O.S para incluir o(s) caminho(s).");
                return false;
            }
            else if (!rowData.num_caminho.HasValue)
            {
                MessageBox.Show("Informe a ordem do caminho da O.S.");
                return false;
            }
            else if (!rowData.codigo_setor.HasValue)
            {
                MessageBox.Show("Seleciona o Setor da O.S.");
                return false;
            }
            else if (rowData.orientacao_caminho == "")
            {
                MessageBox.Show("Informe uma orientação para o Setor.");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(rowData.distribuir_os))
            {
                MessageBox.Show("Informe como será distribuida a O.S.");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(rowData.cliente))
            {
                MessageBox.Show("Informe o cliente da O.S.");
                return false;
            }

            return true;
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


                List<long?> list = [.. caminhos.SelectedItems.Cast<ObsOsModel>().Select(c => c.num_caminho).Distinct()];

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var servicos = await vm.GetOsEmitidas(vm.OrdemServico.num_os_produto, list);
                var printer = new OrdemServicoModeloPrinter();
                await printer.ImprimirAsync(servicos, async numOsProduto => await vm.GetServicos(numOsProduto));

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private static void PreencherBlocoSuperior(
            IWorksheet worksheet,
            OsEmissaoProducaoImprimirModel servico,
            IEnumerable<ProdutoServicoModel> setores)
        {
            worksheet.Range["E2"].Text = servico.cliente;
            worksheet.Range["G2"].Text = servico.num_os_produto.ToString();
            worksheet.Range["I2"].Text = servico.num_os_servico.ToString();
            worksheet.Range["B4"].Text = servico.tipo;
            worksheet.Range["D4"].Text = $"{servico.data_inicio:dd/MM/yy}";
            worksheet.Range["B5"].Text = servico.setor_caminho;
            worksheet.Range["F5"].Text = servico.solicitado_por;
            worksheet.Range["G4"].Text = $"META HT: {servico.meta_peca_hora}";
            worksheet.Range["B6"].Text = servico.planilha;
            worksheet.Range["F6"].Text = Convert.ToString(servico.cod_compl_adicional);
            worksheet.Range["B7"].Text = servico.descricao_completa;
            worksheet.Range["G7"].Text = $"{servico.data_de_expedicao:dd/MM/yy}";
            worksheet.Range["B9"].Text = Convert.ToString(servico.quantidade);
            worksheet.Range["D9"].Text = Convert.ToString(servico.nivel);
            worksheet.Range["B10"].Text = servico.setor_caminho_proximo;
            worksheet.Range["B11"].Text = servico.tema;
            worksheet.Range["A13"].Text = servico.orientacao_caminho;
            worksheet.Range["A23"].Text = servico.laco;
            worksheet.Range["A25"].Text = servico.obs_iluminacao;
            PreencherSetores(worksheet, setores, 9, 17);
        }

        private static void PreencherBlocoInferior(
            IWorksheet worksheet,
            OsEmissaoProducaoImprimirModel servico,
            IEnumerable<ProdutoServicoModel> setores)
        {
            worksheet.Range["E29"].Text = servico.cliente;
            worksheet.Range["G29"].Text = servico.num_os_produto.ToString();
            worksheet.Range["I29"].Text = servico.num_os_servico.ToString();
            worksheet.Range["B31"].Text = servico.tipo;
            worksheet.Range["D31"].Text = $"{servico.data_inicio:dd/MM/yy}";
            worksheet.Range["B32"].Text = servico.setor_caminho;
            worksheet.Range["F32"].Text = servico.solicitado_por;
            worksheet.Range["G31"].Text = $"META HT: {servico.meta_peca_hora}";
            worksheet.Range["B33"].Text = servico.planilha;
            worksheet.Range["F33"].Text = Convert.ToString(servico.cod_compl_adicional);
            worksheet.Range["B34"].Text = servico.descricao_completa;
            worksheet.Range["G34"].Text = $"{servico.data_de_expedicao:dd/MM/yy}";
            worksheet.Range["B36"].Text = Convert.ToString(servico.quantidade);
            worksheet.Range["D36"].Text = Convert.ToString(servico.nivel);
            worksheet.Range["B37"].Text = servico.setor_caminho_proximo;
            worksheet.Range["B38"].Text = servico.tema;
            worksheet.Range["A40"].Text = servico.orientacao_caminho;
            worksheet.Range["A50"].Text = servico.laco;
            worksheet.Range["A52"].Text = servico.obs_iluminacao;
            PreencherSetores(worksheet, setores, 37, 45);
        }

        private static void PreencherSetores(IWorksheet worksheet, IEnumerable<ProdutoServicoModel> setores, int primeiraLinha, int linhaLimite)
        {
            var linha = primeiraLinha;
            foreach (var setor in setores)
            {
                worksheet.Range[$"G{linha}"].Text = setor.setor_caminho;
                linha++;
                if (linha == linhaLimite)
                    break;
            }
        }

        private void ImprimirPermissaoTrabalhoSeNecessario(ExcelEngine excelEngine, OsEmissaoProducaoImprimirModel servico)
        {
            if (servico.pt != true)
                return;

            using IWorkbook wbPt = excelEngine.Excel.Workbooks.Open(BaseSettings.ResolveModeloPath("PERMISSAO_TRABALHO.xlsx"));
            IWorksheet wsPt = wbPt.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(wsPt);
            wsPt.Range["G1"].Number = (double)servico.num_os_servico;
            var caminhoPt = BaseSettings.ResolveImpressosPath($"PERMISSAO_TRABALHO_{servico.num_os_servico}.xlsx");
            wbPt.SaveAs(caminhoPt);
            Process.Start(new ProcessStartInfo(caminhoPt)
            {
                Verb = "Print",
                UseShellExecute = true,
            });
        }

        private static void SelecionarAbasParaImpressao(IWorkbook workbook, IWorksheet ordemServico, IWorksheet requisicao)
        {
            ordemServico.TabSelected = true;
            ordemServico.TabActive = true;
            requisicao.TabSelected = true;

            workbook.Worksheets[2].Hide();
        }

        private static void ConfigurarImpressao(IWorksheet worksheet, string areaImpressao)
        {
            Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
            worksheet.PageSetup.PrintArea = areaImpressao;
            worksheet.PageSetup.CenterHorizontally = true;
            worksheet.PageSetup.CenterVertically = false;
            worksheet.PageSetup.FitToPagesWide = 1;
            worksheet.PageSetup.FitToPagesTall = 1;
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
                var data = await ProdutoOrdemRepository.GetSetoresAsync();
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
                var data = await ProdutoOrdemRepository.GetSiglasAsync();
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
                var data = await ProdutoOrdemRepository.GetPlanilhasAsync();
                return new ObservableCollection<RelplanModel>(data);
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
                return await ProdutoOrdemRepository.GetAlteracaoSolicitacaoAsync(num_os_produto);
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
                return await ProdutoOrdemRepository.SaveProdutoOsAsync(produtoOs);
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
                return await ProdutoOrdemRepository.SaveObsOsAsync(obsOs);
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
                var data = await ProdutoOrdemRepository.GetCaminhosOsAsync(num_os_produto);
                return new ObservableCollection<ObsOsModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OsEmissaoProducaoImprimirModel>> GetOsEmitidas(long? num_os_produto, List<long?> list)
        {
            try
            {
                var data = await ProdutoOrdemRepository.GetOsEmitidasAsync(num_os_produto, list);
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
}

