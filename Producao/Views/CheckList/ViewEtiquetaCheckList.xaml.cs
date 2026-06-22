using Microsoft.EntityFrameworkCore;
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

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewEtiquetaCheckList.xam
    /// </summary>
    public partial class ViewEtiquetaCheckList : UserControl
    {
        private int etiqueta = 1;
        public ViewEtiquetaCheckList()
        {
            InitializeComponent();
            this.DataContext = new EtiquetaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
                //vm.Siglas =  await Task.Run(vm.GetSiglasAsync);
                vm.Dados = await vm.GetItensAsync();
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
                //vm.Dados = await Task.Run(() => vm.GetItensAsync(vm.Sigla.sigla_serv));
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
                vm.Etiquetas = await vm.GetEtiquetasAsync(vm.Dado.coddetalhescompl);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
        }

        private async void dgEtiqueta_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
                vm.Etiqueta = (EtiquetaProducaoModel)e.Row.Item;
                EtiquetaProducaoModel data = (EtiquetaProducaoModel)e.Row.Item;

                if (data.codvol is null)
                {
                    for (int i = 0; i < data.volumes_total; i++)
                    {
                        vm.Etiqueta.volumes = i + 1;
                        if (i > 0)
                        {
                            vm.Etiqueta.codvol = null;
                            vm.Etiqueta.qtd = 0;
                            //vm.Etiqueta.criado_por = Environment.UserName;
                            //vm.Etiqueta.criado_em = DateTime.Now;
                        }
                        vm.Etiqueta = await vm.AddEtiquetaAsync(vm.Etiqueta);
                    }
                    vm.Etiquetas = await vm.GetEtiquetasAsync(data.coddetalhescompl);
                }
                else 
                {
                    vm.Etiqueta = await vm.AddEtiquetaAsync(vm.Etiqueta);
                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void dgEtiqueta_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
            EtiquetaProducaoModel rowData = (EtiquetaProducaoModel)e.Row.Item;
            if (!rowData.coddetalhescompl.HasValue)
            {
                e.IsValid = false;
                AddValidation(e, "codvol", "Erro ao selecionar a linha.");
                AddValidation(e, "volumes", "Erro ao selecionar a linha.");
                AddValidation(e, "volumes_total", "Erro ao selecionar a linha.");
                AddValidation(e, "qtd", "Erro ao selecionar a linha.");
                AddValidation(e, "largura", "Erro ao selecionar a linha.");
                AddValidation(e, "altura", "Erro ao selecionar a linha.");
                AddValidation(e, "profundidade", "Erro ao selecionar a linha.");
                AddValidation(e, "peso_bruto", "Erro ao selecionar a linha.");
                AddValidation(e, "peso_liquido", "Erro ao selecionar a linha.");
                AddValidation(e, "impresso", "Erro ao selecionar a linha.");
            }
            /*else if(rowData.codvol.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("codvol", "Este campo precisar está em branco.");
            }*/
            else if (!rowData.volumes.HasValue)
            {
                e.IsValid = false;
                AddValidation(e, "volumes", "Informe o volume.");
            }
            else if (rowData.volumes == 0)
            {
                e.IsValid = false;
                AddValidation(e, "volumes", "Volume não pode ser Zero(0).");
            }
            else if (!rowData.volumes_total.HasValue)
            {
                e.IsValid = false;
                AddValidation(e, "volumes_total", "Informe o total de volume.");
            }
            else if (rowData.volumes_total < rowData.volumes)
            {
                e.IsValid = false;
                AddValidation(e, "volumes_total", "Total de volumes não pode ser menor que volume");
            }
            else if (!rowData.qtd.HasValue)
            {
                e.IsValid = false;
                AddValidation(e, "qtd", "Informe a quantidade em cada volume.");
            }
            else if (rowData.qtd > vm.Dado.qtd_nao_expedida)
            {
                e.IsValid = false;
                AddValidation(e, "qtd", "a quantidade da etiqueta não pode ser maior que a do checklist");
            }
            /*if ((e.RowData as EtiquetaProducaoModel).volumes == null)
            {
                e.IsValid = false;

            }*/
        }

        private void dgEtiqueta_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
            e.NewObject = new EtiquetaProducaoModel
            {
                coddetalhescompl = vm.Dado.coddetalhescompl,
                criado_por = Environment.UserName,
                criado_em = DateTime.Now
            };

            /*
             * vm.Etiqueta.criado_por = Environment.UserName;
             * vm.Etiqueta.criado_em = DateTime.Now;
            */
        }

        private async void dgEtiqueta_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            
            if (MessageBox.Show("Confirma a exclusão a etiqueta?", "Excluir", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    EtiquetaProducaoModel data = (EtiquetaProducaoModel)e.Items.First();
                    EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
                    await vm.DeleteEtiquetaAsync(data);
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
            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private async void OnImprimirEtiquetaCheckListClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { DataContext: EtiquetaCheckListModel item })
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
                var etiquetasEmitidas = await vm.GetEtiquetasEmitidasAsync(item.coddetalhescompl);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                ViewEtiquetaCheckListEmitida.ImprimirEtiquetas(etiquetasEmitidas, vm.BaseSettings);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
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
    }

    public class EtiquetaViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
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
        /*
        private ObservableCollection<EtiquetaCheckListModel> _itens;
        public ObservableCollection<EtiquetaCheckListModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }
        */
        private ObservableCollection<EtiquetaCheckListModel> _dados;
        public ObservableCollection<EtiquetaCheckListModel> Dados
        {
            get { return _dados; }
            set
            {
                _dados = value;
                RaisePropertyChanged("Dados");
            }
        }

        private EtiquetaCheckListModel _dado;
        public EtiquetaCheckListModel Dado
        {
            get { return _dado; }
            set { _dado = value; RaisePropertyChanged("Dado"); }
        }

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

        public EtiquetaViewModel()
        {
           
        }

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

        public async Task<ObservableCollection<EtiquetaCheckListModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.EtiquetaCheckLists
                    .Where(e => e.qtd_detalhe > 0 && e.qtd_nao_expedida > 0)
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
                var data = await db.EtiquetaProducaos.Where(e => e.coddetalhescompl == coddetalhescompl ).OrderBy(c => c.codvol).ToListAsync();
                return new ObservableCollection<EtiquetaProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<EtiquetaEmitidaModel>> GetEtiquetasEmitidasAsync(long? coddetalhescompl)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.EtiquetaEmitidas
                    .Where(e => e.coddetalhescompl == coddetalhescompl)
                    .OrderBy(e => e.codvol)
                    .ToListAsync();
                return new ObservableCollection<EtiquetaEmitidaModel>(data);
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
                /*db.Entry(Etiqueta).State = Etiqueta.codvol == null ?
                                   EntityState.Added :
                                   EntityState.Modified;*/
                await db.EtiquetaProducaos.SingleMergeAsync(etiqueta);
                await db.SaveChangesAsync();

                return etiqueta;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteEtiquetaAsync(EtiquetaProducaoModel etiqueta)
        {
            try
            {
                using DatabaseContext db = new();
                db.Entry(etiqueta).State = EntityState.Deleted;
                int num = await db.SaveChangesAsync();
                db.Entry(etiqueta).State = EntityState.Detached;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
