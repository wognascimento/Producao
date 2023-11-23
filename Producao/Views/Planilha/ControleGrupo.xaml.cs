using Microsoft.EntityFrameworkCore;
using Producao.Views.CentralModelos;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Producao.Views.Planilha
{
    /// <summary>
    /// Interação lógica para ControleGrupo.xam
    /// </summary>
    public partial class ControleGrupo : UserControl
    {
        public ControleGrupo()
        {
            InitializeComponent();
            this.DataContext = new ControleGrupoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControleGrupoViewModel vm = (ControleGrupoViewModel)DataContext;
                vm.ControlePlanilhaGrupos = await Task.Run(vm.GetItensAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnCurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            SfDataGrid grid = (SfDataGrid)sender;
            int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            var column = grid.Columns[columnindex];
            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "os")
            {
                try
                {
                    var rowIndex = grid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
                    if (rowIndex > -1)
                    {
                        var record = (ControlePlanilhaGrupoModel)grid.View.Records[rowIndex].Data;
                        var value = record.os;
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        ControleGrupoViewModel vm = (ControleGrupoViewModel)DataContext;
                        await Task.Run(() => vm.SaveAsync(record));
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

        private async void OnRowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControleGrupoViewModel vm = (ControleGrupoViewModel)DataContext;
                ControlePlanilhaGrupoModel data = (ControlePlanilhaGrupoModel)e.RowData;
                data = await Task.Run(() => vm.SaveAsync(data));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class ControleGrupoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControlePlanilhaGrupoModel>? _controlePlanilhaGrupos;
        public ObservableCollection<ControlePlanilhaGrupoModel> ControlePlanilhaGrupos
        {
            get { return _controlePlanilhaGrupos; }
            set { _controlePlanilhaGrupos = value; RaisePropertyChanged("ControlePlanilhaGrupos"); }
        }

        private ControlePlanilhaGrupoModel? _controlePlanilhaGrupo;
        public ControlePlanilhaGrupoModel ControlePlanilhaGrupo
        {
            get { return _controlePlanilhaGrupo; }
            set { _controlePlanilhaGrupo = value; RaisePropertyChanged("ControlePlanilhaGrupo"); }
        }

        public async Task<ObservableCollection<ControlePlanilhaGrupoModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControlePlanilhaGrupos
                    .ToListAsync();
                return new ObservableCollection<ControlePlanilhaGrupoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControlePlanilhaGrupoModel> SaveAsync(ControlePlanilhaGrupoModel controle)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ControlePlanilhaGrupos.SingleUpdateAsync(controle);
                await db.SaveChangesAsync();
                return controle;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
