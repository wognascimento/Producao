using Microsoft.EntityFrameworkCore;
using Producao.Views.kit.solucao;
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

namespace Producao.Views.kit.desmontagem
{
    /// <summary>
    /// Interação lógica para ViewKitDesmontagem.xam
    /// </summary>
    public partial class ViewKitDesmontagem : UserControl
    {
        public ViewKitDesmontagem()
        {
            InitializeComponent();
            DataContext = new ViewKitDesmontagemViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ViewKitDesmontagemViewModel vm = (ViewKitDesmontagemViewModel)DataContext;
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewKitDesmontagemViewModel vm = (ViewKitDesmontagemViewModel)DataContext;
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

        private void SfDataGrid_AddNewRowInitiating(object sender, AddNewRowInitiatingEventArgs e)
        {
            ViewKitDesmontagemViewModel vm = (ViewKitDesmontagemViewModel)DataContext;

            ((OsKitSolucaoModel)e.NewObject).data_emissao = DateTime.Now;
            ((OsKitSolucaoModel)e.NewObject).data_solicitacao = DateTime.Now;
            ((OsKitSolucaoModel)e.NewObject).t_os_mont = vm.Sigla.num_os;
            ((OsKitSolucaoModel)e.NewObject).tipo_manutencao = "0";
            ((OsKitSolucaoModel)e.NewObject).shopping = vm.Sigla.cliente;
        }

        private void SfDataGrid_RowValidating(object sender, RowValidatingEventArgs e)
        {
            OsKitSolucaoModel rowData = (OsKitSolucaoModel)e.RowData;
            if (!rowData.t_os_mont.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("data_emissao", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("solicitante", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("concluir_ate", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("atendente", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("data_solicitacao", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("noite_montagem", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("obs_de_envio", "Erro ao selecionar sigla.");
            }
            else if (rowData.solicitante == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("solicitante", "Informa o Solicitante.");
            }
            else if (rowData.atendente == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("atendente", "Informa o Atendente.");
            }
            else if (rowData.noite_montagem == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("noite_montagem", "Informa a Noite de Montagem.");
            }
        }

        private async void SfDataGrid_RowValidated(object sender, RowValidatedEventArgs e)
        {
            try
            {
                var sfdatagrid = sender as SfDataGrid;
                ViewKitDesmontagemViewModel vm = (ViewKitDesmontagemViewModel)DataContext;

                OsKitSolucaoModel data = (OsKitSolucaoModel)e.RowData;

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

    public class ViewKitDesmontagemViewModel : INotifyPropertyChanged
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

        public ViewKitDesmontagemViewModel()
        {
            rowDetalhesCommand = new RelayCommand(DetalhesCanExecute);
        }

        public async void DetalhesCanExecute(object obj)
        {
            //GRIobj = { Syncfusion.UI.Xaml.Grid.SfDataGrid}
            var sfdatagrid = obj as SfDataGrid;

            ((MainWindow)Application.Current.MainWindow).adicionarFilho(new ViewDetalhesKitDesmontagem(OsKit), $"DETALHES KIT DESMONTAGEM {OsKit.os}", "DETALHES_KIT_DESMONTAGEM");
        }

        public async Task<ObservableCollection<TblServicoModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.tblServicos.OrderBy(c => c.sigla).Where(c => c.tipo.Equals("KIT DESMONTAGEM")).ToListAsync();
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
                var kit = await db.OsKitSolucaos.FindAsync(osKit.os);
                if (kit != null)
                {
                    db.Entry(kit).CurrentValues.SetValues(osKit);
                    db.Update(osKit);
                }
                db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
