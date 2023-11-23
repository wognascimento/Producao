using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Producao.Views.CentralModelos.CentralStatusCheckListMenuCommands;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Interação lógica para ViewCentralStatusCheckList.xam
    /// </summary>
    public partial class ViewCentralStatusCheckList : UserControl
    {
        public ViewCentralStatusCheckList()
        {
            InitializeComponent();
            this.DataContext = new ViewCentralStatusCheckListViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralStatusCheckListViewModel vm = (ViewCentralStatusCheckListViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
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
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public static class CentralStatusCheckListMenuCommands
    {
        static BaseCommand? createModelo;
        public static BaseCommand CreateModelo
        {
            get
            {
                if (createModelo == null)
                    createModelo = new BaseCommand(OnCreateModeloClicked);
                return createModelo;
            }
        }

        private async static void OnCreateModeloClicked(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as StatusChkGeralCentralModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as StatusChkGeralCentralModel;

            ViewCentralStatusCheckListViewModel vm = (ViewCentralStatusCheckListViewModel)grid.DataContext;

            if (record?.status == "falta modelo")
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    var dados = new ModeloModel
                    {
                        codcompladicional = record.codcompladicional,
                        tema = record.tema,
                        cadastrado_por = Environment.UserName,
                        data_cadastro = DateTime.Now
                    };

                    vm.Modelo = await Task.Run(() => vm.AddModeloAsync(dados, record.idtema));

                    QryModeloModel modelo = await Task.Run(() => vm.GetModelo(vm.Modelo.id_modelo));
                    vm.QryModelos = new ObservableCollection<QryModeloModel>();
                    vm?.QryModelos.Add(modelo);
                    var window = new ModeloReceita(modelo);
                    window.Owner = App.Current.MainWindow;
                    window.ShowDialog();
        
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        public class ViewCentralStatusCheckListViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string propName)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }

            private StatusChkGeralCentralModel item;
            public StatusChkGeralCentralModel Item
            {
                get { return item; }
                set { item = value; RaisePropertyChanged("Item"); }
            }

            private ObservableCollection<StatusChkGeralCentralModel> itens;
            public ObservableCollection<StatusChkGeralCentralModel> Itens
            {
                get { return itens; }
                set { itens = value; RaisePropertyChanged("Itens"); }
            }

            private ModeloModel modelo;
            public ModeloModel Modelo
            {
                get { return modelo; }
                set { modelo = value; RaisePropertyChanged("Modelo"); }
            }

            private ObservableCollection<QryModeloModel>? qrymodelos;
            public ObservableCollection<QryModeloModel> QryModelos
            {
                get { return qrymodelos; }
                set { qrymodelos = value; RaisePropertyChanged("QryModelos"); }
            }

            public async Task<ObservableCollection<StatusChkGeralCentralModel>> GetItensAsync()
            {
                try
                {
                    using DatabaseContext db = new();
                    var data = await db.statusChkGeralCentrals
                        .OrderBy(x => x.sigla)
                        .ThenBy(x => x.tema)
                        .ThenBy(x => x.ordem)
                        .ToListAsync();
                    return new ObservableCollection<StatusChkGeralCentralModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ModeloModel> AddModeloAsync(ModeloModel modelo, long? idtema)
            {
                using DatabaseContext db = new();
                var strategy = db.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () => 
                {
                    var transaction = db.Database.BeginTransaction();
                    try
                    {
                        await db.Modelos.SingleMergeAsync(modelo);
                        await db.SaveChangesAsync();

                        /*var historico = await db.HistoricosModelo.Where(c => c.codcompladicional_modelo == modelo.codcompladicional && c.idtema == idtema).ToListAsync();
                        foreach (HistoricoModelo item in historico)
                        {
                            var receita = new ModeloReceitaModel
                            {
                                id_modelo = modelo.id_modelo,
                                codcompladicional = item.codcompladicional_receita,
                                qtd_modelo = item.media_qtd_modelo,
                                qtd_producao = item.media_qtd_producao,
                                cadastrado_por = Environment.UserName,
                                data_cadastro = DateTime.Now,
                            };
                            await db.ReceitaModelos.SingleMergeAsync(receita);
                            await db.SaveChangesAsync();
                        }*/
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                });
                return modelo;
            }

            public async Task<QryModeloModel> GetModelo(long? id_modelo)
            {
                try
                {
                    using DatabaseContext db = new();
                    var data = await db.qryModelos.Where(m => m.id_modelo == id_modelo).FirstOrDefaultAsync();
                    return data;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

    }
}
