using Producao.Views.CentralModelos.Compat;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.OrdemServico.Servicos
{
    /// <summary>
    /// Interação lógica para EmissaoServico.xam
    /// </summary>
    public partial class EmissaoServico : UserControl
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public EmissaoServico()
        {
            InitializeComponent();
            DataContext = new EmissaoServicoViewModel();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EmissaoServicoViewModel vm = (EmissaoServicoViewModel)DataContext;
                vm.OrdemServico = new TblServicoModel();
                vm.Tipos = await vm.GetTiposAsync();
                vm.Setores = await vm.GetSetoresAsync();
                vm.Planilhas = await vm.GetPlanilhasAsync();
                vm.Siglas = await vm.GetSiglasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnAdicionarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EmissaoServicoViewModel vm = (EmissaoServicoViewModel)DataContext;
                vm.OrdemServico.codigo_setor = vm.Setor.codigo_setor;
                vm.OrdemServico.data_emissao = DateTime.Now;
                vm.OrdemServico.emitido_por = Environment.UserName;
                vm.OrdemServico.emitido_por_data = DateTime.Now;
                vm.OrdemServico.quantidade = Convert.ToDouble(txtQuantidade.Text);

                var OS = await vm.GravarAsync(vm.OrdemServico);

                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("ORDEM_SERVICO_SERVICO_MODELO.xlsx"));
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);


                IWorkbook wbPt = excelEngine.Excel.Workbooks.Open(BaseSettings.ResolveModeloPath("PERMISSAO_TRABALHO.xlsx"));
                IWorksheet wsPt = wbPt.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(wsPt);



                worksheet.Range["A1"].Text = $"ORDEM DE SERVIÇO {OS.data_emissao.Value.Year} ";
                worksheet.Range["F5"].Text = OS.num_os.ToString();
                worksheet.Range["C7"].Text = OS.data_emissao.Value.ToString();
                worksheet.Range["C9"].Text = OS.tipo;
                worksheet.Range["C11"].Text = OS.descricao_setor;
                worksheet.Range["C13"].Text = OS.planilha;
                worksheet.Range["C15"].Text = OS.descricao_servico;
                worksheet.Range["C17"].Text = OS.quantidade.ToString();
                worksheet.Range["C19"].Text = OS.cliente;
                worksheet.Range["C21"].Text = OS.orientacao;
                worksheet.Range["C26"].Text = OS.data_conclusao.Value.ToString();
                worksheet.Range["C28"].Text = OS.emitido_por;

                workbook.SaveAs(BaseSettings.ResolveImpressosPath($"ORDEM_SERVICO_SERVICO_MODELO.xlsx"));
                workbook.Close();

                Process.Start(new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"ORDEM_SERVICO_SERVICO_MODELO.xlsx"))
                {
                    UseShellExecute = true
                });


                if (OS.pt == true)
                {
                    wsPt.Range["G1"].Number = (double)OS.num_os;
                    wbPt.SaveAs(BaseSettings.ResolveImpressosPath($"PERMISSAO_TRABALHO.xlsx"));

                    Process.Start(
                        new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"PERMISSAO_TRABALHO.xlsx"))
                        {
                            Verb = "Print",
                            UseShellExecute = true,
                        }
                    );

                }


                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class EmissaoServicoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private string _tipo;
        public string Tipo
        {
            get { return _tipo; }
            set { _tipo = value; RaisePropertyChanged("Tipo"); }
        }
        private TblServicoModel _ordemServico;
        public TblServicoModel OrdemServico
        {
            get { return _ordemServico; }
            set { _ordemServico = value; RaisePropertyChanged("OrdemServico"); }
        }

        private ObservableCollection<string> _tipos;
        public ObservableCollection<string> Tipos
        {
            get { return _tipos; }
            set { _tipos = value; RaisePropertyChanged("Tipos"); }
        }

        private ObservableCollection<SetorProducaoModel> _setores;
        public ObservableCollection<SetorProducaoModel> Setores
        {
            get { return _setores; }
            set { _setores = value; RaisePropertyChanged("Setores"); }
        }
        
        private SetorProducaoModel _setor;
        public SetorProducaoModel Setor
        {
            get { return _setor; }
            set { _setor = value; RaisePropertyChanged("Setor"); }
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

        private ObservableCollection<SiglaChkListModel> _siglas;
        public ObservableCollection<SiglaChkListModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }
        private SiglaChkListModel _sigla;
        public SiglaChkListModel Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged("Sigla"); }
        }

        public async Task<ObservableCollection<string>> GetTiposAsync()
        {
            try
            {
                var result = await ServicoOrdemRepository.GetTiposAsync();

                return new ObservableCollection<string>(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<SetorProducaoModel>> GetSetoresAsync()
        {
            try
            {
                var data = await ServicoOrdemRepository.GetSetoresAsync();

                return new ObservableCollection<SetorProducaoModel>(data);
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
                var data = await ServicoOrdemRepository.GetPlanilhasAsync();
                return new ObservableCollection<RelplanModel>(data);
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
                var data = await ServicoOrdemRepository.GetSiglasAsync();
                return new ObservableCollection<SiglaChkListModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TblServicoModel> GravarAsync(TblServicoModel model)
        {
            try
            {
                return await ServicoOrdemRepository.SaveServicoAsync(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

