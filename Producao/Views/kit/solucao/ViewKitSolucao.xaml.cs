using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.DataBase.Model;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.kit.solucao
{
    /// <summary>
    /// Interação lógica para ViewKitSolucao.xam
    /// </summary>
    public partial class ViewKitSolucao : UserControl
    {
        public ViewKitSolucao()
        {
            InitializeComponent();
            DataContext = new KitSolucaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;
                await vm.CriarOSAsync();
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.OsKits = await Task.Run(async () => await vm.GetOsKitsAsync(vm?.Sigla?.num_os));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void SfDataGrid_AddNewRowInitiating(object sender, AddNewRowInitiatingEventArgs e)
        {
            KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;

            ((OsKitSolucaoModel)e.NewObject).data_emissao = DateTime.Now;
            ((OsKitSolucaoModel)e.NewObject).data_solicitacao = DateTime.Now;
            ((OsKitSolucaoModel)e.NewObject).t_os_mont = vm.Sigla.num_os;
            ((OsKitSolucaoModel)e.NewObject).tipo_manutencao = "0";
            ((OsKitSolucaoModel)e.NewObject).shopping = vm.Sigla.cliente;
        }

        private void SfDataGrid_RowValidating(object sender, RowValidatingEventArgs e)
        {
            OsKitSolucaoModel rowData = (OsKitSolucaoModel)e.RowData;
            if (!rowData.t_os_mont.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("data_emissao", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("solicitante", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("concluir_ate", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("atendente", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("data_solicitacao", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("noite_montagem", "Erro ao selecionar sigla.");
                e.ErrorMessages.Add("obs_de_envio", "Erro ao selecionar sigla.");
            }
            else if (rowData.solicitante == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("solicitante", "Informa o Solicitante.");
            }
            else if (rowData.atendente == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("atendente", "Informa o Atendente.");
            }
            else if (rowData.noite_montagem == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("noite_montagem", "Informa a Noite de Montagem.");
            }
        }

        private async void SfDataGrid_RowValidated(object sender, RowValidatedEventArgs e)
        {
            try
            {
                var sfdatagrid = sender as SfDataGrid;
                KitSolucaoViewModel vm = (KitSolucaoViewModel)DataContext;

                OsKitSolucaoModel data = (OsKitSolucaoModel)e.RowData;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                /*
                if (this.osKit.View.IsAddingNew)
                {
                    await Task.Run(() => vm.AddOsKitsAsync(data));
                    //AddOsKitsAsync
                }
                else if (this.osKit.View.IsEditingItem)
                {
                    await Task.Run(() => vm.EditOsKitsAsync(data));
                    //EditOsKitsAsync
                }
                */

                //data.concluir_ate = data.concluir_ate.Value.Date;
                //data.data_emissao = data.data_emissao.Value.Date;
                //data.data_solicitacao = data.data_solicitacao.Value.Date;

                await Task.Run(() => vm.AddOsKitsAsync(data));

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

       
    }

    public class KitSolucaoViewModel : INotifyPropertyChanged
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ICommand rowDetalhesCommand { get; set; }
        public ICommand RowDetalhesCommand
        {
            get { return rowDetalhesCommand; }
            set { rowDetalhesCommand = value; }
        }

        private ObservableCollection<TblServicoModel> _siglas;
        public ObservableCollection<TblServicoModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }

        private TblServicoModel _sigla;
        public TblServicoModel Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged("Sigla"); }
        }

        private ObservableCollection<OsKitSolucaoModel> _osKits;
        public ObservableCollection<OsKitSolucaoModel> OsKits
        {
            get { return _osKits; }
            set { _osKits = value; RaisePropertyChanged("OsKits"); }
        }

        private OsKitSolucaoModel _osKit;
        public OsKitSolucaoModel OsKit
        {
            get { return _osKit; }
            set { _osKit = value; RaisePropertyChanged("OsKit"); }
        }

        public KitSolucaoViewModel()
        {
            rowDetalhesCommand = new RelayCommand(DetalhesCanExecute);
        }

        public async void DetalhesCanExecute(object obj)
        {
            //GRIobj = { Syncfusion.UI.Xaml.Grid.SfDataGrid}
            var sfdatagrid = obj as SfDataGrid;

            if (OsKit == null)
            {
                MessageBox.Show("A linha não foi totalmente inserida.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ((MainWindow)Application.Current.MainWindow).adicionarFilho(new ViewDetalhesKitSolucao(OsKit), $"DETALHES KIT SOLUÇÃO {OsKit.os}", "DETALHES_KIT_SOLUCAO");
        }

        public async Task<ObservableCollection<TblServicoModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.tblServicos.OrderBy(c => c.sigla).Where(c => c.tipo.Equals("KIT SOLUÇÃO")).ToListAsync();
                return new ObservableCollection<TblServicoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OsKitSolucaoModel>> GetOsKitsAsync(long? os_mont)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.OsKitSolucaos.OrderBy(c => c.os).Where(c => c.t_os_mont == os_mont).ToListAsync();
                return new ObservableCollection<OsKitSolucaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddOsKitsAsync(OsKitSolucaoModel? osKit)
        {
            try
            {
                using DatabaseContext db = new();
                //var data = await db.OsKitSolucaos.OrderBy(c => c.os).Where(c => c.t_os_mont == os_mont).ToListAsync();
                //await db.OsKitSolucaos.AddAsync(osKit);
                await db.OsKitSolucaos.SingleMergeAsync(osKit);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task EditOsKitsAsync(OsKitSolucaoModel? osKit)
        {
            try
            {
                using DatabaseContext db = new();
                //var data = await db.OsKitSolucaos.OrderBy(c => c.os).Where(c => c.t_os_mont == os_mont).ToListAsync();
                var kit = await  db.OsKitSolucaos.FindAsync(osKit.os);
                if (kit != null)
                {
                    db.Entry(kit).CurrentValues.SetValues(osKit);
                    db.Update(osKit);
                }
                db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CriarOSAsync()
        {
            using var conn = new NpgsqlConnection(BaseSettings.connectionString);

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

    }
}
