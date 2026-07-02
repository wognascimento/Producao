using Producao.Views.OrdemServico.Produto;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

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
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }


        private async void RadGridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction != GridViewEditAction.Commit)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                BaixaOrdemServicoViewModel vm = (BaixaOrdemServicoViewModel)DataContext;
                TblServicoModel data = (TblServicoModel)e.Row.Item;
                await vm.BaixaAsync(data);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
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
                var data = await ServicoOrdemRepository.GetServicosAsync();
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
                await ServicoOrdemRepository.BaixarAsync(baixa);
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
                baixa.cancelado_por ??= Environment.UserName;
                baixa.data_cancelamento ??= DateTime.Now;
                await ServicoOrdemRepository.CancelarAsync(baixa);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static class ContextMenuCommandsBaixaOrdemServico
    {
        static ICommand? cancelarOS;
        public static ICommand CancelarOS
        {
            get
            {
                cancelarOS ??= new RelayCommand(OnCancelarOSClicked);
                return cancelarOS;
            }
        }

        private static async void OnCancelarOSClicked(object obj)
        {
            var grid = obj as RadGridView;
            if (grid is null)
            {
                return;
            }

            var item = grid.SelectedItem as TblServicoModel;
            if (item is null)
            {
                return;
            }

            BaixaOrdemServicoViewModel vm = (BaixaOrdemServicoViewModel)grid.DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                item.cancelar = "-1";
                await vm.CancelarAsync(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }
}

