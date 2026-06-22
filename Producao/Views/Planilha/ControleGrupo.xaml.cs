using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Planilha
{
    /// <summary>
    /// Interação lógica para ControleGrupo.xam
    /// </summary>
    public partial class ControleGrupo : UserControl
    {
        public ControleGrupo()
        {
            InitializeComponent();
            this.DataContext = new ControleGrupoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControleGrupoViewModel vm = (ControleGrupoViewModel)DataContext;
                vm.ControlePlanilhaGrupos = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell?.Column?.UniqueName == "os" &&
                e.Cell.DataContext is ControlePlanilhaGrupoModel record)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    ControleGrupoViewModel vm = (ControleGrupoViewModel)DataContext;
                    await vm.SaveAsync(record);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControleGrupoViewModel vm = (ControleGrupoViewModel)DataContext;
                if (e.EditedItem is not ControlePlanilhaGrupoModel data)
                {
                    return;
                }

                data = await vm.SaveAsync(data);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class ControleGrupoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControlePlanilhaGrupoModel>? _controlePlanilhaGrupos;
        public ObservableCollection<ControlePlanilhaGrupoModel> ControlePlanilhaGrupos
        {
            get { return _controlePlanilhaGrupos; }
            set { _controlePlanilhaGrupos = value; RaisePropertyChanged("ControlePlanilhaGrupos"); }
        }

        private ControlePlanilhaGrupoModel? _controlePlanilhaGrupo;
        public ControlePlanilhaGrupoModel ControlePlanilhaGrupo
        {
            get { return _controlePlanilhaGrupo; }
            set { _controlePlanilhaGrupo = value; RaisePropertyChanged("ControlePlanilhaGrupo"); }
        }

        private ObservableCollection<string>? _statusProducao = ["ACABAMENTO", "ARAMADO", "COMPRAS", "ELÉTRICA", "EMBALAGEM", "ETIQUETAGEM/EXPEDIÇÃO", "FIBRA", "FILA", "MARCENARIA", "PINTURA", "PROJETOS", "REVESTIMENTO", "SEPARAÇÃO", "SERRALHERIA", "TERCEIRIZADO MARCENARIA", "TERCEIRIZADO SERRALHERIA"];
        public ObservableCollection<string> StatusProducao
        {
            get { return _statusProducao; }
            set { _statusProducao = value; RaisePropertyChanged("StatusProducao"); }
        }

        public async Task<ObservableCollection<ControlePlanilhaGrupoModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControlePlanilhaGrupos
                    .ToListAsync();
                return new ObservableCollection<ControlePlanilhaGrupoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControlePlanilhaGrupoModel> SaveAsync(ControlePlanilhaGrupoModel controle)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ControlePlanilhaGrupos.SingleUpdateAsync(controle);
                await db.SaveChangesAsync();
                return controle;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
