using Microsoft.EntityFrameworkCore;
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

namespace Producao.Views.Estoque
{
    /// <summary>
    /// Interação lógica para DigitacaoCCE.xam
    /// </summary>
    public partial class DigitacaoCCE : UserControl
    {
        public DigitacaoCCE()
        {
            InitializeComponent();
            DataContext = new DigitacaoCCEViewModel();
        }

        private async void OnBuscarLancamentoSemana(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    DigitacaoCCEViewModel vm = (DigitacaoCCEViewModel)DataContext;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    string text = ((TextBox)sender).Text;
                    vm.Itens = await Task.Run(() => vm.GetItensAsync(int.Parse(text)));
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void itens_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            DigitacaoCCEViewModel vm = (DigitacaoCCEViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ContaProcessSemanaModel data = (ContaProcessSemanaModel)e.RowData;
                var barcode = await Task.Run(() => vm.GetBarcodeAsync(data.cod_compladicional));
                var cce = new ContaProcessSemanaModel
                {
                    cod_movimento = data.cod_movimento,
                    cod_compladicional = data.cod_compladicional,
                    barcode = barcode.barcode,
                    quantidade = data.quantidade,
                    semana = data.semana,
                    digitado_por = data.digitado_por,
                    digitado_data = data.digitado_data,
                    galpao = "JAC"
                };


                data = await Task.Run(() => vm.SaveCCEAsync(cce));
                var record = sfdatagrid.View.CurrentAddItem as ContaProcessSemanaModel;
                sfdatagrid.View.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void itens_AddNewRowInitiating(object sender, AddNewRowInitiatingEventArgs e)
        {
            var data = e.NewObject as ContaProcessSemanaModel;
            data.semana = int.Parse(tbCodproduto.Text);
            data.digitado_por = Environment.UserName;
            data.digitado_data = DateTime.Now;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class DigitacaoCCEViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<ContaProcessSemanaModel> _itens;
        public ObservableCollection<ContaProcessSemanaModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        public async Task<ObservableCollection<ContaProcessSemanaModel>> GetItensAsync(int semana)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ContaProcessSemanas
                    .Where(c => c.semana == semana)
                    .ToListAsync();
                return new ObservableCollection<ContaProcessSemanaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ContaProcessSemanaModel> SaveCCEAsync(ContaProcessSemanaModel cce)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ContaProcessSemanas.SingleMergeAsync(cce);
                await db.SaveChangesAsync();
                return cce;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarcodeModel> GetBarcodeAsync(long? codigo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Barcodes
                    .Where(c => c.codigo == codigo)
                    .FirstOrDefaultAsync();
                return data;
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
