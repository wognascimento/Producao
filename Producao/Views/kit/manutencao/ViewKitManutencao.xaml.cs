using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Producao.Views.kit.solucao;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.kit.manutencao
{
    /// <summary>
    /// Interação lógica para ViewKitSolucao.xam
    /// </summary>
    public partial class ViewKitManutencao : UserControl
    {
        private bool inicializado;

        public ViewKitManutencao()
        {
            InitializeComponent();
            DataContext = new ViewKitManutencaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (inicializado)
                return;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ViewKitManutencaoViewModel vm = (ViewKitManutencaoViewModel)DataContext;
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                inicializado = true;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ViewKitManutencaoViewModel vm = (ViewKitManutencaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.OsKits = await Task.Run(async () => await vm.GetOsKitsAsync(vm?.Sigla?.num_os));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void RadGridView_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            ViewKitManutencaoViewModel vm = (ViewKitManutencaoViewModel)DataContext;

            e.NewObject = new OsKitSolucaoModel
            {
                data_emissao = DateTime.Now,
                data_solicitacao = DateTime.Now,
                t_os_mont = vm.Sigla?.num_os,
                tipo_manutencao = "0",
                shopping = vm.Sigla?.cliente
            };
        }

        private void RadGridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == GridViewEditOperationType.None || e.Row.Item is not OsKitSolucaoModel rowData)
                return;

            if (!rowData.t_os_mont.HasValue)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Erro ao selecionar sigla.", PropertyName = string.Empty });
            }
            else if (rowData.solicitante == null)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informa o Solicitante.", PropertyName = "solicitante" });
            }
            else if (rowData.atendente == null)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informa o Atendente.", PropertyName = "atendente" });
            }
            else if (rowData.noite_montagem == null)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informa a Noite de Montagem.", PropertyName = "noite_montagem" });
            }
        }

        private async void RadGridView_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            try
            {
                ViewKitManutencaoViewModel vm = (ViewKitManutencaoViewModel)DataContext;

                if (e.Row.Item is not OsKitSolucaoModel data)
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                /*
                if (this.osKit.View.IsAddingNew)
                {
                    await Task.Run(() => vm.AddOsKitsAsync(data));
                    //AddOsKitsAsync
                }
                else if (this.osKit.View.IsEditingItem)
                {
                    await Task.Run(() => vm.EditOsKitsAsync(data));
                    //EditOsKitsAsync
                }
                */

                await Task.Run(() => vm.AddOsKitsAsync(data));

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

       
    }

    public class ViewKitManutencaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ICommand rowDetalhesCommand { get; set; }
        public ICommand RowDetalhesCommand
        {
            get { return rowDetalhesCommand; }
            set { rowDetalhesCommand = value; }
        }

        private ObservableCollection<TblServicoModel> _siglas;
        public ObservableCollection<TblServicoModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }

        private TblServicoModel _sigla;
        public TblServicoModel Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged("Sigla"); }
        }

        private ObservableCollection<OsKitSolucaoModel> _osKits;
        public ObservableCollection<OsKitSolucaoModel> OsKits
        {
            get { return _osKits; }
            set { _osKits = value; RaisePropertyChanged("OsKits"); }
        }

        private OsKitSolucaoModel _osKit;
        public OsKitSolucaoModel OsKit
        {
            get { return _osKit; }
            set { _osKit = value; RaisePropertyChanged("OsKit"); }
        }

        public ViewKitManutencaoViewModel()
        {
            rowDetalhesCommand = new RelayCommand(DetalhesCanExecute);
        }

        public void DetalhesCanExecute(object obj)
        {
            var osKit = obj as OsKitSolucaoModel ?? OsKit;

            if (osKit == null)
            {
                MessageBox.Show("A linha não foi totalmente inserida.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OsKit = osKit;
            ((MainWindow)Application.Current.MainWindow).adicionarFilho(new ViewDetalhesKitManutencao(osKit), $"DETALHES KIT MANUTENÇÃO {osKit.os}", "DETALHES_KIT_MANUTENCAO");
        }

        public async Task<ObservableCollection<TblServicoModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.tblServicos.OrderBy(c => c.sigla).Where(c => c.tipo.Equals("KIT MANUTENÇÃO")).ToListAsync();
                return new ObservableCollection<TblServicoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OsKitSolucaoModel>> GetOsKitsAsync(long? os_mont)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.OsKitSolucaos.OrderBy(c => c.os).Where(c => c.t_os_mont == os_mont).ToListAsync();
                return new ObservableCollection<OsKitSolucaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddOsKitsAsync(OsKitSolucaoModel? osKit)
        {
            try
            {
                using DatabaseContext db = new();
                //var data = await db.OsKitSolucaos.OrderBy(c => c.os).Where(c => c.t_os_mont == os_mont).ToListAsync();
                //await db.OsKitSolucaos.AddAsync(osKit);
                await db.OsKitSolucaos.SingleMergeAsync(osKit);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task EditOsKitsAsync(OsKitSolucaoModel? osKit)
        {
            try
            {
                using DatabaseContext db = new();
                //var data = await db.OsKitSolucaos.OrderBy(c => c.os).Where(c => c.t_os_mont == os_mont).ToListAsync();
                var kit = await  db.OsKitSolucaos.FindAsync(osKit.os);
                if (kit != null)
                {
                    db.Entry(kit).CurrentValues.SetValues(osKit);
                    db.Update(osKit);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
