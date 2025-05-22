using Microsoft.EntityFrameworkCore;
using Npgsql;
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
                vm.OrdemServicos = await Task.Run(vm.GetallAsync);

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void itens_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            SfDataGrid grid = (SfDataGrid)sender;
            int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            var column = grid.Columns[columnindex];
            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "cancelar")
            {
                try
                {
                    var rowIndex = grid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
                    if (rowIndex == 0)
                    {
                        var record = (TblServicoModel)grid.View.Records[rowIndex].Data;
                        record.cancelado_por = Environment.UserName;
                        record.data_cancelamento = DateTime.Now;
                        var value = record.cancelar;
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        EmissaoServicoEmitidasViewModel vm = (EmissaoServicoEmitidasViewModel)DataContext;
                        TblServicoModel expedModel = await Task.Run(() => vm.GravarAsync(record));
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }

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

        static BaseCommand? cancelarOS;
        public static BaseCommand CancelarOS
        {
            get
            {
                cancelarOS ??= new BaseCommand(OnCancelarOSClicked);
                return cancelarOS;
            }
        }

        private static async void OnCancelarOSClicked(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as TblServicoModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var OS = grid.SelectedItem as TblServicoModel;
            EmissaoServicoEmitidasViewModel vm = (EmissaoServicoEmitidasViewModel)grid.DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(@$"{BaseSettings.CaminhoSistema}\Modelos\ORDEM_SERVICO_SERVICO_MODELO.xlsx");
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

                workbook.SaveAs(@$"{BaseSettings.CaminhoSistema}\Impressos\ORDEM_SERVICO_SERVICO_MODELO.xlsx");
                workbook.Close();

                Process.Start(new ProcessStartInfo(@$"{BaseSettings.CaminhoSistema}\Impressos\ORDEM_SERVICO_SERVICO_MODELO.xlsx")
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
