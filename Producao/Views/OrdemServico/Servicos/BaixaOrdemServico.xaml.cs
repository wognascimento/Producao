using Microsoft.EntityFrameworkCore;
using Producao.Views.OrdemServico.Produto;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.OrdemServico.Servicos
{
    /// <summary>
    /// Interação lógica para BaixaOrdemServico.xam
    /// </summary>
    public partial class BaixaOrdemServico : UserControl
    {
        public BaixaOrdemServico()
        {
            InitializeComponent();
            this.DataContext = new BaixaOrdemServicoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                BaixaOrdemServicoViewModel vm = (BaixaOrdemServicoViewModel)DataContext;
                vm.Itens = await Task.Run(vm.GetItensAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }


        private async void SfDataGrid_RowValidated(object sender, RowValidatedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                BaixaOrdemServicoViewModel vm = (BaixaOrdemServicoViewModel)DataContext;
                TblServicoModel data = (TblServicoModel)e.RowData;
                await Task.Run(() => vm.BaixaAsync(data));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

    }

    public class BaixaOrdemServicoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<TblServicoModel>? _itens;
        public ObservableCollection<TblServicoModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private TblServicoModel? _item;
        public TblServicoModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }

        public async Task<ObservableCollection<TblServicoModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.tblServicos
                    .ToListAsync();
                return new ObservableCollection<TblServicoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task BaixaAsync(TblServicoModel baixa)
        {
            try
            {
                using DatabaseContext db = new();
                var os = await db.tblServicos.FindAsync(baixa.num_os);
                os.data_conclusao = baixa.data_conclusao;
                os.data_conclusao_efetiva = baixa.data_conclusao_efetiva;
                //await db.ProdutoServicos.SingleMergeAsync(os);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CancelarAsync(TblServicoModel baixa)
        {
            try
            {
                using DatabaseContext db = new();
                var os = await db.tblServicos.FindAsync(baixa.num_os);
                os.cancelar = baixa.cancelar;
                //os.concluida_os_data = baixa.concluida_os_data;
                await db.tblServicos.SingleMergeAsync(os);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static class ContextMenuCommandsBaixaOrdemServico
    {
        static BaseCommand? cancelarOS;
        public static BaseCommand CancelarOS
        {
            get
            {
                cancelarOS ??= new BaseCommand(OnCancelarOSClicked);
                return cancelarOS;
            }
        }

        private static async void OnCancelarOSClicked(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as TblServicoModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as TblServicoModel;
            BaixaOrdemServicoViewModel vm = (BaixaOrdemServicoViewModel)grid.DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                item.cancelar = "-1";
                await Task.Run(() => vm.CancelarAsync(item));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }
    }
}
