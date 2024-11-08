using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
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
    /// Interação lógica para AreaTema.xam
    /// </summary>
    public partial class AreaTema : UserControl
    {
        public AreaTema()
        {
            InitializeComponent();
            this.DataContext = new AreaTemaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                AreaTemaViewModel vm = (AreaTemaViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                vm.AreaTemas = await vm.GeAreaTemasAsync();
                //vm.Locais = ["Praça principal", "Trono", "Presépio", "Mini-Cenário", "Oficina", "Fachada"];
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnCurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {

        }

        private async void OnRowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                AreaTemaViewModel vm = (AreaTemaViewModel)DataContext;
                TblAreaTemaModel data = (TblAreaTemaModel)e.RowData;
                data = await vm.SaveAsync(data);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (DbUpdateException ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.InnerException.Message);
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

        private void gridAreaTema_CurrentCellDropDownSelectionChanged(object sender, CurrentCellDropDownSelectionChangedEventArgs e)
        {
            
            var sfdatagrid = sender as SfDataGrid;
            var viewModel = (AreaTemaViewModel)sfdatagrid.DataContext;
            int rowIndex = sfdatagrid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);

            TblAreaTemaModel record;

            if (rowIndex == -1)
                record = (TblAreaTemaModel)sfdatagrid.View.CurrentAddItem;
            else
                record = (TblAreaTemaModel)(sfdatagrid.View.Records[rowIndex] as RecordEntry).Data;


            if (sfdatagrid.CurrentColumn.MappingName == "sigla")
                record.tema = ((AprovadoModel)e.SelectedItem).Tema;

            sfdatagrid.UpdateDataRow(e.RowColumnIndex.RowIndex);


        }

        private void gridAreaTema_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            var sfDataGrid = e.OriginalSender as SfDataGrid;
            if (sfDataGrid.CurrentColumn.MappingName != "sigla")
                return;
            var datarow = sfDataGrid.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == e.RowColumnIndex.RowIndex);
            datarow.Element.DataContext = null;
            sfDataGrid.UpdateDataRow(e.RowColumnIndex.RowIndex);
        }
    }

    public class AreaTemaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<AprovadoModel>? _siglas;
        public ObservableCollection<AprovadoModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }

        private ObservableCollection<string>? _locais = ["Praça principal", "Trono", "Presépio", "Mini-Cenário", "Oficina", "Fachada"];
        public ObservableCollection<string> Locais
        {
            get { return _locais; }
            set { _locais = value; RaisePropertyChanged("Locais"); }
        }

        private ObservableCollection<TblAreaTemaModel>? _areaTemas;
        public ObservableCollection<TblAreaTemaModel> AreaTemas
        {
            get { return _areaTemas; }
            set { _areaTemas = value; RaisePropertyChanged("AreaTemas"); }
        }

        private TblAreaTemaModel? _areaTema;
        public TblAreaTemaModel AreaTema
        {
            get { return _areaTema; }
            set { _areaTema = value; RaisePropertyChanged("AreaTema"); }
        }

        public async Task<ObservableCollection<AprovadoModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Aprovados
                    .OrderBy(a => a.SiglaServ)
                    .ToListAsync();
                return new ObservableCollection<AprovadoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<string>> GeTemasAsync(string sigla)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Aprovados
                    .Where(a => a.SiglaServ == sigla)
                    .OrderBy(a => a.Tema)
                    .GroupBy(a => a.Tema)
                    .Select(a => a.Key)
                    .ToListAsync();
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ObservableCollection<TblAreaTemaModel>> GeAreaTemasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.AreaTemas
                    .ToListAsync();
                return new ObservableCollection<TblAreaTemaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TblAreaTemaModel> SaveAsync(TblAreaTemaModel areaTema)
        {
            try
            {
                using DatabaseContext db = new();
                //await db.AreaTemas.AddAsync(areaTema);
                //await db.SaveChangesAsync();

                db.Entry(areaTema).State = areaTema.codareatema == 0 ?
                                   EntityState.Added :
                                   EntityState.Modified;

                db.SaveChanges();

                return areaTema;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
