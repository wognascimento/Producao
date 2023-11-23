using Microsoft.EntityFrameworkCore;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Producao.Views
{
    /// <summary>
    /// Interação lógica para ViewCheckListRevisao.xam
    /// </summary>
    public partial class ViewCheckListRevisao : UserControl
    {


        public ViewCheckListRevisao()
        {
            InitializeComponent();
            this.DataContext = new ViewModel();
            itens.Columns["ok"].FilterPredicates.Add(new FilterPredicate() { FilterType = FilterType.Equals, FilterValue = "0    " });
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                ViewModel vm = (ViewModel)DataContext;
                await Task.Run(vm.GetDados);
                await Task.Run(vm.GetRevisores);
                //itens.Columns["ok"].FilterPredicates.Add(new FilterPredicate() { FilterType = FilterType.Equals, FilterValue = "0    " });
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void itens_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs args)
        {
            /*
            ViewModel vm = (ViewModel)DataContext;

            var recordIndex = this.itens.ResolveToRecordIndex(args.RowColumnIndex.RowIndex);
            var columnIndex = this.itens.ResolveToGridVisibleColumnIndex(args.RowColumnIndex.ColumnIndex);
            var mappingName = this.itens.Columns[columnIndex].MappingName;
            var record = (this.itens.View.Records.GetItemAt(recordIndex) as ControleMemorialModel);
            var cellValue = this.itens.View.GetPropertyAccessProvider().GetValue(record, mappingName);

            if (mappingName == "altera_ok")
            {
                record.confirma_alteracao_por = Environment.UserName;
                record.confirma_alteracao_data = DateTime.Now;
            }
            else if(mappingName == "motivo_alt_pos_revisao")
            {
                record.ok_revisao_alterada = "-1";
                record.data_alt_revisao = DateTime.Now;
                record.revisao_alt_por = Environment.UserName;
            }
            else if (mappingName == "ok")
            {
                record.revisado_por = Environment.UserName;
                record.data_revisado_por = DateTime.Now;
                record.ok_revisao_alterada = "-1";
            }
            try
            {
                await Task.Run(async () => await vm.AtualizarControleAsync(record));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private async void itens_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {
            ViewModel vm = (ViewModel)DataContext;
            SfDataGrid? grid = sender as SfDataGrid;
            int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            var column = grid.Columns[columnindex];
            var rowIndex = grid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
            var record = grid.View.Records[rowIndex].Data as ControleMemorialModel;

            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "altera_ok")
            {
                record.confirma_alteracao_por = Environment.UserName;
                record.confirma_alteracao_data = DateTime.Now;
                //var value = record.inativo;
            }
            
            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "motivo_alt_pos_revisao")
            {
                record.ok_revisao_alterada = "-1";
                record.data_alt_revisao = DateTime.Now;
                record.revisao_alt_por = Environment.UserName;
                //var value = record.inativo;
            }
            
            if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "ok")
            {
                record.revisado_por = Environment.UserName;
                record.data_revisado_por = DateTime.Now;
                record.ok_revisao_alterada = "-1";
                //var value = record.inativo;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(() => vm.AtualizarControleAsync(record));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void itens_RowValidating(object sender, RowValidatingEventArgs e)
        {
            try
            {
                ViewModel vm = (ViewModel)DataContext;
                var record = e.RowData as ControleMemorialModel; 
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(() => vm.AtualizarControleAsync(record));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ControleMemorialModel> _dados;
        public ObservableCollection<ControleMemorialModel> Dados
        {
            get { return _dados; }
            set { _dados = value; RaisePropertyChanged("Dados"); }
        }

        private ObservableCollection<RevisorModel> _revisores;
        public ObservableCollection<RevisorModel> Revisores
        {
            get { return _revisores; }
            set { _revisores = value; RaisePropertyChanged("Revisores"); }
        }

        public ViewModel()
        {
            //Dados = new ObservableCollection<ControleMemorialModel>(); 
        }

        public async Task GetDados()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControleMemorials.ToListAsync();
                Dados = new ObservableCollection<ControleMemorialModel>(data);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task GetRevisores()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Revisores.OrderBy(r => r.revisores).ToListAsync();
                Revisores = new ObservableCollection<RevisorModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //ControleMemorialModel
        public async Task AtualizarControleAsync(ControleMemorialModel controle)
        {
            try
            {
                using DatabaseContext db = new();
                db.Entry(controle).State = controle.cod_linha_qdfecha == null ?
                                   EntityState.Added :
                                   EntityState.Modified;

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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class PrazoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as ControleMemorialModel;

            if (data == null)
                return DependencyProperty.UnsetValue;

            return data.ok.Trim().Contains('0') && data?.prazo_revisao < DateTime.Now
                ? new SolidColorBrush(Colors.Red)
                : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SiglaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
      
            var data = value as ControleMemorialModel;

            
            if (data == null)
                return DependencyProperty.UnsetValue;
            

            if (data.altera_ok.Trim().Contains('0') && data.memorial_alterado_por != null)
                return new SolidColorBrush(Colors.Yellow);
            else if(data.memorial_data_alterado > data.confirma_alteracao_data)
                return new SolidColorBrush(Colors.Red);
            else
                return DependencyProperty.UnsetValue;

            /*
            return data.altera_ok.Trim().Contains('0') && data.memorial_alterado_por != null
                ? new SolidColorBrush(Colors.Yellow)
                : DependencyProperty.UnsetValue;

            return data.memorial_data_alterado > data.confirma_alteracao_data
                ? new SolidColorBrush(Colors.Yellow)
                : DependencyProperty.UnsetValue;
            */

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
