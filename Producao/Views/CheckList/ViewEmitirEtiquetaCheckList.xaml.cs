using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewEmitirEtiquetaCheckList.xam
    /// </summary>
    public partial class ViewEmitirEtiquetaCheckList : UserControl
    {
        public ViewEmitirEtiquetaCheckList()
        {
            this.DataContext = new EmitirEtiquetaViewModel();
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EmitirEtiquetaViewModel vm = (EmitirEtiquetaViewModel)DataContext;
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                vm.Itens = await Task.Run(async () => await vm.GetItensAsync(""));
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
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EmitirEtiquetaViewModel vm = (EmitirEtiquetaViewModel)DataContext;
                //vm.Itens = await Task.Run(async () => await vm.GetItensAsync(vm.Sigla.sigla_serv));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void dgCheckList_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            /*
            if (MessageBox.Show("Confirma a exclusão do item?", "Excluir", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    ExpedModel data = (ExpedModel)e.Items[0];
                    ExpedicaoProdutoViewModel vm = (ExpedicaoProdutoViewModel)DataContext;
                    await Task.Run((() => vm.DeleteExpedAsync(data)));
                    e.Cancel = false;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    int num2 = (int)MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
            else
                e.Cancel = true;
            */
        }
    }

    public class EmitirEtiquetaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        
        public DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        private ObservableCollection<SiglaChkListModel> _siglas;
        public ObservableCollection<SiglaChkListModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }
        private SiglaChkListModel _sigla;
        public SiglaChkListModel Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged("Sigla"); }
        }

        private ObservableCollection<EtiquetaCheckListModel> _itens;
        public ObservableCollection<EtiquetaCheckListModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }
        private EtiquetaCheckListModel _item;
        public EtiquetaCheckListModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }
        /*
        private ObservableCollection<EtiquetaProducaoModel> _etiquetas;
        public ObservableCollection<EtiquetaProducaoModel> Etiquetas
        {
            get { return _etiquetas; }
            set { _etiquetas = value; RaisePropertyChanged("Etiquetas"); }
        }
        private EtiquetaProducaoModel _etiqueta;
        public EtiquetaProducaoModel Etiqueta
        {
            get { return _etiqueta; }
            set { _etiqueta = value; RaisePropertyChanged("Etiqueta"); }
        }
        */

        public async Task<ObservableCollection<SiglaChkListModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Siglas.OrderBy(c => c.sigla_serv).ToListAsync();
                return new ObservableCollection<SiglaChkListModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<EtiquetaCheckListModel>> GetItensAsync(string? sigla_serv)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.EtiquetaCheckLists
                    .OrderBy(c => c.item_memorial)
                    .ToListAsync();
                return new ObservableCollection<EtiquetaCheckListModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<EtiquetaProducaoModel>> GetEtiquetasAsync(long? coddetalhescompl)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.EtiquetaProducaos.Where(e => e.coddetalhescompl == coddetalhescompl).OrderBy(c => c.codvol).ToListAsync();
                return new ObservableCollection<EtiquetaProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EtiquetaProducaoModel> AddEtiquetaAsync(EtiquetaProducaoModel etiqueta)
        {
            try
            {
                using DatabaseContext db = new();
                await db.EtiquetaProducaos.SingleMergeAsync(etiqueta);
                await db.SaveChangesAsync();
                return etiqueta;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /*
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        */

    }
}
