using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CentralModelos
{
    public partial class ViewCentralStatusCheckList : UserControl
    {
        public ViewCentralStatusCheckList()
        {
            InitializeComponent();
            DataContext = new ViewCentralStatusCheckListViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralStatusCheckListViewModel vm = (ViewCentralStatusCheckListViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private async void OnGerarModeloClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { DataContext: StatusChkGeralCentralModel record })
            {
                return;
            }

            if (record.status != "falta modelo")
            {
                return;
            }

            ViewCentralStatusCheckListViewModel vm = (ViewCentralStatusCheckListViewModel)DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                var dados = new ModeloModel
                {
                    codcompladicional = record.codcompladicional,
                    tema = record.tema,
                    cadastrado_por = Environment.UserName,
                    data_cadastro = DateTime.Now
                };

                vm.Modelo = await vm.AddModeloAsync(dados, record.idtema);
                var modelo = await vm.GetModelo(vm.Modelo.id_modelo);

                vm.QryModelos = new ObservableCollection<QryModeloModel> { modelo };
                var window = new ModeloReceita(modelo)
                {
                    Owner = App.Current.MainWindow
                };
                window.ShowDialog();
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public class ViewCentralStatusCheckListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private StatusChkGeralCentralModel? item;
        public StatusChkGeralCentralModel? Item
        {
            get => item;
            set { item = value; RaisePropertyChanged(nameof(Item)); }
        }

        private ObservableCollection<StatusChkGeralCentralModel>? itens;
        public ObservableCollection<StatusChkGeralCentralModel>? Itens
        {
            get => itens;
            set { itens = value; RaisePropertyChanged(nameof(Itens)); }
        }

        private ModeloModel? modelo;
        public ModeloModel? Modelo
        {
            get => modelo;
            set { modelo = value; RaisePropertyChanged(nameof(Modelo)); }
        }

        private ObservableCollection<QryModeloModel>? qrymodelos;
        public ObservableCollection<QryModeloModel>? QryModelos
        {
            get => qrymodelos;
            set { qrymodelos = value; RaisePropertyChanged(nameof(QryModelos)); }
        }

        public async Task<ObservableCollection<StatusChkGeralCentralModel>> GetItensAsync()
        {
            using DatabaseContext db = new();
            var data = await db.statusChkGeralCentrals
                .OrderBy(x => x.sigla)
                .ThenBy(x => x.tema)
                .ThenBy(x => x.ordem)
                .ToListAsync();
            return new ObservableCollection<StatusChkGeralCentralModel>(data);
        }

        public async Task<ModeloModel> AddModeloAsync(ModeloModel modelo, long? idtema)
        {
            using DatabaseContext db = new();
            var strategy = db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
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

        public async Task<QryModeloModel> GetModelo(long? id_modelo)
        {
            using DatabaseContext db = new();
            return await db.qryModelos.Where(m => m.id_modelo == id_modelo).FirstOrDefaultAsync();
        }
    }
}
