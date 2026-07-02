using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CentralModelos
{
    public partial class ViewCentralStatusCheckList : UserControl
    {
        public ViewCentralStatusCheckList()
        {
            InitializeComponent();
            DataContext = new ViewCentralStatusCheckListViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralStatusCheckListViewModel vm = (ViewCentralStatusCheckListViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private async void OnGerarModeloClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { DataContext: StatusChkGeralCentralModel record })
            {
                return;
            }

            if (record.status != "falta modelo")
            {
                return;
            }

            ViewCentralStatusCheckListViewModel vm = (ViewCentralStatusCheckListViewModel)DataContext;

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                var dados = new ModeloModel
                {
                    codcompladicional = record.codcompladicional,
                    tema = record.tema,
                    cadastrado_por = Environment.UserName,
                    data_cadastro = DateTime.Now
                };

                vm.Modelo = await vm.AddModeloAsync(dados, record.idtema);
                var modelo = await vm.GetModelo(vm.Modelo.id_modelo);

                vm.QryModelos = new ObservableCollection<QryModeloModel> { modelo };
                var window = new ModeloReceita(modelo)
                {
                    Owner = App.Current.MainWindow
                };
                window.ShowDialog();
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public class ViewCentralStatusCheckListViewModel : INotifyPropertyChanged
    {
        static ViewCentralStatusCheckListViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private StatusChkGeralCentralModel? item;
        public StatusChkGeralCentralModel? Item
        {
            get => item;
            set { item = value; RaisePropertyChanged(nameof(Item)); }
        }

        private ObservableCollection<StatusChkGeralCentralModel>? itens;
        public ObservableCollection<StatusChkGeralCentralModel>? Itens
        {
            get => itens;
            set { itens = value; RaisePropertyChanged(nameof(Itens)); }
        }

        private ModeloModel? modelo;
        public ModeloModel? Modelo
        {
            get => modelo;
            set { modelo = value; RaisePropertyChanged(nameof(Modelo)); }
        }

        private ObservableCollection<QryModeloModel>? qrymodelos;
        public ObservableCollection<QryModeloModel>? QryModelos
        {
            get => qrymodelos;
            set { qrymodelos = value; RaisePropertyChanged(nameof(QryModelos)); }
        }

        public async Task<ObservableCollection<StatusChkGeralCentralModel>> GetItensAsync()
        {
            const string sql = """
                SELECT *
                FROM modelos.qry_status_chk_geral_central
                ORDER BY sigla, tema, ordem;
                """;

            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<StatusChkGeralCentralModel>(sql);
            return new ObservableCollection<StatusChkGeralCentralModel>(data);
        }

        public async Task<ModeloModel> AddModeloAsync(ModeloModel modelo, long? idtema)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                if (modelo.id_modelo is null or 0)
                {
                    modelo.id_modelo = await conn.ExecuteScalarAsync<long>(
                        """
                        INSERT INTO modelos.tbl_modelos
                            (foto, tema, obs_modelo, aprovado, aprovado_por, data_aprovacao, alterado, data_alteracao,
                             liberado, liberado_por, data_liberacao, codcompladicional, cadastrado_por, data_cadastro, qtd_fiada_cascata)
                        VALUES
                            (@foto, @tema, @obs_modelo, @aprovado, @aprovado_por, @data_aprovacao, @alterado, @data_alteracao,
                             @liberado, @liberado_por, @data_liberacao, @codcompladicional, @cadastrado_por, @data_cadastro, @qtd_fiada_cascata)
                        RETURNING id_modelo;
                        """,
                        modelo,
                        transaction);
                }
                else
                {
                    await conn.ExecuteAsync(
                        """
                        UPDATE modelos.tbl_modelos
                        SET foto = @foto,
                            tema = @tema,
                            obs_modelo = @obs_modelo,
                            aprovado = @aprovado,
                            aprovado_por = @aprovado_por,
                            data_aprovacao = @data_aprovacao,
                            alterado = @alterado,
                            data_alteracao = @data_alteracao,
                            liberado = @liberado,
                            liberado_por = @liberado_por,
                            data_liberacao = @data_liberacao,
                            codcompladicional = @codcompladicional,
                            cadastrado_por = @cadastrado_por,
                            data_cadastro = @data_cadastro,
                            qtd_fiada_cascata = @qtd_fiada_cascata
                        WHERE id_modelo = @id_modelo;
                        """,
                        modelo,
                        transaction);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return modelo;
        }

        public async Task<QryModeloModel> GetModelo(long? id_modelo)
        {
            const string sql = """
                SELECT *
                FROM modelos.qrymodelos
                WHERE id_modelo = @id_modelo
                LIMIT 1;
                """;

            await using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<QryModeloModel>(sql, new { id_modelo });
        }
    }
}

