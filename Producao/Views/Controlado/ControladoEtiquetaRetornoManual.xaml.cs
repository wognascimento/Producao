using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.DataBase.Model;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.Grid.ScrollAxis;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Persistence.Core;

namespace Producao.Views.Controlado
{
    /// <summary>
    /// Interação lógica para ControladoEtiquetaRetornoManual.xam
    /// </summary>
    public partial class ControladoEtiquetaRetornoManual : UserControl
    {
        public ControladoEtiquetaRetornoManual()
        {
            InitializeComponent();
            DataContext = new ControladoEtiquetaRetornoManualViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControladoEtiquetaRetornoManualViewModel vm = (ControladoEtiquetaRetornoManualViewModel)DataContext;
                vm.Retornos = await Task.Run(vm.GetRetornoItensAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {

        }

        private async void OnRowValidating(object sender, RowValidatingEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            ControladoEtiquetaRetornoManualViewModel vm = (ControladoEtiquetaRetornoManualViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                QryControladoEtiquetaRetornoModel data = (QryControladoEtiquetaRetornoModel)e.RowData;
                var saida = await Task.Run(() => vm.GetSaidaAsync(data.codigo));
                if (saida == null)
                {
                    MessageBox.Show("Por favor, verifique se a etiqueta foi marcada como 'saída' ou se já retornou.", "Etiqueta não encontrada");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                    var toRemove = vm.Retornos.Where(x => x.codigo == ((QryControladoEtiquetaRetornoModel)e.RowData).codigo).ToList();
                    foreach (var item in toRemove)
                        vm.Retornos.Remove(item);
                }
                else
                {
                    vm.RetornoBaixa = new()
                    {
                        barcode = saida.barcode,
                        inserido_por = Environment.UserName,
                        inserido_em = DateTime.Now
                    };

                    vm.RetornoBaixa = await Task.Run(() => vm.AddRetornoEtiquetaAsync(vm.RetornoBaixa));

                    ((QryControladoEtiquetaRetornoModel)e.RowData).planilha = saida.planilha;
                    ((QryControladoEtiquetaRetornoModel)e.RowData).descricao_completa = saida.descricao_completa;
                    sfdatagrid.View.Refresh();
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.Retornos.Where(x => x.codigo == ((QryControladoEtiquetaRetornoModel)e.RowData).codigo).ToList();
                foreach (var item in toRemove)
                    vm.Retornos.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void retornos_CurrentCellValidating(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValidatingEventArgs e)
        {
            /*
            try
            {

                ControladoEtiquetaRetornoManualViewModel vm = (ControladoEtiquetaRetornoManualViewModel)DataContext;
                QryControladoEtiquetaRetornoModel record = (QryControladoEtiquetaRetornoModel)e.RowData;

                if (e.Column.MappingName == "codigo")
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    var saida = await Task.Run(() => vm.GetSaidaAsync(long.Parse(Convert.ToString(e.NewValue))));
                    if (saida == null)
                    {
                        MessageBox.Show("Por favor, verifique se a etiqueta foi marcada como 'saída' ou se já retornou.");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        this.retornos.GetAddNewRowController().CancelAddNew();
                        //this.retornos.MoveCurrentCell(new RowColumnIndex(RowIndex, ColumnIndex));
                        return;
                    }
                    record.planilha = saida.planilha;
                    record.descricao_completa = saida.descricao_completa;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
            */
        }
    }

    class ControladoEtiquetaRetornoManualViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<QryControladoEtiquetaRetornoModel> _retornos;
        public ObservableCollection<QryControladoEtiquetaRetornoModel> Retornos
        {
            get { return _retornos; }
            set { _retornos = value; RaisePropertyChanged("Retornos"); }
        }

        private QryControladoEtiquetaRetornoModel _retorno;
        public QryControladoEtiquetaRetornoModel Retorno
        {
            get { return _retorno; }
            set { _retorno = value; RaisePropertyChanged("Retorno"); }
        }

        private QryGeralRequisicaoModel _saida;
        public QryGeralRequisicaoModel Saida
        {
            get { return _saida; }
            set { _saida = value; RaisePropertyChanged("Saida"); }
        }

        private ControladoShoppingRetornoModel _retornoBaixa;
        public ControladoShoppingRetornoModel RetornoBaixa
        {
            get { return _retornoBaixa; }
            set { _retornoBaixa = value; RaisePropertyChanged("RetornoBaixa"); }
        }

        public async Task<ObservableCollection<QryControladoEtiquetaRetornoModel>> GetRetornoItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.qryControladoEtiquetaRetornos
                    .ToListAsync();
                return new ObservableCollection<QryControladoEtiquetaRetornoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QryGeralRequisicaoModel> GetSaidaAsync(long? codigo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.qryGeralRequisicaos.Where(s => s.retorno == null && s.codigo == codigo).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControladoShoppingRetornoModel> AddRetornoEtiquetaAsync(ControladoShoppingRetornoModel controlado)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ControladoShoppingRetornos.SingleMergeAsync(controlado);
                await db.SaveChangesAsync();
                return controlado;
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }
    }
}
