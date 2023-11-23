using Microsoft.EntityFrameworkCore;
using Npgsql;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
}
