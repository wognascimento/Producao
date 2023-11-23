using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Producao.Views.kit.solucao;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Producao.Views.kit
{
    /// <summary>
    /// Interação lógica para ViewControleGeralSolicitacao.xam
    /// </summary>
    public partial class ViewControleGeralSolicitacao : UserControl
    {
        public ViewControleGeralSolicitacao()
        {
            InitializeComponent();
            DataContext = new ControleGeralSolicitacaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControleGeralSolicitacaoViewModel vm = (ControleGeralSolicitacaoViewModel)DataContext;
                vm.Controles = await Task.Run(vm.GetControlesAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {

        }

        private async void OnRowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            try
            {
                var sfdatagrid = sender as SfDataGrid;
                ControleGeralSolicitacaoViewModel vm = (ControleGeralSolicitacaoViewModel)DataContext;

                ControleSolicaoGeralModel data = (ControleSolicaoGeralModel)e.RowData;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });


                await Task.Run(() => vm.AddControleAsync(
                    new ControleEnvioModel 
                    {
                        coddetalhescompl = data.coddetalhescompl,
                        data_envio = data.data_envio,
                        local_galpao = "JACAREÍ",
                        status = data.status,
                        placa = data.placa,
                        motorista = data.motorista,
                        horario_saida = data.horario_saida,
                        ordem = data.ordem,
                    }));

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class ControleGeralSolicitacaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControleSolicaoGeralModel> _controles;
        public ObservableCollection<ControleSolicaoGeralModel> Controles
        {
            get { return _controles; }
            set { _controles = value; RaisePropertyChanged("Controles"); }
        }
        private ControleSolicaoGeralModel _controle;
        public ControleSolicaoGeralModel Controle
        {
            get { return _controle; }
            set { _controle = value; RaisePropertyChanged("Controle"); }
        }

        public async Task<ObservableCollection<ControleSolicaoGeralModel>> GetControlesAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControleSolicaoGeral.ToListAsync();
                return new ObservableCollection<ControleSolicaoGeralModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddControleAsync(ControleEnvioModel? controle)
        {
            try
            {
                using DatabaseContext db = new();
                //var data = await db.OsKitSolucaos.OrderBy(c => c.os).Where(c => c.t_os_mont == os_mont).ToListAsync();
                //await db.OsKitSolucaos.AddAsync(osKit);
                await db.ControleEnvio.SingleMergeAsync(controle);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
