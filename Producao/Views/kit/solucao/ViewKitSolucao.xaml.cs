using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.kit.solucao
{
    /// <summary>
    /// Interacao logica para ViewKitSolucao.xaml
    /// </summary>
    public partial class ViewKitSolucao : UserControl
    {
        private bool inicializado;

        public ViewKitSolucao()
        {
            InitializeComponent();
            DataContext = new KitSolucaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (inicializado)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;
                await vm.CriarOSAsync();
                vm.Siglas = await vm.GetSiglasAsync();
                inicializado = true;
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;
                Mouse.OverrideCursor = Cursors.Wait;
                vm.OsKits = await vm.GetOsKitsAsync(vm?.Sigla?.num_os);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void RadGridView_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;

            e.NewObject = new OsKitSolucaoModel
            {
                data_emissao = DateTime.Now,
                data_solicitacao = DateTime.Now,
                t_os_mont = vm.Sigla?.num_os,
                tipo_manutencao = "0",
                shopping = vm.Sigla?.cliente
            };
        }

        private void RadGridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == GridViewEditOperationType.None || e.Row.Item is not OsKitSolucaoModel rowData)
                return;

            if (!rowData.t_os_mont.HasValue)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Erro ao selecionar sigla.", PropertyName = string.Empty });
            }
            else if (rowData.solicitante == null)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informa o Solicitante.", PropertyName = "solicitante" });
            }
            else if (rowData.atendente == null)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informa o Atendente.", PropertyName = "atendente" });
            }
            else if (rowData.noite_montagem == null)
            {
                e.IsValid = false;
                e.ValidationResults.Add(new GridViewCellValidationResult { ErrorMessage = "Informa a Noite de Montagem.", PropertyName = "noite_montagem" });
            }
        }

        private async void RadGridView_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            try
            {
                KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;

                if (e.Row.Item is not OsKitSolucaoModel data)
                    return;

                Mouse.OverrideCursor = Cursors.Wait;
                await vm.AddOsKitsAsync(data);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void OnDetalhesClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not KitSolucaoViewModel vm ||
                sender is not RadButton { DataContext: OsKitSolucaoModel osKit })
            {
                return;
            }

            vm.OsKit = osKit;

            if (vm.RowDetalhesCommand?.CanExecute(osKit) == true)
                vm.RowDetalhesCommand.Execute(osKit);
        }
    }

    public class KitSolucaoViewModel : INotifyPropertyChanged
    {
        private readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ICommand rowDetalhesCommand;
        public ICommand RowDetalhesCommand
        {
            get { return rowDetalhesCommand; }
            set { rowDetalhesCommand = value; }
        }

        private ObservableCollection<TblServicoModel>? _siglas;
        public ObservableCollection<TblServicoModel>? Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged(nameof(Siglas)); }
        }

        private TblServicoModel? _sigla;
        public TblServicoModel? Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged(nameof(Sigla)); }
        }

        private ObservableCollection<OsKitSolucaoModel>? _osKits;
        public ObservableCollection<OsKitSolucaoModel>? OsKits
        {
            get { return _osKits; }
            set { _osKits = value; RaisePropertyChanged(nameof(OsKits)); }
        }

        private OsKitSolucaoModel? _osKit;
        public OsKitSolucaoModel? OsKit
        {
            get { return _osKit; }
            set { _osKit = value; RaisePropertyChanged(nameof(OsKit)); }
        }

        public KitSolucaoViewModel()
        {
            rowDetalhesCommand = new RelayCommand(DetalhesCanExecute);
        }

        public void DetalhesCanExecute(object obj)
        {
            var osKit = obj as OsKitSolucaoModel ?? OsKit;

            if (osKit == null)
            {
                MessageBox.Show("A linha nao foi totalmente inserida.", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OsKit = osKit;
            ((MainWindow)Application.Current.MainWindow).adicionarFilho(new ViewDetalhesKitSolucao(osKit), $"DETALHES KIT SOLUÇÃO {osKit.os}", "DETALHES_KIT_SOLUCAO");
        }

        public async Task<ObservableCollection<TblServicoModel>> GetSiglasAsync()
        {
            const string sql = @"
                SELECT *
                FROM producao.tbl_servicos
                WHERE tipo = 'KIT SOLUÇÃO'
                ORDER BY sigla;";

            await using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
            var data = await conn.QueryAsync<TblServicoModel>(sql);
            return new ObservableCollection<TblServicoModel>(data);
        }

        public async Task<ObservableCollection<OsKitSolucaoModel>> GetOsKitsAsync(long? os_mont)
        {
            const string sql = @"
                SELECT *
                FROM kitsolucao.t_os_kitsolucao
                WHERE t_os_mont = @os_mont
                ORDER BY os;";

            await using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
            var data = await conn.QueryAsync<OsKitSolucaoModel>(sql, new { os_mont });
            return new ObservableCollection<OsKitSolucaoModel>(data);
        }

        public async Task AddOsKitsAsync(OsKitSolucaoModel? osKit)
        {
            if (osKit is null)
                return;

            await using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);

            if (osKit.os.HasValue && osKit.os.Value > 0)
            {
                await conn.ExecuteAsync(UpdateOsKitSql, osKit);
                return;
            }

            osKit.os = await conn.ExecuteScalarAsync<long>(InsertOsKitSql, osKit);
        }

        public async Task EditOsKitsAsync(OsKitSolucaoModel? osKit)
        {
            if (osKit is null)
                return;

            await using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
            await conn.ExecuteAsync(UpdateOsKitSql, osKit);
        }

        public async Task CriarOSAsync()
        {
            await using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);

            await conn.ExecuteAsync(
                @"INSERT INTO producao.tbl_servicos (
                    tipo,
                    codigo_setor,
                    descricao_setor,
                    descricao_servico,
                    orientacao,
                    quantidade,
                    data_emissao,
                    emitido_por,
                    planilha,
                    cliente,
                    data_conclusao,
                    codigo_servico,
                    sigla
                )
                SELECT
                    producao.tbl_tipo_os.tipo_servico AS tipo,
                    79 AS codigo_setor,
                    'TODOS TAB/JAC' AS descricao_setor,
                    producao.tbl_tipo_os.descricao_servico || ' ' || EXTRACT(YEAR FROM CURRENT_DATE) AS descricao_servico,
                    'OS REF GRUPOS DE MOMADES' AS orientacao,
                    1 AS quantidade,
                    CURRENT_DATE AS data_emissao,
                    'SISTEMA' AS emitido_por,
                    'TODAS' AS planilha,
                    CASE 
                        WHEN tipo_servico = 'KIT MANUTENÇÃO' THEN sigla_serv || '-M'
                        WHEN tipo_servico = 'KIT SOLUÇÃO' THEN sigla_serv || '-S'
                        WHEN tipo_servico = 'KIT DESMONTAGEM' THEN sigla_serv || '-D'
                        ELSE sigla_serv
                    END AS cliente,
                    make_date(EXTRACT(YEAR FROM CURRENT_DATE)::integer, 12, 31) AS data_conclusao,
                    producao.tbl_tipo_os.codigo_tipo AS codigo_servico,
                    qry_siglas_emit_os.sigla_serv
                FROM
                    producao.tbl_tipo_os
                    CROSS JOIN producao.qry_siglas_emit_os
                WHERE
                    producao.tbl_tipo_os.tipo_servico IN ('KIT MANUTENÇÃO', 'KIT SOLUÇÃO', 'KIT DESMONTAGEM')
                GROUP BY
                    producao.tbl_tipo_os.tipo_servico,
                    producao.tbl_tipo_os.descricao_servico,
                    producao.tbl_tipo_os.codigo_tipo,
                    qry_siglas_emit_os.sigla_serv
                ORDER BY
                    CASE 
                        WHEN tipo_servico = 'KIT MANUTENÇÃO' THEN sigla_serv || '-M'
                        WHEN tipo_servico = 'KIT SOLUÇÃO' THEN sigla_serv || '-S'
                        WHEN tipo_servico = 'KIT DESMONTAGEM' THEN sigla_serv || '-D'
                        ELSE sigla_serv
                    END;");
        }

        private const string InsertOsKitSql = @"
            INSERT INTO kitsolucao.t_os_kitsolucao (
                t_os_mont, shopping, data_emissao, data_solicitacao, solicitante,
                concluir_ate, forma_de_envio, responsavel, obs_de_envio, valor_estimado,
                noite_montagem, volumes, atendente, cod_solicita_transporte,
                tipo_manutencao, status, status_por, status_data, id_manutencao
            )
            VALUES (
                @t_os_mont, @shopping, @data_emissao, @data_solicitacao, @solicitante,
                @concluir_ate, @forma_de_envio, @responsavel, @obs_de_envio, @valor_estimado,
                @noite_montagem, @volumes, @atendente, @cod_solicita_transporte,
                @tipo_manutencao, @status, @status_por, @status_data, @id_manutencao
            )
            RETURNING os;";

        private const string UpdateOsKitSql = @"
            UPDATE kitsolucao.t_os_kitsolucao SET
                t_os_mont = @t_os_mont,
                shopping = @shopping,
                data_emissao = @data_emissao,
                data_solicitacao = @data_solicitacao,
                solicitante = @solicitante,
                concluir_ate = @concluir_ate,
                forma_de_envio = @forma_de_envio,
                responsavel = @responsavel,
                obs_de_envio = @obs_de_envio,
                valor_estimado = @valor_estimado,
                noite_montagem = @noite_montagem,
                volumes = @volumes,
                atendente = @atendente,
                cod_solicita_transporte = @cod_solicita_transporte,
                tipo_manutencao = @tipo_manutencao,
                status = @status,
                status_por = @status_por,
                status_data = @status_data,
                id_manutencao = @id_manutencao
            WHERE os = @os;";
    }
}
