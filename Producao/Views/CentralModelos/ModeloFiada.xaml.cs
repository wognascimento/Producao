using Microsoft.EntityFrameworkCore;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Lógica interna para ModeloFiada.xaml
    /// </summary>
    public partial class ModeloFiada : Window
    {
        private QryModeloModel? modelo;

        public ModeloFiada(QryModeloModel? modelo)
        {
            InitializeComponent();
            this.modelo = modelo;
            this.DataContext = new ModeloFiadaViewModel();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
            vm.Modelo = this.modelo;
            vm.Modelos = new ObservableCollection<string> { "MOD. 01", "MOD. 02", "MOD. 03", "MOD. 04", "MOD. 05", "MOD. 06", "MOD. 07", "MOD. 08", "MOD. 09", "MOD. 10" };

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.ModeloFiada = await Task.Run(() => vm.GetModelosFiadaAsync(modelo));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void SfDataGrid_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
            ((ModeloFiadaModel)e.NewObject).id_modelo = modelo.id_modelo;
        }

        private void SfDataGrid_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            ModeloFiadaModel rowData = (ModeloFiadaModel)e.RowData;
            if (rowData.id_modelo == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("modelofiada", "Modelo não selecionado, Fecha e abre a Janela");
                e.ErrorMessages.Add("qtdmodelofiada", "Modelo não selecionado, Fecha e abre a Janela");
            }
            else if (rowData.modelofiada == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("modelofiada", "Seleciona o MODELO da fiada.");
            }
            else if (rowData.qtdmodelofiada == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("qtdmodelofiada", "Informa a QUANTIDADE enfeites do MODELO");
            }
        }

        private async void SfDataGrid_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
                ModeloFiadaModel data = (ModeloFiadaModel)e.RowData;
                data = await Task.Run(() => vm.SaveModelosFiadaAsync(data));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void IntegerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var field = sender as IntegerTextBox;
                var valor = Convert.ToInt32(field.Value);
                ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
                var dados = await Task.Run(() => vm.AddModeloAsync(modelo.id_modelo, valor));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class ModeloFiadaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<string>? modelos;
        public ObservableCollection<string> Modelos
        {
            get { return modelos; }
            set { modelos = value; RaisePropertyChanged("Modelos"); }
        }

        private ObservableCollection<ModeloFiadaModel>? modeloFiada;
        public ObservableCollection<ModeloFiadaModel> ModeloFiada
        {
            get { return modeloFiada; }
            set { modeloFiada = value; RaisePropertyChanged("ModeloFiada"); }
        }

        private QryModeloModel? modelo;
        public QryModeloModel Modelo
        {
            get { return modelo; }
            set { modelo = value; RaisePropertyChanged("Modelo"); }
        }

        public async Task<ObservableCollection<ModeloFiadaModel>> GetModelosFiadaAsync(QryModeloModel? modelo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ModelosFiada
                    .OrderBy(c => c.modelofiada)
                    .Where(c => c.id_modelo == modelo.id_modelo)
                    .ToListAsync();
                return new ObservableCollection<ModeloFiadaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ModeloFiadaModel> SaveModelosFiadaAsync(ModeloFiadaModel modelo)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ModelosFiada.SingleMergeAsync(modelo);
                await db.SaveChangesAsync();
                return modelo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ModeloModel> AddModeloAsync(long? id_modelo, int? qtd_fiada_cascata)
        {
            using DatabaseContext db = new();
            var strategy = db.Database.CreateExecutionStrategy();
            ModeloModel modelo = new();
            await strategy.ExecuteAsync(async () => 
            {
                var transaction = db.Database.BeginTransaction();
                try
                {
                    modelo = await db.Modelos.FindAsync(id_modelo);
                    modelo.qtd_fiada_cascata = qtd_fiada_cascata;
                    await db.Modelos.SingleMergeAsync(modelo);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            });

            return modelo;
        }


    }
}
