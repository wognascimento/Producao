using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.Planilha
{
    /// <summary>
    /// Interação lógica para ControleGrupo.xam
    /// </summary>
    public partial class ControleGrupo : UserControl
    {
        private ICollectionView? _gridView;
        private bool _atualizandoSequencia;

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
                vm.StatusProducao = await vm.GetStatusProducaoAsync();
                vm.ControlePlanilhaGrupos = await vm.GetItensAsync();
                VincularAtualizacaoSequencia();
                AtualizarSequencia();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell?.Column?.UniqueName == "os")
                return;
        }

        private async void OnOsCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not ControlePlanilhaGrupoModel record)
                return;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                adicionais.SelectedItem = record;
                ControleGrupoViewModel vm = (ControleGrupoViewModel)DataContext;
                await vm.SaveOsAsync(record);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
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
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DesvincularAtualizacaoSequencia();
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private void VincularAtualizacaoSequencia()
        {
            DesvincularAtualizacaoSequencia();
            _gridView = CollectionViewSource.GetDefaultView(adicionais.ItemsSource);

            if (_gridView is INotifyCollectionChanged notifyCollectionChanged)
                notifyCollectionChanged.CollectionChanged += OnGridViewCollectionChanged;
        }

        private void DesvincularAtualizacaoSequencia()
        {
            if (_gridView is INotifyCollectionChanged notifyCollectionChanged)
                notifyCollectionChanged.CollectionChanged -= OnGridViewCollectionChanged;

            _gridView = null;
        }

        private void OnGridViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AgendarAtualizacaoSequencia();
        }

        private void OnGridViewViewChanged(object sender, EventArgs e)
        {
            AgendarAtualizacaoSequencia();
        }

        private void AgendarAtualizacaoSequencia()
        {
            Dispatcher.BeginInvoke(new Action(AtualizarSequencia));
        }

        private void AtualizarSequencia()
        {
            if (_atualizandoSequencia || adicionais.ItemsSource == null)
                return;

            try
            {
                _atualizandoSequencia = true;
                var sequencia = 1;

                foreach (var grupo in adicionais.Items.OfType<ControlePlanilhaGrupoModel>())
                {
                    grupo.sequencia = sequencia++;
                }
            }
            finally
            {
                _atualizandoSequencia = false;
            }
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

        private ObservableCollection<string>? _statusProducao = [];
        public ObservableCollection<string> StatusProducao
        {
            get { return _statusProducao; }
            set { _statusProducao = value; RaisePropertyChanged("StatusProducao"); }
        }

        public async Task<ObservableCollection<string>> GetStatusProducaoAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<string>(
                    @"SELECT status_producao
                      FROM producao.tbl_status_producao
                      ORDER BY status_producao;");
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ControlePlanilhaGrupoModel>> GetItensAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<ControlePlanilhaGrupoModel>(
                    @"SELECT *
                      FROM producao.view_controle_planilha_grupo;");
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
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                await conn.ExecuteAsync(
                    @"UPDATE producao.tbldetalhescomplemento
                      SET obs_planilheiro = @obs_planilheiro,
                          resp_prod = @resp_prod,
                          status_producao = @status_producao,
                          os = @os,
                          enviado_baia = @enviado_baia
                      WHERE coddetalhescompl = @coddetalhescompl;",
                    controle);
                return controle;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveOsAsync(ControlePlanilhaGrupoModel controle)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                await conn.ExecuteAsync(
                    @"UPDATE producao.tbldetalhescomplemento
                      SET os = @os
                      WHERE coddetalhescompl = @coddetalhescompl;",
                    controle);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
