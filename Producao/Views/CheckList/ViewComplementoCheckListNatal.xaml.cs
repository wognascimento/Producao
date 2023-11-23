using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.DataBase.Model;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                vm.Grupos = await Task.Run(vm.GetGruposAsync);
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

                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                //SiglaChkListModel valor = (SiglaChkListModel)this.cbSigla.SelectedItem;
                vm.Itens = await Task.Run(async () => await vm.GetItensSiglaAsync(vm?.Sigla?.id_aprovado));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnPlanilhaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                //SiglaChkListModel valor = (SiglaChkListModel)this.cbSigla.SelectedItem;
                vm.Itens = await Task.Run(async () => await vm.GetItensPlanilhaAsync(vm?.Planilha?.planilha));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnGrupoSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var dado = this.cbGrupo.SelectedItem;
                vm.Itens = await Task.Run(async () => await vm.GetItensGrupoAsync((string)dado));
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
                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm?.Chklist?.coduniadicional));
                vm.CheckListGeralComplementos = await Task.Run(() => vm.GetCheckListGeralComplementoAsync(vm?.Chklist?.codcompl));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {

        }

        private void dgComplemento_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;

            ((QryCheckListGeralComplementoModel)e.NewObject).codcompl = vm.Chklist.codcompl;
        }

        private void OnCurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;

            var sfdatagrid = sender as SfDataGrid;
            int RowIndex = sfdatagrid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
            var ColumnIndex = sfdatagrid.ResolveToScrollColumnIndex(3);
            QryCheckListGeralComplementoModel? record;
            if (RowIndex == -1)
            {
                record = sfdatagrid.View.CurrentAddItem as QryCheckListGeralComplementoModel;
                RowIndex = 1;
            }
            else
            {
                record = sfdatagrid.View.Records[RowIndex].Data as QryCheckListGeralComplementoModel;
            }
            record.unidade = "UNID";
            record.saldoestoque = 0;
            sfdatagrid.MoveCurrentCell(new RowColumnIndex(RowIndex, ColumnIndex));

        }

        private void dgComplemento_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {

        }

        private async void dgComplemento_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            ViewComplementoCheckListNatalViewModel vm = (ViewComplementoCheckListNatalViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                QryCheckListGeralComplementoModel data = (QryCheckListGeralComplementoModel)e.RowData;
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

                vm.DetCompl = await Task.Run(() => vm.AddDetalhesComplementoCheckListAsync(vm.DetCompl));
                ((QryCheckListGeralComplementoModel)e.RowData).coddetalhescompl = vm.DetCompl.coddetalhescompl;
                sfdatagrid.View.Refresh();
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

        private void dgComplemento_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            QryCheckListGeralComplementoModel rowData = (QryCheckListGeralComplementoModel)e.RowData;
            if (!rowData.codcompl.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("codcompladicional", "Erro ao selecionar a linha.");
                e.ErrorMessages.Add("qtd", "Erro ao selecionar a linha.");
            }
            else if (!rowData.codcompladicional.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("codcompladicional", "Seleciona o COMPLEMENTO ADICIONAL.");
            }
            else if (rowData.qtd == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("qtd", "Informa a QTDE.");
            }
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
                    using DatabaseContext db = new();
                    var data = await db.Siglas.OrderBy(c => c.sigla_serv).ToListAsync();
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
                    using DatabaseContext db = new();
                    var data = await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1") && !c.planilha.Contains("ESTOQUE") && !c.planilha.Contains("ALMOX")).ToListAsync();
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
                    using DatabaseContext db = new();

                    var data = await db.Relplans
                        .Where(c => c.ativo.Equals("1") && !c.planilha.Contains("ESTOQUE") && !c.planilha.Contains("ALMOX"))
                        .OrderBy(g => g.agrupamento)
                        .GroupBy(g => new { g.agrupamento })
                        .Select(p => p.Key.agrupamento).ToListAsync();
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
                    using DatabaseContext db = new();
                    var data = await db.ChklistNaoCompletados.OrderBy(c => c.planilha).Where(p => p.id_aprovado == id_aprovado).ToListAsync();
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
                    using DatabaseContext db = new();
                    var data = await db.ChklistNaoCompletados.OrderBy(c => c.planilha).Where(p => p.planilha == planilha).ToListAsync();
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
                    using DatabaseContext db = new();
                    var data = await db.ChklistNaoCompletados.OrderBy(c => c.planilha).Where(p => p.agrupamento.Contains(grupo)).ToListAsync();
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
                    using DatabaseContext db = new();
                    var data = await db.ComplementoAdicionais
                        .OrderBy(c => c.complementoadicional)
                        .Where(c => c.coduniadicional.Equals(coduniadicional))
                        .Where(c => c.inativo != "-1")
                        .ToListAsync();

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
                    using DatabaseContext db = new();
                    var data = await db.CheckListGeralComplementos
                        .OrderBy(c => c.coddetalhescompl)
                        .Where(c => c.codcompl == codcompl)
                        .ToListAsync();

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
                    using DatabaseContext db = new();
                    await db.DetalhesComplementos.SingleMergeAsync(detCompl);
                    await db.SaveChangesAsync();
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
