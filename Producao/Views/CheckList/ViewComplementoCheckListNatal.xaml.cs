using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
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
    /// Interação lógica para ViewComplementoCheckListNatal.xam
    /// </summary>
    public partial class ViewComplementoCheckListNatal : UserControl
    {
        public ViewComplementoCheckListNatal()
        {
            InitializeComponent();
            DataContext = new ViewComplementoCheckListNatalViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                vm.Planilhas = await vm.GetPlanilhasAsync();
                vm.Grupos = await vm.GetGruposAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                //SiglaChkListModel valor = (SiglaChkListModel)this.cbSigla.SelectedItem;
                vm.Itens = await vm.GetItensSiglaAsync(vm?.Sigla?.id_aprovado);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnPlanilhaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                //SiglaChkListModel valor = (SiglaChkListModel)this.cbSigla.SelectedItem;
                vm.Itens = await vm.GetItensPlanilhaAsync(vm?.Planilha?.planilha);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnGrupoSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                if (cbGrupo.SelectedItem is string grupo)
                {
                    vm.Itens = await vm.GetItensGrupoAsync(grupo);
                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void dgCheckListGeral_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm?.Chklist?.coduniadicional);
                vm.CheckListGeralComplementos = await vm.GetCheckListGeralComplementoAsync(vm?.Chklist?.codcompl);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {

        }

        private void dgComplemento_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;

            e.NewObject = new QryCheckListGeralComplementoModel
            {
                codcompl = vm.Chklist?.codcompl
            };
        }

        private void OnComplementoAdicionalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not RadComboBox combo ||
                combo.DataContext is not QryCheckListGeralComplementoModel record ||
                combo.SelectedItem is not TblComplementoAdicionalModel complemento)
            {
                return;
            }

            record.unidade = complemento.unidade;
            record.saldoestoque = complemento.saldo_estoque;

        }

        private void dgComplemento_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell?.Column?.UniqueName != "codcompladicional" ||
                e.Cell.DataContext is not QryCheckListGeralComplementoModel record)
            {
                return;
            }

            ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
            var complemento = vm.CompleAdicionais?.FirstOrDefault(x => x.codcompladicional == record.codcompladicional);
            if (complemento is null)
            {
                return;
            }

            record.unidade = complemento.unidade;
            record.saldoestoque = complemento.saldo_estoque;
            dgComplemento.Rebind();
        }

        private async void dgComplemento_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                if (e.Row.Item is not QryCheckListGeralComplementoModel data)
                {
                    return;
                }

                vm.DetCompl = new()
                {
                    coddetalhescompl = data?.coddetalhescompl,
                    codcompl = data.codcompl,
                    codcompladicional = data.codcompladicional,
                    qtd = data.qtd,
                    confirmado = data.confirmado,
                    confirmado_data = data.confirmado == "-1" ? DateTime.Now : null,
                    confirmado_por = data.confirmado == "-1" ? Environment.UserName : null,
                    desabilitado_confirmado_data = data.confirmado == "-1" ? DateTime.Now : null,
                    desabilitado_confirmado_por = data.confirmado == "-1" ? Environment.UserName : null
                };

                vm.DetCompl = await vm.AddDetalhesComplementoCheckListAsync(vm.DetCompl);
                data.coddetalhescompl = vm.DetCompl.coddetalhescompl;
                dgComplemento.Rebind();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.CheckListGeralComplementos.Where(x => x.coddetalhescompl == null).ToList();
                foreach (var item in toRemove)
                    vm.CheckListGeralComplementos.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRequisicaoClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { DataContext: QryCheckListGeralComplementoModel detalhe })
            {
                return;
            }

            ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
            if (vm.Chklist is null)
            {
                MessageBox.Show("Selecione um item antes de abrir a requisição.");
                return;
            }

            var sigla = vm.Siglas?.FirstOrDefault(s => s.id_aprovado == vm.Chklist.id_aprovado)
                        ?? vm.Siglas?.FirstOrDefault(s => s.sigla_serv == vm.Chklist.sigla);

            if (sigla is null)
            {
                MessageBox.Show("Não foi possível localizar a sigla para abrir a requisição.");
                return;
            }

            var helper = new CheckListViewModel
            {
                Sigla = sigla,
                CheckListGeral = new QryCheckListGeralModel
                {
                    sigla = vm.Chklist.sigla,
                    planilha = vm.Chklist.planilha,
                    codigo = vm.Chklist.codproduto,
                    coduniadicional = vm.Chklist.coduniadicional,
                    codcompl = vm.Chklist.codcompl,
                    descricao = vm.Chklist.descricao,
                    descricao_adicional = vm.Chklist.descricao_adicional,
                    orient_montagem = vm.Chklist.orient_montagem,
                    obs = vm.Chklist.obs,
                    id_aprovado = vm.Chklist.id_aprovado,
                    nivel = vm.Chklist.nivel,
                    local_shoppings = vm.Chklist.local_shoppings
                },
                CheckListGeralComplemento = detalhe
            };

            helper.ChangeCanExecute(sender);
        }

        private void dgComplemento_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row.Item is not QryCheckListGeralComplementoModel rowData)
            {
                return;
            }

            if (!rowData.codcompl.HasValue)
            {
                AddValidation(e, "codcompladicional", "Erro ao selecionar a linha.");
                AddValidation(e, "qtd", "Erro ao selecionar a linha.");
            }
            else if (!rowData.codcompladicional.HasValue)
            {
                AddValidation(e, "codcompladicional", "Seleciona o COMPLEMENTO ADICIONAL.");
            }
            else if (rowData.qtd == null)
            {
                AddValidation(e, "qtd", "Informa a QTDE.");
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

        public class ViewComplementoCheckListNatalViewModel : INotifyPropertyChanged
        {

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
            private ObservableCollection<RelplanModel> _planilhas;
            public ObservableCollection<RelplanModel> Planilhas
            {
                get { return _planilhas; }
                set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
            }
            private RelplanModel _planilha;
            public RelplanModel Planilha
            {
                get { return _planilha; }
                set { _planilha = value; RaisePropertyChanged("Planilha"); }
            }

            private ObservableCollection<string> _grupos;
            public ObservableCollection<string> Grupos
            {
                get { return _grupos; }
                set { _grupos = value; RaisePropertyChanged("Grupos"); }
            }

            private ObservableCollection<ChklistNaoCompletadoModel> _itens;
            public ObservableCollection<ChklistNaoCompletadoModel> Itens
            {
                get { return _itens; }
                set { _itens = value; RaisePropertyChanged("Itens"); }
            }
            private ChklistNaoCompletadoModel _chklist;
            public ChklistNaoCompletadoModel Chklist
            {
                get { return _chklist; }
                set { _chklist = value; RaisePropertyChanged("Chklist"); }
            }

            private ObservableCollection<TblComplementoAdicionalModel> _compleAdicionais;
            public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
            {
                get { return _compleAdicionais; }
                set { _compleAdicionais = value; RaisePropertyChanged("CompleAdicionais"); }
            }
            private TblComplementoAdicionalModel _compledicional;
            public TblComplementoAdicionalModel Compledicional
            {
                get { return _compledicional; }
                set { _compledicional = value; RaisePropertyChanged("Compledicional"); }
            }

            private QryCheckListGeralComplementoModel _checkListGeralComplemento;
            public QryCheckListGeralComplementoModel CheckListGeralComplemento
            {
                get { return _checkListGeralComplemento; }
                set { _checkListGeralComplemento = value; RaisePropertyChanged("CheckListGeralComplemento"); }
            }
            private ObservableCollection<QryCheckListGeralComplementoModel> _checkListGeralComplementos;
            public ObservableCollection<QryCheckListGeralComplementoModel> CheckListGeralComplementos
            {
                get { return _checkListGeralComplementos; }
                set { _checkListGeralComplementos = value; RaisePropertyChanged("CheckListGeralComplementos"); }
            }

            private DetalhesComplemento _detCompl;
            public DetalhesComplemento DetCompl
            {
                get { return _detCompl; }
                set { _detCompl = value; RaisePropertyChanged("DetCompl"); }
            }
            private ObservableCollection<DetalhesComplemento> _detCompls;
            public ObservableCollection<DetalhesComplemento> DetCompls
            {
                get { return _detCompls; }
                set { _detCompls = value; RaisePropertyChanged("DetCompls"); }
            }

            public async Task<ObservableCollection<SiglaChkListModel>> GetSiglasAsync()
            {
                try
                {
                    const string sql = """
                        SELECT sigla,
                               sigla_serv,
                               nome,
                               tema,
                               CASE
                                   WHEN data_de_expedicao IS NULL THEN NULL
                                   ELSE data_de_expedicao::timestamp
                               END AS data_de_expedicao,
                               nivel,
                               baia_local,
                               pa,
                               kit_pa,
                               tipo_arvore,
                               kit_enf_arv_p,
                               forracao,
                               tipo_festao,
                               obs_materiais,
                               iluminacao,
                               obs_iluminacao,
                               id_aprovado,
                               staus,
                               laco,
                               cor_predominante
                        FROM producao.view_sigla_chkgeral
                        ORDER BY sigla_serv;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<SiglaChkListModel>(sql);
                    return new ObservableCollection<SiglaChkListModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
            {
                try
                {
                    const string sql = """
                        SELECT *
                        FROM producao.relplan
                        WHERE ativo = '1'
                          AND planilha NOT LIKE '%ESTOQUE%'
                          AND planilha NOT LIKE '%ALMOX%'
                        ORDER BY planilha;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<RelplanModel>(sql);
                    return new ObservableCollection<RelplanModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ObservableCollection<string>> GetGruposAsync()
            {
                try
                {
                    const string sql = """
                        SELECT DISTINCT agrupamento
                        FROM producao.relplan
                        WHERE ativo = '1'
                          AND planilha NOT LIKE '%ESTOQUE%'
                          AND planilha NOT LIKE '%ALMOX%'
                          AND agrupamento IS NOT NULL
                        ORDER BY agrupamento;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<string>(sql);
                    return new ObservableCollection<string>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ObservableCollection<ChklistNaoCompletadoModel>> GetItensSiglaAsync(long? id_aprovado)
            {
                try
                {
                    const string sql = """
                        SELECT coddetalhescompl,
                               ordem,
                               agrupamento,
                               sigla,
                               local_shoppings,
                               codproduto,
                               obs,
                               dataalteracaodesc,
                               alteradopor,
                               orient_montagem,
                               item_memorial,
                               datainclusaodesc,
                               incluidopordesc,
                               kp,
                               kp2,
                               qtd,
                               coduniadicional,
                               dataalteradescadic,
                               alteradopordescadic,
                               codcompl,
                               nivel,
                               descricao,
                               planilha,
                               descricao_adicional,
                               ok,
                               confirmado,
                               CASE
                                   WHEN fechamento_shopp IS NULL THEN NULL
                                   ELSE fechamento_shopp::timestamp
                               END AS fechamento_shopp,
                               CASE
                                   WHEN data_de_expedicao IS NULL THEN NULL
                                   ELSE data_de_expedicao::timestamp
                               END AS data_de_expedicao,
                               baia_local,
                               ok_revisao_alterada,
                               id_aprovado
                        FROM producao.qry_chklist_nao_completado
                        WHERE id_aprovado = @id_aprovado
                        ORDER BY planilha;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<ChklistNaoCompletadoModel>(sql, new { id_aprovado });
                    return new ObservableCollection<ChklistNaoCompletadoModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ObservableCollection<ChklistNaoCompletadoModel>> GetItensPlanilhaAsync(string planilha)
            {
                try
                {
                    const string sql = """
                        SELECT coddetalhescompl,
                               ordem,
                               agrupamento,
                               sigla,
                               local_shoppings,
                               codproduto,
                               obs,
                               dataalteracaodesc,
                               alteradopor,
                               orient_montagem,
                               item_memorial,
                               datainclusaodesc,
                               incluidopordesc,
                               kp,
                               kp2,
                               qtd,
                               coduniadicional,
                               dataalteradescadic,
                               alteradopordescadic,
                               codcompl,
                               nivel,
                               descricao,
                               planilha,
                               descricao_adicional,
                               ok,
                               confirmado,
                               CASE
                                   WHEN fechamento_shopp IS NULL THEN NULL
                                   ELSE fechamento_shopp::timestamp
                               END AS fechamento_shopp,
                               CASE
                                   WHEN data_de_expedicao IS NULL THEN NULL
                                   ELSE data_de_expedicao::timestamp
                               END AS data_de_expedicao,
                               baia_local,
                               ok_revisao_alterada,
                               id_aprovado
                        FROM producao.qry_chklist_nao_completado
                        WHERE planilha = @planilha
                        ORDER BY planilha;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<ChklistNaoCompletadoModel>(sql, new { planilha });
                    return new ObservableCollection<ChklistNaoCompletadoModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ObservableCollection<ChklistNaoCompletadoModel>> GetItensGrupoAsync(string grupo)
            {
                try
                {
                    const string sql = """
                        SELECT coddetalhescompl,
                               ordem,
                               agrupamento,
                               sigla,
                               local_shoppings,
                               codproduto,
                               obs,
                               dataalteracaodesc,
                               alteradopor,
                               orient_montagem,
                               item_memorial,
                               datainclusaodesc,
                               incluidopordesc,
                               kp,
                               kp2,
                               qtd,
                               coduniadicional,
                               dataalteradescadic,
                               alteradopordescadic,
                               codcompl,
                               nivel,
                               descricao,
                               planilha,
                               descricao_adicional,
                               ok,
                               confirmado,
                               CASE
                                   WHEN fechamento_shopp IS NULL THEN NULL
                                   ELSE fechamento_shopp::timestamp
                               END AS fechamento_shopp,
                               CASE
                                   WHEN data_de_expedicao IS NULL THEN NULL
                                   ELSE data_de_expedicao::timestamp
                               END AS data_de_expedicao,
                               baia_local,
                               ok_revisao_alterada,
                               id_aprovado
                        FROM producao.qry_chklist_nao_completado
                        WHERE agrupamento ILIKE @grupo
                        ORDER BY planilha;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<ChklistNaoCompletadoModel>(sql, new { grupo = $"%{grupo}%" });
                    return new ObservableCollection<ChklistNaoCompletadoModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ObservableCollection<TblComplementoAdicionalModel>> GetCompleAdicionaisAsync(long? coduniadicional)
            {
                try
                {
                    CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                    const string sql = """
                        SELECT *
                        FROM producao.tblcomplementoadicional
                        WHERE coduniadicional = @coduniadicional
                          AND COALESCE(inativo, '') <> '-1'
                        ORDER BY complementoadicional;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<TblComplementoAdicionalModel>(sql, new { coduniadicional });

                    return new ObservableCollection<TblComplementoAdicionalModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<ObservableCollection<QryCheckListGeralComplementoModel>> GetCheckListGeralComplementoAsync(long? codcompl)
            {
                try
                {
                    const string sql = """
                        SELECT coddetalhescompl,
                               codcompladicional,
                               qtd,
                               saldoestoque,
                               data_alteracao,
                               alterado_por,
                               codcompl,
                               confirmado,
                               complementoadicional,
                               unidade,
                               local_producao,
                               justificativa,
                               resp_prod,
                               confirmado_por,
                               CASE
                                   WHEN confirmado_data IS NULL THEN NULL
                                   ELSE confirmado_data::timestamp
                               END AS confirmado_data,
                               desabilitado_confirmado_por,
                               CASE
                                   WHEN desabilitado_confirmado_data IS NULL THEN NULL
                                   ELSE desabilitado_confirmado_data::timestamp
                               END AS desabilitado_confirmado_data,
                               transf_galpao,
                               terceiro,
                               CASE
                                   WHEN meta_producao IS NULL THEN NULL
                                   ELSE meta_producao::timestamp
                               END AS meta_producao,
                               os,
                               req,
                               status_producao,
                               status_transferencia,
                               num_os_produto,
                               qtd_expedida
                        FROM producao.qrychkgeral_complemento
                        WHERE codcompl = @codcompl
                        ORDER BY coddetalhescompl;
                        """;
                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                    var data = await conn.QueryAsync<QryCheckListGeralComplementoModel>(sql, new { codcompl });

                    return new ObservableCollection<QryCheckListGeralComplementoModel>(data);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<DetalhesComplemento> AddDetalhesComplementoCheckListAsync(DetalhesComplemento detCompl)
            {
                try
                {
                    detCompl.alterado_por = Environment.UserName;
                    detCompl.data_alteracao = DateTime.Now;

                    await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);

                    if (!detCompl.coddetalhescompl.HasValue || detCompl.coddetalhescompl.Value == 0)
                    {
                        detCompl.inserido_por ??= Environment.UserName;
                        detCompl.data_inserido ??= DateTime.Now;

                        const string insertSql = """
                            INSERT INTO producao.tbldetalhescomplemento
                            (
                                codcompladicional,
                                qtd,
                                data_alteracao,
                                alterado_por,
                                codcompl,
                                confirmado,
                                local_producao,
                                justificativa,
                                resp_prod,
                                confirmado_por,
                                confirmado_data,
                                desabilitado_confirmado_por,
                                desabilitado_confirmado_data,
                                transf_galpao,
                                terceiro,
                                meta_producao,
                                num_os_produto,
                                status_producao,
                                status_transferencia,
                                data_inserido,
                                inserido_por
                            )
                            VALUES
                            (
                                @codcompladicional,
                                @qtd,
                                @data_alteracao,
                                @alterado_por,
                                @codcompl,
                                @confirmado,
                                @local_producao,
                                @justificativa,
                                @resp_prod,
                                @confirmado_por,
                                @confirmado_data,
                                @desabilitado_confirmado_por,
                                @desabilitado_confirmado_data,
                                @transf_galpao,
                                @terceiro,
                                @meta_producao,
                                @num_os_produto,
                                @status_producao,
                                @status_transferencia,
                                @data_inserido,
                                @inserido_por
                            )
                            RETURNING coddetalhescompl;
                            """;

                        detCompl.coddetalhescompl = await conn.ExecuteScalarAsync<long?>(insertSql, detCompl);
                    }
                    else
                    {
                        const string updateSql = """
                            UPDATE producao.tbldetalhescomplemento
                            SET codcompladicional = @codcompladicional,
                                qtd = @qtd,
                                data_alteracao = @data_alteracao,
                                alterado_por = @alterado_por,
                                codcompl = @codcompl,
                                confirmado = @confirmado,
                                local_producao = @local_producao,
                                justificativa = @justificativa,
                                resp_prod = @resp_prod,
                                confirmado_por = @confirmado_por,
                                confirmado_data = @confirmado_data,
                                desabilitado_confirmado_por = @desabilitado_confirmado_por,
                                desabilitado_confirmado_data = @desabilitado_confirmado_data,
                                transf_galpao = @transf_galpao,
                                terceiro = @terceiro,
                                meta_producao = @meta_producao,
                                num_os_produto = @num_os_produto,
                                status_producao = @status_producao,
                                status_transferencia = @status_transferencia
                            WHERE coddetalhescompl = @coddetalhescompl;
                            """;

                        await conn.ExecuteAsync(updateSql, detCompl);
                    }

                    return detCompl;
                }
                catch (NpgsqlException)
                {
                    throw;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string propName)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }
}
