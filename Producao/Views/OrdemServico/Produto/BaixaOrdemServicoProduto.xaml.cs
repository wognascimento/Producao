using Microsoft.EntityFrameworkCore;
using Producao.Views.CentralModelos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para BaixaOrdemServicoProduto.xam
    /// </summary>
    public partial class BaixaOrdemServicoProduto : UserControl
    {
        public BaixaOrdemServicoProduto()
        {
            InitializeComponent();
            this.DataContext = new BaixaOrdemServicoProdutoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                BaixaOrdemServicoProdutoViewModel vm = (BaixaOrdemServicoProdutoViewModel)DataContext;
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
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
                BaixaOrdemServicoProdutoViewModel vm = (BaixaOrdemServicoProdutoViewModel)DataContext;
                BaixaOsProducaoModel data = (BaixaOsProducaoModel)e.Row.Item;
                await vm.BaixaAsync(data);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class BaixaOrdemServicoProdutoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BaixaOsProducaoModel>? _itens;
        public ObservableCollection<BaixaOsProducaoModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private BaixaOsProducaoModel? _item;
        public BaixaOsProducaoModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }

        public async Task<ObservableCollection<BaixaOsProducaoModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.BaixaOsProducoes
                    .ToListAsync();
                return new ObservableCollection<BaixaOsProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task BaixaAsync(BaixaOsProducaoModel baixa)
        {
            try
            {
                using DatabaseContext db = new();
                var os = await db.ProdutoServicos.FindAsync(baixa.num_os_servico);
                os.recebido_setor_data = baixa.recebido_setor_data;
                os.concluida_os_data = baixa.concluida_os_data;
                //await db.ProdutoServicos.SingleMergeAsync(os);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CancelarAsync(BaixaOsProducaoModel baixa)
        {
            try
            {
                using DatabaseContext db = new();
                var os = await db.ProdutoServicos.FindAsync(baixa.num_os_servico);
                os.cancelada_os = baixa.cancelada_os;
                //os.concluida_os_data = baixa.concluida_os_data;
                await db.ProdutoServicos.SingleMergeAsync(os);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }

    public static class ContextMenuCommandsBaixaOrdemServicoProduto
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

            var item = grid.SelectedItem as BaixaOsProducaoModel;
            if (item is null)
            {
                return;
            }

            BaixaOrdemServicoProdutoViewModel vm = (BaixaOrdemServicoProdutoViewModel)grid.DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                item.cancelada_os = "-1";
                await vm.CancelarAsync(item);
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
