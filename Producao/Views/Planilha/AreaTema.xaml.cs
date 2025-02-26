using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Producao.DataBase.Model;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Persistence.Core;

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

        private async void OnRowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var registro = e.RowData as TblAreaTemaModel;
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
                MessageBox.Show(ex.InnerException?.Message);

                if (gridAreaTema.IsAddNewIndex(e.RowIndex))
                    gridAreaTema.View.Remove(registro);
                else
                    gridAreaTema.View.Refresh();
            }
            catch (Exception ex)
            {
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);

                if (gridAreaTema.IsAddNewIndex(e.RowIndex))
                    gridAreaTema.View.Remove(registro);
                else
                    gridAreaTema.View.Refresh();
            }
        }

        private void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {

        }

        private void gridAreaTema_CurrentCellDropDownSelectionChanged(object sender, CurrentCellDropDownSelectionChangedEventArgs e)
        {
            
            var sfdatagrid = sender as SfDataGrid;
            var viewModel = (AreaTemaViewModel)sfdatagrid.DataContext;
            //int rowIndex = sfdatagrid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
            /*
            TblAreaTemaModel record;

            if (rowIndex == -1)
                record = (TblAreaTemaModel)sfdatagrid.View.CurrentAddItem;
            else
                record = (TblAreaTemaModel)(sfdatagrid.View.Records[rowIndex] as RecordEntry).Data;


            if (sfdatagrid.CurrentColumn.MappingName == "sigla")
                record.tema = ((AprovadoModel)e.SelectedItem).Tema;

            sfdatagrid.UpdateDataRow(e.RowColumnIndex.RowIndex);
            */

            if (gridAreaTema.CurrentColumn.MappingName == "sigla")
            {
                var rowIndex = gridAreaTema.SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;

                // Obtém a ViewModel (ajuste conforme seu DataContext)
                var listaFuncionarios = (ObservableCollection<TblAreaTemaModel>)gridAreaTema.ItemsSource;

                TblAreaTemaModel? registro = null;

                // Verifica se é uma linha nova sendo editada
                if (gridAreaTema.IsAddNewIndex(rowIndex))
                {
                    // Se for uma nova linha, obtemos o objeto que está sendo editado
                    registro = gridAreaTema.View.CurrentAddItem as TblAreaTemaModel;
                }
                else if (rowIndex > 0 && rowIndex <= listaFuncionarios.Count)
                {
                    // Se for uma linha existente, acessamos pelo ItemsSource
                    registro = listaFuncionarios[rowIndex - 1];
                }

                if (registro != null)
                {
                    // Obtém o nome selecionado no ComboBox
                    string? nomeSelecionado = ((AprovadoModel)e.SelectedItem).SiglaServ;

                    // Busca o setor correspondente ao funcionário selecionado
                    var funcionario = ((AreaTemaViewModel)this.DataContext).Siglas.FirstOrDefault(f => f.SiglaServ == nomeSelecionado);

                    if (funcionario != null)
                    {
                        registro.tema = funcionario.Tema;
                        //sfdatagrid.UpdateDataRow(e.RowColumnIndex.RowIndex);
                        gridAreaTema.UpdateDataRow(rowIndex);
                    }
                }
            }
        }

        private void OnCurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {

        }

        private void gridAreaTema_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            var sfDataGrid = e.OriginalSender as SfDataGrid;
            var dataGrid = sender as SfDataGrid;
            var datarow = sfDataGrid.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == e.RowColumnIndex.RowIndex);
            var dodos = datarow.RowData as TblAreaTemaModel;
            if (datarow.RowData is TblAreaTemaModel currentItem)
            {
                // Atualizar a coluna CenografiaPlanta com base em ConstrucaoTotal
                if (sfDataGrid.CurrentColumn.MappingName == "construcao_total")
                {
                    var calculo = dodos.area_total_planta - dodos.trilha_planta - dodos.pa - dodos.construcao_total;
                    currentItem.cenografia_planta = calculo;
                    sfDataGrid.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    sfDataGrid.View.CommitEdit();
                }
            }
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
