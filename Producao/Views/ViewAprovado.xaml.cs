using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views
{
    /// <summary>
    /// Interação lógica para ViewAprovado.xam
    /// </summary>
    public partial class ViewAprovado : UserControl
    {
        public ObservableCollection<AprovadoModel> AprovadosList{ get; set; }

        public ViewAprovado()
        {
            InitializeComponent();
            this.DataContext = new ViewAprovadoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewAprovadoViewModel vm = (ViewAprovadoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Aprovados = await Task.Run(vm.GetAprovados);
                //AprovadosList = await Task.Run(async () => await new AprovadoViewModel().GetAprovados());
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
            
        }

        private void OnCurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            
        }

        private void OnCurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {
            /*
            SfDataGrid grid = sender as SfDataGrid;
            int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            var column = grid.Columns[columnindex];
            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "IsDelivered")
            {
                var rowIndex = grid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
                var record = grid.View.Records[rowIndex].Data as OrderInfo;

                var value = record.IsDelivered;
            }
            */

            var dataGrid = sender as SfDataGrid;

            var provider = dataGrid.View.GetPropertyAccessProvider();

            //here get the information of the changed row by using RowIndex
            AprovadoModel record = dataGrid.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex) as AprovadoModel;

            //if (record == null)
            //    return;

            //here get the column by using column Index
            var column = dataGrid.Columns[dataGrid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex)];

            //here get the changed cell value
            //var cellValue = provider.GetValue(record, column.MappingName);

            //MessageBox.Show("Changed data in a row : " + cellValue);

            var dataRow = this.itens.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == e.RowColumnIndex.RowIndex);
            

            if (column.MappingName.Equals("OkPlantaPca"))
            {
                record.PlantaPca = Environment.UserName;
                record.LiberacaoPlantaPca = DateTimeOffset.Now;

                (dataRow.RowData as AprovadoModel).PlantaPca = Environment.UserName;
                (dataRow.RowData as AprovadoModel).LiberacaoPlantaPca = DateTimeOffset.Now;
            }
            else if (column.MappingName.Equals("OkPlantaBase"))
            {
                record.PlantaBase = Environment.UserName;
                record.LiberacaoPlantaBase = DateTimeOffset.Now;
            }
            else if (column.MappingName.Equals("OkPlantaMall"))
            {
                record.PlantaMall = Environment.UserName;
                record.ConclusaoPlantaMall = DateTimeOffset.Now;
            }
            else if (column.MappingName.Equals("OkPlantaFachada"))
            {
                record.PlantaFachada = Environment.UserName;
                record.ConclusaoPlantaFachada = DateTimeOffset.Now;
            }

            try
            {
                //await new AprovadoViewModel().SaveAsync(record);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

            //MessageBox.Show("Changed data in a row : ");

        }

        private async void itens_RowValidated(object sender, RowValidatedEventArgs e)
        {
            try
            {
                var sfdatagrid = sender as SfDataGrid;
                ViewAprovadoViewModel vm = (ViewAprovadoViewModel)DataContext;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var record = sfdatagrid.View.CurrentEditItem as AprovadoModel;
                await Task.Run(() => vm.SaveAsync(record));

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

    public class ViewAprovadoViewModel : INotifyPropertyChanged
    {
        private AprovadoModel _aprovado;
        public AprovadoModel Aprovado
        {
            get { return _aprovado; }
            set { _aprovado = value; RaisePropertyChanged("Aprovado"); }
        }
        private ObservableCollection<AprovadoModel> _aprovados;
        public ObservableCollection<AprovadoModel> Aprovados
        {
            get { return _aprovados; }
            set { _aprovados = value; RaisePropertyChanged("Aprovados"); }
        }

        public async Task<ObservableCollection<AprovadoModel>> GetAprovados()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Aprovados.OrderBy(c => c.Ordem).ToListAsync();
                return new ObservableCollection<AprovadoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveAsync(AprovadoModel aprovado)
        {
            try
            {
                using DatabaseContext db = new();
                AprovadoModel found = await db.Aprovados.FindAsync(aprovado.IdAprovado);
                db.Entry(found).CurrentValues.SetValues(aprovado);
                await db.SaveChangesAsync();
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
