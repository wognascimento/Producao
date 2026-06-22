using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Producao.Views.OrdemServico.Produto;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Desbaiamento
{
    /// <summary>
    /// Interação lógica para EmitirOSDesbaiamento.xam
    /// </summary>
    public partial class EmitirOSDesbaiamento : UserControl
    {
        public EmitirOSDesbaiamento()
        {
            InitializeComponent();
            this.DataContext = new EmitirOSDesbaiamentoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EmitirOSDesbaiamentoViewModel vm = (EmitirOSDesbaiamentoViewModel)DataContext;
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

            var grid = sender as RadGridView;
            EmitirOSDesbaiamentoViewModel vm = (EmitirOSDesbaiamentoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                OsExpModel data = (OsExpModel)e.Row.Item;
                data.inserido_por = Environment.UserName;
                data.inserido_em = DateTime.Now;
                vm.Item = await vm.AddOsAsync(data);

                ((OsExpModel)e.Row.Item).n_os_desbaiamento = vm.Item.n_os_desbaiamento;
                grid?.Items.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.Itens.Where(x => x.n_os_desbaiamento == null).ToList();
                foreach (var item in toRemove)
                    vm.Itens.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

    }

    public class EmitirOSDesbaiamentoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<OsExpModel>? _itens;
        public ObservableCollection<OsExpModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        private OsExpModel? _item;
        public OsExpModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }

        public async Task<ObservableCollection<OsExpModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.OsExps
                    .ToListAsync();
                return new ObservableCollection<OsExpModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OsExpModel> AddOsAsync(OsExpModel osExp)
        {
            try
            {
                using DatabaseContext db = new();
                await db.OsExps.SingleMergeAsync(osExp);
                await db.SaveChangesAsync();
                return osExp;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
