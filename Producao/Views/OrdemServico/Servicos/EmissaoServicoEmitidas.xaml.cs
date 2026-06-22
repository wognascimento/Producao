using Microsoft.EntityFrameworkCore;
using Npgsql;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Servicos
{
    /// <summary>
    /// Interação lógica para EmissaoServicoEmitidas.xam
    /// </summary>
    public partial class EmissaoServicoEmitidas : UserControl
    {
        public EmissaoServicoEmitidas()
        {
            DataContext = new EmissaoServicoEmitidasViewModel();
            InitializeComponent();
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EmissaoServicoEmitidasViewModel vm = (EmissaoServicoEmitidasViewModel)DataContext;
                vm.OrdemServicos = await vm.GetallAsync();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void itens_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction != GridViewEditAction.Commit)
            {
                return;
            }

            try
            {
                var record = e.Row.Item as TblServicoModel;
                if (record is null)
                {
                    return;
                }

                record.cancelado_por = Environment.UserName;
                record.data_cancelamento = DateTime.Now;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EmissaoServicoEmitidasViewModel vm = (EmissaoServicoEmitidasViewModel)DataContext;
                await vm.GravarAsync(record);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class EmissaoServicoEmitidasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private TblServicoModel _ordemServico;
        public TblServicoModel OrdemServico
        {
            get { return _ordemServico; }
            set { _ordemServico = value; RaisePropertyChanged("OrdemServico"); }
        }
        private ObservableCollection<TblServicoModel> _ordemServicos;
        public ObservableCollection<TblServicoModel> OrdemServicos
        {
            get { return _ordemServicos; }
            set { _ordemServicos = value; RaisePropertyChanged("OrdemServicos"); }
        }

        public async Task<ObservableCollection<TblServicoModel>> GetallAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.tblServicos.OrderBy(s => s.num_os).ToListAsync();
                return new ObservableCollection<TblServicoModel>(data);
            }
            catch (NpgsqlException)
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

    public static class ContextMenuCommandsEmissaoServicoEmitidas
    {

        static DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        static ICommand? cancelarOS;
        public static ICommand CancelarOS
        {
            get
            {
                cancelarOS ??= new RelayCommand(OnCancelarOSClicked);
                return cancelarOS;
            }
        }

        private static async void OnCancelarOSClicked(object obj)
        {
            var grid = obj as RadGridView;
            if (grid is null)
            {
                return;
            }

            var OS = grid.SelectedItem as TblServicoModel;
            if (OS is null)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("ORDEM_SERVICO_SERVICO_MODELO.xlsx"));
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

                workbook.SaveAs(BaseSettings.ResolveImpressosPath($"ORDEM_SERVICO_SERVICO_MODELO.xlsx"));
                workbook.Close();

                Process.Start(new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"ORDEM_SERVICO_SERVICO_MODELO.xlsx"))
                {
                    UseShellExecute = true
                });


                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }
    }
}
