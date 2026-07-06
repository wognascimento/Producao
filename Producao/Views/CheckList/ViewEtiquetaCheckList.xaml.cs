using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
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
        private bool _dadosCarregados;

        public ViewEtiquetaCheckList()
        {
            InitializeComponent();
            this.DataContext = new EtiquetaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_dadosCarregados)
                return;

            try
            {
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EtiquetaViewModel vm = (EtiquetaViewModel)DataContext;
                //vm.Siglas =  await vm.GetSiglasAsync();
                vm.Dados = await vm.GetItensAsync();
                _dadosCarregados = true;
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
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
                //vm.Dados = await vm.GetItensAsync(vm.Sigla.sigla_serv);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
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
                Producao.ErrorDialog.Show(ex, "Erro");
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
                Producao.ErrorDialog.Show(ex, "Erro");
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
                    Producao.ErrorDialog.Show(ex, "Erro");
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
                Producao.ErrorDialog.Show(ex, "Erro");
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
        static EtiquetaViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

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
                const string sql = """
                    SELECT *
                    FROM producao.view_sigla_chkgeral
                    ORDER BY sigla_serv;
                    """;

                var data = await QueryAsync<SiglaChkListModel>(sql);
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
                const string sql = """
                    SELECT *
                    FROM producao.qryetiquetachkgeral
                    WHERE qtd_detalhe > 0
                      AND qtd_nao_expedida > 0
                    ORDER BY item_memorial;
                    """;

                var data = await QueryAsync<EtiquetaCheckListModel>(sql);
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
                const string sql = """
                    SELECT *
                    FROM producao.tbl_etiqueta_producao
                    WHERE coddetalhescompl = @coddetalhescompl
                    ORDER BY codvol;
                    """;

                var data = await QueryAsync<EtiquetaProducaoModel>(sql, new { coddetalhescompl });
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
                const string sql = """
                    SELECT *
                    FROM producao.etiqueta_emitida
                    WHERE coddetalhescompl = @coddetalhescompl
                    ORDER BY codvol;
                    """;

                var data = await QueryAsync<EtiquetaEmitidaModel>(sql, new { coddetalhescompl });
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
                await using var conn = CreateConnection();

                if (etiqueta.codvol is null or 0)
                {
                    const string insertSql = """
                        INSERT INTO producao.tbl_etiqueta_producao
                            (coddetalhescompl, volumes, volumes_total, qtd, largura, altura, profundidade,
                             peso_bruto, peso_liquido, impresso, impresso_por, impresso_em, criado_por, criado_em)
                        VALUES
                            (@coddetalhescompl, @volumes, @volumes_total, @qtd, @largura, @altura, @profundidade,
                             @peso_bruto, @peso_liquido, @impresso, @impresso_por, @impresso_em, @criado_por, @criado_em)
                        RETURNING codvol;
                        """;

                    etiqueta.codvol = await conn.ExecuteScalarAsync<long>(insertSql, etiqueta);
                }
                else
                {
                    const string updateSql = """
                        UPDATE producao.tbl_etiqueta_producao
                        SET coddetalhescompl = @coddetalhescompl,
                            volumes = @volumes,
                            volumes_total = @volumes_total,
                            qtd = @qtd,
                            largura = @largura,
                            altura = @altura,
                            profundidade = @profundidade,
                            peso_bruto = @peso_bruto,
                            peso_liquido = @peso_liquido,
                            impresso = @impresso,
                            impresso_por = @impresso_por,
                            impresso_em = @impresso_em,
                            criado_por = @criado_por,
                            criado_em = @criado_em
                        WHERE codvol = @codvol;
                        """;

                    await conn.ExecuteAsync(updateSql, etiqueta);
                }

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
                await using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    "DELETE FROM producao.tbl_etiqueta_producao WHERE codvol = @codvol;",
                    new { etiqueta.codvol });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

