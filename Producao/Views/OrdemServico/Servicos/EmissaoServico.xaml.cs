using Microsoft.EntityFrameworkCore;
using Producao.Views.CentralModelos;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Producao.Views.OrdemServico.Servicos
{
    /// <summary>
    /// Interação lógica para EmissaoServico.xam
    /// </summary>
    public partial class EmissaoServico : UserControl
    {
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
                vm.Tipos = await Task.Run(vm.GetTiposAsync);
                vm.Setores = await Task.Run(vm.GetSetoresAsync);
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

                var OS = await Task.Run(() => vm.GravarAsync(vm.OrdemServico));

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open("Modelos/ORDEM_SERVICO_SERVICO_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];
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

                workbook.SaveAs($"Impressos/ORDEM_SERVICO_SERVICO_MODELO.xlsx");
                workbook.Close();

                Process.Start(new ProcessStartInfo($"Impressos\\ORDEM_SERVICO_SERVICO_MODELO.xlsx")
                {
                    UseShellExecute = true
                });


                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                using DatabaseContext db = new();
                //var data = await (from tipo in db.tblTipoOs select new { tipo.tipo_servico }).ToListAsync();

                var result = await db.tblTipoOs
                   .OrderBy(a => a.tipo_servico)
                   .GroupBy(cGrp => new { cGrp.tipo_servico })
                   .Select (cGrp =>  cGrp.Key.tipo_servico )
                   .ToListAsync();

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
                using DatabaseContext db = new();
                var dados = await db.SetorProducaos.OrderBy(c => c.setor).Where(c => c.inativo.Equals("0")).ToListAsync();
                var data = new ObservableCollection<SetorProducaoModel>();
                foreach (var dado in dados)
                {
                    dado.setor = $"{dado.setor} - {dado.galpao}";
                    data.Add(dado);
                }

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
                using DatabaseContext db = new();
                var data = await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1")).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.Siglas.OrderBy(c => c.sigla_serv).ToListAsync();
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
                using DatabaseContext db = new();
                await db.tblServicos.SingleMergeAsync(model);
                await db.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
