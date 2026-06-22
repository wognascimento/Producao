using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.CentralModelos
{
    public partial class ModeloFiada : Window
    {
        private QryModeloModel? modelo;

        public ModeloFiada(QryModeloModel? modelo)
        {
            InitializeComponent();
            this.modelo = modelo;
            DataContext = new ModeloFiadaViewModel();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
            vm.Modelo = modelo;
            vm.Modelos = new ObservableCollection<string> { "MOD. 01", "MOD. 02", "MOD. 03", "MOD. 04", "MOD. 05", "MOD. 06", "MOD. 07", "MOD. 08", "MOD. 09", "MOD. 10" };

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                vm.ModeloFiada = await vm.GetModelosFiadaAsync(modelo);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                MessageBox.Show(ex.Message);
            }
        }

        private void OnAddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            if (modelo == null)
            {
                return;
            }

            ((ModeloFiadaModel)e.NewObject).id_modelo = modelo.id_modelo;
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row?.Item is not ModeloFiadaModel rowData)
            {
                return;
            }

            if (rowData.id_modelo == null)
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloFiadaModel.modelofiada), "Modelo não selecionado. Feche e abra a janela novamente.");
            }
            else if (string.IsNullOrWhiteSpace(rowData.modelofiada))
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloFiadaModel.modelofiada), "Selecione o modelo da fiada.");
            }
            else if (rowData.qtdmodelofiada == null)
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloFiadaModel.qtdmodelofiada), "Informe a quantidade de enfeites do modelo.");
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditedItem is not ModeloFiadaModel data)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
                await vm.SaveModelosFiadaAsync(data);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                MessageBox.Show(ex.Message);
            }
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = propertyName,
                ErrorMessage = message
            });
        }

        private async void QtdFiada_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (!IsLoaded || modelo?.id_modelo == null)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                var valor = e.NewValue.HasValue ? Convert.ToInt32(e.NewValue.Value) : (int?)null;
                ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
                await vm.AddModeloAsync(modelo.id_modelo, valor);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class ModeloFiadaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<string>? modelos;
        public ObservableCollection<string> Modelos
        {
            get => modelos;
            set { modelos = value; RaisePropertyChanged(nameof(Modelos)); }
        }

        private ObservableCollection<ModeloFiadaModel>? modeloFiada;
        public ObservableCollection<ModeloFiadaModel> ModeloFiada
        {
            get => modeloFiada;
            set { modeloFiada = value; RaisePropertyChanged(nameof(ModeloFiada)); }
        }

        private QryModeloModel? modelo;
        public QryModeloModel Modelo
        {
            get => modelo;
            set { modelo = value; RaisePropertyChanged(nameof(Modelo)); }
        }

        public async Task<ObservableCollection<ModeloFiadaModel>> GetModelosFiadaAsync(QryModeloModel? modelo)
        {
            using DatabaseContext db = new();
            var data = await db.ModelosFiada
                .OrderBy(c => c.modelofiada)
                .Where(c => c.id_modelo == modelo.id_modelo)
                .ToListAsync();
            return new ObservableCollection<ModeloFiadaModel>(data);
        }

        public async Task<ModeloFiadaModel> SaveModelosFiadaAsync(ModeloFiadaModel modelo)
        {
            using DatabaseContext db = new();
            await db.ModelosFiada.SingleMergeAsync(modelo);
            await db.SaveChangesAsync();
            return modelo;
        }

        public async Task<ModeloModel> AddModeloAsync(long? id_modelo, int? qtd_fiada_cascata)
        {
            using DatabaseContext db = new();
            var strategy = db.Database.CreateExecutionStrategy();
            ModeloModel modelo = new();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    modelo = await db.Modelos.FindAsync(id_modelo);
                    modelo.qtd_fiada_cascata = qtd_fiada_cascata;
                    await db.Modelos.SingleMergeAsync(modelo);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });

            return modelo;
        }
    }
}
