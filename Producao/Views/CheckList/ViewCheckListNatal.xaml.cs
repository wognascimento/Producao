using Dapper;
using Producao.Utils;
using Producao.Views.CentralModelos.Compat;
using Producao.Views.popup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewCheckList.xam
    /// </summary>
    public partial class ViewCheckListNatal : UserControl
    {
        private bool suppressComboCascade;
        private bool _dadosCarregados;
        private bool _restaurandoCelulaComErro;
        private readonly Dictionary<string, object?> _dgCheckListGeralValoresOriginais = new();
        private readonly HashSet<string> _dgCheckListGeralSalvamentosPendentes = new();
        private QryCheckListGeralModel? _dgCheckListGeralItemComErro;
        private Telerik.Windows.Controls.GridViewColumn? _dgCheckListGeralColunaComErro;

        DataBaseSettings BaseSettings = DataBaseSettings.Instance;


        static ViewCheckListNatal()
        {
            SqlMapper.AddTypeHandler(new DateOnlyToDateTimeHandler());
        }


        public ViewCheckListNatal()
        {
            DataContext = new CheckListViewModel();
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_dadosCarregados)
                return;

            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                vm.Planilhas = await vm.GetPlanilhasAsync();
                _dadosCarregados = true;
                
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnSelectionClient(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void OnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (sender is RadGridView grid && DeveRestaurarCelulaComErro(grid))
            {
                RestaurarEdicaoNaCelula(grid, _dgCheckListGeralItemComErro, _dgCheckListGeralColunaComErro);
                return;
            }

            //CheckListViewModel vm = (CheckListViewModel)DataContext;
            //vm.CheckListGeralComplemento = new QryCheckListGeralComplementoModel();
            //vm.CheckListGeralComplementos = new ObservableCollection<QryCheckListGeralComplementoModel>();
            //btnAddicionar.Visibility = Visibility.Collapsed;

            try
            {
               
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                /*
                var visualcontainer = this.dgCheckListGeral.GetVisualContainer();
                var rowColumnIndex = visualcontainer.PointToCellRowColumnIndex(e.GetPosition(visualcontainer));
                var recordindex = this.dgCheckListGeral.ResolveToRecordIndex(rowColumnIndex.RowIndex);
                var recordentry = this.dgCheckListGeral.View.GroupDescriptions.Count == 0 ? this.dgCheckListGeral.View.Records[recordindex] : this.dgCheckListGeral.View.TopLevelGroup.DisplayElements[recordindex];
                var record = ((RecordEntry)recordentry).Data as QryCheckListGeralModel;
                */
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                var record = vm.CheckListGeral;

                if (record == null)
                {
                    ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                vm.ComplementoCheckList = new ComplementoCheckListModel
                {
                    ordem = vm?.CheckListGeral?.id,
                    sigla = vm?.CheckListGeral?.sigla,
                    local_shoppings = vm?.CheckListGeral?.local_shoppings,
                    codproduto = vm?.CheckListGeral?.codigo,
                    obs = vm?.CheckListGeral?.obs,
                    dataalteracaodesc = vm?.CheckListGeral?.dataalteracaodesc,
                    alteradopor = vm?.CheckListGeral?.alteradopor,
                    orient_montagem = vm?.CheckListGeral?.orient_montagem,
                    item_memorial = vm?.CheckListGeral?.item_memorial,
                    incluidopordesc = vm?.CheckListGeral?.incluidopordesc,
                    kp = vm?.CheckListGeral?.kp,
                    orient_desmont = vm?.CheckListGeral?.orient_desmont,
                    qtd = vm.CheckListGeral.qtd,
                    coduniadicional = vm?.CheckListGeral?.coduniadicional,
                    codcompl = vm?.CheckListGeral?.codcompl,
                    nivel = vm?.CheckListGeral?.nivel,
                    carga = vm?.CheckListGeral?.carga,
                    class_solucao = vm?.CheckListGeral?.class_solucao,
                    motivos = vm?.CheckListGeral?.motivos,
                    id_aprovado = vm?.CheckListGeral?.id_aprovado,
                    historico = vm?.CheckListGeral?.historico,
                    agrupar = vm?.CheckListGeral?.agrupar,
                    inserido_por = vm?.CheckListGeral?.inserido_por,
                    inserido_em = vm?.CheckListGeral?.inserido_em,
                };

                suppressComboCascade = true;
                try
                {
                    vm.Planilha = (from p in vm.Planilhas where p.planilha == record?.planilha select p).FirstOrDefault();
                    vm.Produtos = await vm.GetProdutosAsync(vm?.Planilha?.planilha);
                    vm.Produto = (from p in vm.Produtos where p.codigo == record?.codigo select p).FirstOrDefault();
                    vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm?.Produto?.codigo);
                    vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == record?.coduniadicional select d).FirstOrDefault();
                }
                finally
                {
                    suppressComboCascade = false;
                }

                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm?.CheckListGeral?.coduniadicional);
                vm.CheckListGeralComplementos = await vm.GetCheckListGeralComplementoAsync(vm?.CheckListGeral?.codcompl);

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                //btnAddicionar.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

        }

        private async void OnPlanilhaSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (suppressComboCascade)
                return;

            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CheckListViewModel vm = (CheckListViewModel)DataContext;

                LimparDescricao();
                LimparDescricaoAdicional();

                vm.Produtos = string.IsNullOrWhiteSpace(vm.Planilha?.planilha)
                    ? new ObservableCollection<ProdutoModel>()
                    : await vm.GetProdutosAsync(vm.Planilha.planilha);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnDescricaoSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (suppressComboCascade)
                return;

            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CheckListViewModel vm = (CheckListViewModel)DataContext;

                LimparDescricaoAdicional();

                vm.DescAdicionais = vm.Produto?.codigo is null
                    ? new ObservableCollection<TabelaDescAdicionalModel>()
                    : await vm.GetDescAdicionaisAsync(vm.Produto.codigo);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void LimparDescricao()
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;

            vm.Produto = null;
            if (vm.ComplementoCheckList is not null)
                vm.ComplementoCheckList.coduniadicional = null;

            if (vm.ComplementoCheckList is not null)
                vm.ComplementoCheckList.codproduto = null;

            cbDescricao.SelectedItem = null;
            cbDescricao.Text = string.Empty;
        }

        private void LimparDescricaoAdicional()
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;

            vm.DescAdicional = null;
            if (vm.ComplementoCheckList is not null)
                vm.ComplementoCheckList.coduniadicional = null;

            vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
            cbDescricaoAdicional.SelectedItem = null;
            cbDescricaoAdicional.Text = string.Empty;

            vm.Compledicional = null;
            vm.CompleAdicionais = [];
        }

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                dgCheckListGeral.SelectedItem = null;
                dgComplemento.SelectedItem = null;

                Limpar();

                CheckListViewModel vm = (CheckListViewModel)DataContext;
                vm.Sigla = (SiglaChkListModel)e.NewValue;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                vm.Locaisshopping = await vm.GetLocaisShoppAsync(vm?.Sigla?.id_aprovado);
                vm.CheckListGerais = await vm.GetCheckListGeralAsync(vm?.Sigla?.id_aprovado);
                vm.CheckListGeralComplementos = [];
                vm.CheckListGeralComplemento = null;
                vm.CompleAdicionais = [];

                

                var sigla = (e.NewValue as SiglaChkListModel);
                if (sigla != null)
                {
                    if (sigla.sigla.Contains("CIPOLATTI"))
                        vm.Planilhas = await vm.GetTodasPlanilhasAsync();
                    else
                        vm.Planilhas = await vm.GetPlanilhasAsync();
                }
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                vm.Locaisshopping = await vm.GetLocaisShoppAsync(vm?.Sigla?.id_aprovado);
                vm.CheckListGerais = await vm.GetCheckListGeralAsync(vm?.Sigla?.id_aprovado);
                vm.CheckListGeralComplementos = [];
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnLimparClick(object sender, RoutedEventArgs e)
        {
            Limpar();
        }

        void Limpar()
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;

            suppressComboCascade = true;
            try
            {
                tbId.Text = string.Empty;
                tbItem.Text = string.Empty;
                tbLocalShopping.Text = string.Empty;
                cbPlanilha.SelectedItem = null;
                cbPlanilha.Text = string.Empty;
                cbDescricao.SelectedItem = null;
                cbDescricao.Text = string.Empty;
                cbDescricaoAdicional.SelectedItem = null;
                cbDescricaoAdicional.Text = string.Empty;
                tbQtde.Value = null;
                tbOrientacaoProducao.Text = string.Empty;
                tbOrientacaoMontagem.Text = string.Empty;
                tbOrientacaoDesmontagem.Text = string.Empty;

                dgCheckListGeral.SelectedItem = null;
                dgComplemento.SelectedItem = null;
                //btnAddicionar.Visibility = Visibility.Collapsed;

                vm.Produto = null;
                vm.DescAdicional = null;
                vm.Compledicional = null;
                vm.Produtos = [];
                vm.DescAdicionais = [];
                vm.CompleAdicionais = [];
                vm.CheckListGeralComplementos = [];
                vm.ComplementoCheckList = new();
            }
            finally
            {
                suppressComboCascade = false;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

            tbId.Focus();
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                CheckListViewModel vm = (CheckListViewModel)DataContext;
                var record = vm.CheckListGeral;

                if (record is null)
                {
                    ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                vm.ComplementoCheckList = new ComplementoCheckListModel
                {
                    ordem = vm.CheckListGeral.id,
                    sigla = vm.CheckListGeral.sigla,
                    local_shoppings = vm.CheckListGeral.local_shoppings,
                    codproduto = vm.CheckListGeral.codigo,
                    obs = vm.CheckListGeral.obs,
                    dataalteracaodesc = vm.CheckListGeral.dataalteracaodesc,
                    alteradopor = vm.CheckListGeral.alteradopor,
                    orient_montagem = vm.CheckListGeral.orient_montagem,
                    item_memorial = vm.CheckListGeral.item_memorial,
                    incluidopordesc = vm.CheckListGeral.incluidopordesc,
                    kp = vm.CheckListGeral.kp,
                    orient_desmont = vm.CheckListGeral.orient_desmont,
                    qtd = (double)vm.CheckListGeral.qtd,
                    coduniadicional = vm.CheckListGeral.coduniadicional,
                    codcompl = vm.CheckListGeral.codcompl,
                    nivel = vm.CheckListGeral.nivel,
                    carga = vm.CheckListGeral.carga,
                    class_solucao = vm.CheckListGeral.class_solucao,
                    id_aprovado = vm.CheckListGeral.id_aprovado,
                    historico = vm.CheckListGeral.historico,
                    agrupar = vm.CheckListGeral.agrupar
                };

                suppressComboCascade = true;
                try
                {
                    vm.Planilha = (from p in vm.Planilhas where p.planilha == record.planilha select p).FirstOrDefault();
                    vm.Produtos = await vm.GetProdutosAsync(vm.Planilha?.planilha);
                    vm.Produto = (from p in vm.Produtos where p.codigo == record.codigo select p).FirstOrDefault();
                    vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm.Produto?.codigo);
                    vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == record.coduniadicional select d).FirstOrDefault();
                }
                finally
                {
                    suppressComboCascade = false;
                }

                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm.CheckListGeral.coduniadicional);
                vm.CheckListGeralComplementos = await vm.GetCheckListGeralComplementoAsync(vm.CheckListGeral.codcompl);

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                //btnAddicionar.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            

        }

        private async void OnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.ComplementoCheckList.codcompl = null;
                vm.ComplementoCheckList.sigla = vm.Sigla.sigla_serv;
                vm.ComplementoCheckList.id_aprovado = vm.Sigla.id_aprovado;
                vm.ComplementoCheckList.inserido_por = Environment.UserName;
                vm.ComplementoCheckList.inserido_em = DateTime.Now;

                ComplementoCheckListModel compl = await vm.AddComplementoCheckListAsync(vm.ComplementoCheckList);

                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                var locais = await vm.GetLocaisShoppAsync(vm.Sigla.id_aprovado);
                vm.Locaisshopping = locais;

                vm.CheckListGerais = await vm.GetCheckListGeralAsync(vm.Sigla.id_aprovado);
                SelecionarCheckListPorCodCompl(vm, compl.codcompl);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro ao inserir");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            
        }

        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckListViewModel vm = (CheckListViewModel)DataContext;

                if (!PrepararComplementoParaEdicao(vm))
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.ComplementoCheckList.alterado_por = Environment.UserName;
                vm.ComplementoCheckList.alterado_em = DateTime.Now;
                ComplementoCheckListModel compl = await vm.AddComplementoCheckListAsync(vm.ComplementoCheckList);

                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                var locais = await vm.GetLocaisShoppAsync(vm.Sigla.id_aprovado);
                vm.Locaisshopping = locais;

                vm.CheckListGerais = await vm.GetCheckListGeralAsync(vm.Sigla.id_aprovado);
                SelecionarCheckListPorCodCompl(vm, compl.codcompl);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro ao alterar");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private bool PrepararComplementoParaEdicao(CheckListViewModel vm)
        {
            if (vm.ComplementoCheckList?.codcompl is > 0)
                return true;

            if (vm.CheckListGeral?.codcompl is > 0)
            {
                vm.ComplementoCheckList ??= new ComplementoCheckListModel();
                vm.ComplementoCheckList.codcompl = vm.CheckListGeral.codcompl;
                return true;
            }

            MessageBox.Show("Selecione uma linha do checklist antes de editar.", "Editar checklist", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void SelecionarCheckListPorCodCompl(CheckListViewModel vm, long? codcompl)
        {
            var item = vm.CheckListGerais?.FirstOrDefault(x => x.codcompl == codcompl);
            if (item is null)
            {
                dgCheckListGeral.Rebind();
                return;
            }

            dgCheckListGeral.SelectedItem = item;
            dgCheckListGeral.ScrollIntoViewAsync(item, null);
            dgCheckListGeral.Rebind();
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

        private void OnAddComplemento(object sender, RoutedEventArgs e)
        {

            CheckListViewModel vm = (CheckListViewModel)DataContext;

            if(vm.CheckListGeral == null)
            {
                MessageBox.Show("Seleciona uma linha para poder completas", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            vm.DetCompl = new DetalhesComplemento();
            AddDetalhesComplemento detailsWindow = new AddDetalhesComplemento(vm);
            detailsWindow.ShowDialog();
        }

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                CheckListViewModel vm = (CheckListViewModel)DataContext;
                DataBaseSettings BaseSettings = DataBaseSettings.Instance;
                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.IsGridLinesVisible = false;

                IStyle headerStyle;
                IStyle bodyStyle;

                bodyStyle = workbook.Styles.Add("BodyStyle");
                bodyStyle.BeginUpdate();
                bodyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                bodyStyle.Font.Bold = true;
                bodyStyle.WrapText = true;
                bodyStyle.EndUpdate();

                headerStyle = workbook.Styles.Add("headerStyle");
                headerStyle.BeginUpdate();
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.WrapText = true;
                headerStyle.EndUpdate();

                worksheet.Range["A1"].Text = $"{vm.Sigla.sigla_serv} CHECK LIST {BaseSettings.Database}";
                worksheet.Range["A1"].CellStyle.Font.Bold = true;
                worksheet.Range["A1"].CellStyle.Font.Size = 25;

                worksheet.Range["A2"].Text = $"ITEM";
                worksheet.Range["A2"].ColumnWidth = 5;

                worksheet.Range["B2"].Text = $"LOCAL";
                worksheet.Range["B2"].ColumnWidth = 20;

                worksheet.Range["C2"].Text = $"FAMÍLIA DE PRODUTO PLANILHA";
                worksheet.Range["C2"].ColumnWidth = 20;
                worksheet.Range["C2"].WrapText = true;

                worksheet.Range["D2"].Text = $"DESCRIÇÃO";
                worksheet.Range["D2"].ColumnWidth = 45;
                worksheet.Range["D2"].WrapText = true;

                worksheet.Range["E2"].Text = $"UNID";
                worksheet.Range["E2"].ColumnWidth = 5;

                worksheet.Range["F2"].Text = $"QTDE";
                worksheet.Range["F2"].ColumnWidth = 5;

                worksheet.Range["G2"].Text = $"C. UNIT";
                worksheet.Range["G2"].ColumnWidth = 5;

                worksheet.Range["H2"].Text = $"C. TOT";
                worksheet.Range["H2"].ColumnWidth = 5;

                worksheet.Range["I2"].Text = $"ORIENTAÇÃO DE MONTAGEM";
                worksheet.Range["I2"].ColumnWidth = 30;

                worksheet.Range["J2"].Text = $"COD DETALHES COMPL";
                worksheet.Range["J2"].ColumnWidth = 10;
                worksheet.Range["J2"].WrapText = true;

                worksheet.Range["K2"].Text = $"CAMINÃO";
                worksheet.Range["K2"].ColumnWidth = 10;
                worksheet.Range["K2"].WrapText = true;

                worksheet.Rows[1].CellStyle = bodyStyle;

                var dados = await vm.GetChkGeralRelatorioAsync(vm.Sigla.id_aprovado);
                worksheet.ImportData(dados, 3, 1, false);

                worksheet.Range[$"A3:K{dados.Count + 2}"].CellStyle = headerStyle;

                worksheet.Range[$"A3:A{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"A3:A{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"B3:B{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"C3:C{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"D3:D{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"E3:E{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"E3:E{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"F3:F{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"F3:F{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"G3:G{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"G3:G{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"H3:H{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"H3:H{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"I3:I{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"J3:J{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"J3:J{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"K3:K{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"K3:K{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.PageSetup.PrintTitleColumns = "$A:$K";
                worksheet.PageSetup.PrintTitleRows = "$1:$2";
                worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                worksheet.PageSetup.LeftMargin = Producao.Utils.PrintPageSetupHelper.LeftMargin;
                worksheet.PageSetup.RightMargin = Producao.Utils.PrintPageSetupHelper.RightMargin;
                worksheet.PageSetup.TopMargin = Producao.Utils.PrintPageSetupHelper.TopMargin;
                worksheet.PageSetup.BottomMargin = Producao.Utils.PrintPageSetupHelper.BottomMargin;
                worksheet.PageSetup.RightFooter = "&P";
                worksheet.PageSetup.LeftFooter = "&D";
                worksheet.PageSetup.Zoom = 88;
                //worksheet.PageSetup.CenterVertically = true;
                worksheet.PageSetup.CenterHorizontally = true;

                workbook.SaveAs(BaseSettings.ResolveImpressosPath($"CHECKLIST.xlsx"));
                Process.Start(new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"CHECKLIST.xlsx"))
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });


            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            

        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CheckListViewModel vm) return;

            if (vm.Sigla == null)
            {
                MessageBox.Show("Selecione uma SIGLA para copiar os checklists.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var meuUserControl = new CopiaChklist(vm.Sigla);
            RadWindow radWindow = new()
            {
                Content = meuUserControl,
                Header = $"Copiar Para: {vm.Sigla.sigla_serv}",
                Width = 1000,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                RestrictedAreaMargin = new Thickness(0),
                IsRestricted = false,
                ResizeMode = ResizeMode.NoResize,
                CanClose = true,
                HideMinimizeButton = true,
                HideMaximizeButton = true
            };
            // Evento disparado após fechar
            radWindow.Closed += async (sender, e) =>
            {
                // Exemplo: atualizar dados
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                vm.Locaisshopping = await vm.GetLocaisShoppAsync(vm?.Sigla?.id_aprovado);
                vm.CheckListGerais = await vm.GetCheckListGeralAsync(vm?.Sigla?.id_aprovado);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            };
            // Abre como modal
            radWindow.ShowDialog();

        }

        private async void dgComplemento_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {

            QryCheckListGeralComplementoModel? dado = e.Cell?.DataContext as QryCheckListGeralComplementoModel;
            CheckListViewModel vm = (CheckListViewModel)DataContext;

            try
            {
                if (dado is not null && e.Cell?.Column?.UniqueName == "codcompladicional")
                {
                    var complemento = vm.CompleAdicionais?.FirstOrDefault(x => x.codcompladicional == dado.codcompladicional);
                    if (complemento is not null)
                    {
                        dado.unidade = complemento.unidade;
                        dado.saldoestoque = complemento.saldo_estoque;
                        dgComplemento.Rebind();
                    }
                }

                if (dado is not null && e.Cell?.Column?.UniqueName == "confirmado")
                {
                    vm.DetCompl.coddetalhescompl = dado.coddetalhescompl;
                    vm.DetCompl.confirmado = dado.confirmado;
                    vm.DetCompl.confirmado_data = dado.confirmado == "-1" ? DateTime.Now : dado.confirmado_data;
                    vm.DetCompl.confirmado_por = dado.confirmado == "-1" ? Environment.UserName : dado.confirmado_por;
                    vm.DetCompl.desabilitado_confirmado_data = dado.confirmado == "0" ? DateTime.Now : dado.desabilitado_confirmado_data;
                    vm.DetCompl.desabilitado_confirmado_por = dado.confirmado == "0" ? Environment.UserName : dado.desabilitado_confirmado_por;
                    vm.DetCompl = await vm.ConfirmarComplementoCheckListAsync(vm.DetCompl);
                }


            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            
            
        }

        private void OnRequisicaoComplementoClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CheckListViewModel vm ||
                sender is not RadButton { DataContext: QryCheckListGeralComplementoModel complemento })
            {
                return;
            }

            vm.CheckListGeralComplemento = complemento;
            dgComplemento.SelectedItem = complemento;

            if (vm.RowDataCommand?.CanExecute(this) == true)
                vm.RowDataCommand.Execute(this);
        }

        private async void dgComplemento_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;
            try
            {
                if (e.Row.Item is not QryCheckListGeralComplementoModel data)
                {
                    return;
                }

                if (!ComplementoTemDadosParaSalvar(data))
                {
                    CancelarLinhaComplementoVazia(vm.CheckListGeralComplementos, data, dgComplemento);
                    return;
                }

                if (!ComplementoEstaValido(data))
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.DetCompl = new()
                {
                    coddetalhescompl = data?.coddetalhescompl,
                    codcompl = data.codcompl,
                    codcompladicional = data.codcompladicional,
                    qtd = data.qtd.GetValueOrDefault(),
                    confirmado = data.confirmado,
                    confirmado_data = data.confirmado == "-1" ? DateTime.Now : null,
                    confirmado_por = data.confirmado == "-1" ? Environment.UserName : null,
                    desabilitado_confirmado_data = data.confirmado == "0" ? DateTime.Now : null,
                    desabilitado_confirmado_por = data.confirmado == "0" ? Environment.UserName : null,
                    local_producao = "JACAREÍ",
                    os = data.os
                };

                vm.DetCompl = await vm.AddDetalhesComplementoCheckListAsync(vm.DetCompl);
                data.coddetalhescompl = vm.DetCompl.coddetalhescompl;
                dgComplemento.Rebind();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.CheckListGeralComplementos.Where(x => x.coddetalhescompl == null).ToList();
                foreach (var item in toRemove)
                    vm.CheckListGeralComplementos.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void dgComplemento_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row.Item is not QryCheckListGeralComplementoModel rowData)
            {
                return;
            }

            if (!ComplementoTemDadosParaSalvar(rowData))
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
            else if (!QuantidadeComplementoInformada(rowData))
            {
                AddValidation(e, "qtd", "Informa a QTDE.");
            }
        }

        private static bool ComplementoTemDadosParaSalvar(QryCheckListGeralComplementoModel rowData)
        {
            return rowData.codcompladicional.HasValue || QuantidadeComplementoInformada(rowData);
        }

        private static bool ComplementoEstaValido(QryCheckListGeralComplementoModel rowData)
        {
            return rowData.codcompl.HasValue &&
                   rowData.codcompladicional.HasValue &&
                   QuantidadeComplementoInformada(rowData);
        }

        private static bool QuantidadeComplementoInformada(QryCheckListGeralComplementoModel rowData)
        {
            return rowData.qtd.HasValue;
        }

        private static void CancelarLinhaComplementoVazia(ObservableCollection<QryCheckListGeralComplementoModel>? itens, QryCheckListGeralComplementoModel rowData, RadGridView grid)
        {
            if (rowData.coddetalhescompl != null || itens is null)
                return;

            grid.Dispatcher.BeginInvoke(() =>
            {
                if (itens.Contains(rowData))
                    itens.Remove(rowData);

                grid.CancelEdit();
                grid.Rebind();
            });
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = propertyName,
                ErrorMessage = message
            });
        }

        private void dgComplemento_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;

            e.NewObject = new QryCheckListGeralComplementoModel
            {
                codcompl = vm.CheckListGeral?.codcompl,
                qtd = null
            };
        }

        private void chklist_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private void dgCheckListGeral_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            // Nao bloqueia a navegacao do usuario; erros de banco sao tratados no salvamento.
        }

        private void dgCheckListGeral_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            if (e.Cell?.DataContext is not QryCheckListGeralModel dado)
                return;

            var columnName = e.Cell?.Column?.UniqueName;
            if (string.IsNullOrWhiteSpace(columnName))
                return;

            _dgCheckListGeralValoresOriginais[GridValueKey(dado, columnName)] = GetCheckListGeralValue(dado, columnName);
        }

        private void dgCheckListGeral_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape || sender is not RadGridView grid)
                return;

            LiberarCelulaPendenteAtual(grid);
        }

        private async void dgCheckListGeral_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var grid = sender as RadGridView;
            QryCheckListGeralModel? dado = null;
            Telerik.Windows.Controls.GridViewColumn? colunaErro = null;
            string? chaveCelula = null;

            try
            {
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                if (e.Cell?.DataContext is not QryCheckListGeralModel rowData)
                {
                    return;
                }

                dado = rowData;
                colunaErro = e.Cell?.Column;
                _dgCheckListGeralItemComErro = null;
                _dgCheckListGeralColunaComErro = null;
                var columnName = e.Cell?.Column?.UniqueName;

                if (string.IsNullOrWhiteSpace(columnName))
                {
                    return;
                }

                chaveCelula = GridValueKey(dado, columnName);

                if (!CheckListGeralValueChanged(dado, columnName))
                {
                    return;
                }

                if (columnName == "carga")
                {
                    var confirm = MessageBox.Show("Deseja acresentar para os demais itens?", "Aletrta itens", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (confirm == MessageBoxResult.Yes)
                    {
                        var filteredResult = vm.CheckListGerais.Where(x => x.item_memorial == dado.item_memorial).ToList();
                        foreach (var item in filteredResult)
                        {
                            ComplementoCheckListModel Comple = new()
                            {
                                codcompl = item?.codcompl,
                                carga = dado?.carga,
                            };
                            item.carga = Comple.carga;
                            int i = vm.CheckListGerais.IndexOf(item);
                            vm.CheckListGerais[i] = item;
                            await vm.CargaCaminhaoListAsync(Comple);
                            grid.Rebind();
                        }
                    }
                    else
                    {
                        ComplementoCheckListModel Comple = new()
                        {
                            codcompl = dado?.codcompl,
                            carga = dado?.carga,
                        };
                        await vm.CargaCaminhaoListAsync(Comple);
                    }
                    MarcarCheckListGeralSalvo(chaveCelula);
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ComplementoCheckListModel CompleChkList = new()
                {
                    codcompl = dado?.codcompl,
                    obs = dado?.obs,
                    orient_montagem = dado?.orient_montagem,
                    orient_desmont = dado?.orient_desmont,
                    ordem = dado?.id,
                    //carga = dado?.carga,
                    local_shoppings = dado?.local_shoppings,
                    item_memorial = dado?.item_memorial,
                    alterado_por = Environment.UserName,
                    alterado_em = DateTime.Now
                };
                await vm.EditComplementoCheckListAsync(CompleChkList);
                MarcarCheckListGeralSalvo(chaveCelula);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MarcarCheckListGeralPendente(chaveCelula);
                _dgCheckListGeralItemComErro = dado;
                _dgCheckListGeralColunaComErro = colunaErro;
                Producao.ErrorDialog.Show(ex, "Erro ao alterar checklist");
                ManterEdicaoNaCelula(grid, dado, colunaErro);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private bool DeveRestaurarCelulaComErro(RadGridView grid)
        {
            return !_restaurandoCelulaComErro &&
                   _dgCheckListGeralSalvamentosPendentes.Count > 0 &&
                   _dgCheckListGeralItemComErro is not null &&
                   _dgCheckListGeralColunaComErro is not null &&
                   !ReferenceEquals(grid.SelectedItem, _dgCheckListGeralItemComErro);
        }

        private void ManterEdicaoNaCelula(RadGridView? grid, object? item, Telerik.Windows.Controls.GridViewColumn? coluna)
        {
            if (grid is null || item is null || coluna is null)
                return;

            grid.Dispatcher.BeginInvoke(() =>
            {
                RestaurarEdicaoNaCelula(grid, item, coluna);
            }, DispatcherPriority.ContextIdle);
        }

        private void RestaurarEdicaoNaCelula(RadGridView? grid, object? item, Telerik.Windows.Controls.GridViewColumn? coluna)
        {
            if (grid is null || item is null || coluna is null)
                return;

            _restaurandoCelulaComErro = true;
            try
            {
                grid.CancelEdit();
                grid.SelectedItems.Clear();
                grid.SelectedItems.Add(item);
                grid.SelectedItem = item;
                grid.CurrentItem = item;
                grid.CurrentColumn = coluna;
                grid.CurrentCellInfo = new GridViewCellInfo(item, coluna, grid);
                grid.ScrollIntoViewAsync(item, coluna, _ =>
                {
                    grid.Focus();
                    grid.BeginEdit();
                });
                grid.Focus();
                grid.BeginEdit();
            }
            finally
            {
                grid.Dispatcher.BeginInvoke(() =>
                {
                    _restaurandoCelulaComErro = false;
                }, DispatcherPriority.ContextIdle);
            }
        }

        private bool CheckListGeralValueChanged(QryCheckListGeralModel dado, string columnName)
        {
            var key = GridValueKey(dado, columnName);
            if (!_dgCheckListGeralValoresOriginais.TryGetValue(key, out var original))
                return true;

            var atual = GetCheckListGeralValue(dado, columnName);
            if (_dgCheckListGeralSalvamentosPendentes.Contains(key))
            {
                if (ValoresIguais(original, atual))
                {
                    MarcarCheckListGeralSalvo(key);
                    _dgCheckListGeralValoresOriginais.Remove(key);
                    return false;
                }

                return true;
            }

            _dgCheckListGeralValoresOriginais.Remove(key);
            return !ValoresIguais(original, atual);
        }

        private void LiberarCelulaPendenteAtual(RadGridView grid)
        {
            if (grid.CurrentItem is not QryCheckListGeralModel dado)
                return;

            var columnName = grid.CurrentColumn?.UniqueName;
            if (string.IsNullOrWhiteSpace(columnName))
                return;

            var key = GridValueKey(dado, columnName);
            if (!_dgCheckListGeralSalvamentosPendentes.Contains(key))
                return;

            RestaurarValorOriginal(dado, columnName);
            MarcarCheckListGeralSalvo(key);
            _dgCheckListGeralValoresOriginais.Remove(key);
        }

        private void RestaurarValorOriginal(QryCheckListGeralModel dado, string columnName)
        {
            var key = GridValueKey(dado, columnName);
            if (!_dgCheckListGeralValoresOriginais.TryGetValue(key, out var original))
                return;

            switch (columnName)
            {
                case "id":
                    dado.id = original as string;
                    break;
                case "item_memorial":
                    dado.item_memorial = original as string;
                    break;
                case "local_shoppings":
                    dado.local_shoppings = original as string;
                    break;
                case "qtd":
                    dado.qtd = original as double?;
                    break;
                case "obs":
                    dado.obs = original as string;
                    break;
                case "orient_montagem":
                    dado.orient_montagem = original as string;
                    break;
                case "orient_desmont":
                    dado.orient_desmont = original as string;
                    break;
                case "carga":
                    dado.carga = original as string;
                    break;
            }
        }

        private void MarcarCheckListGeralPendente(string? chaveCelula)
        {
            if (!string.IsNullOrWhiteSpace(chaveCelula))
                _dgCheckListGeralSalvamentosPendentes.Add(chaveCelula);
        }

        private void MarcarCheckListGeralSalvo(string? chaveCelula)
        {
            if (!string.IsNullOrWhiteSpace(chaveCelula))
                _dgCheckListGeralSalvamentosPendentes.Remove(chaveCelula);

            if (_dgCheckListGeralSalvamentosPendentes.Count == 0)
            {
                _dgCheckListGeralItemComErro = null;
                _dgCheckListGeralColunaComErro = null;
            }
        }

        private static string GridValueKey(QryCheckListGeralModel dado, string columnName)
        {
            return $"{dado.codcompl?.ToString(CultureInfo.InvariantCulture) ?? "novo"}:{columnName}";
        }

        private static object? GetCheckListGeralValue(QryCheckListGeralModel dado, string columnName)
        {
            return columnName switch
            {
                "id" => dado.id,
                "item_memorial" => dado.item_memorial,
                "local_shoppings" => dado.local_shoppings,
                "qtd" => dado.qtd,
                "obs" => dado.obs,
                "orient_montagem" => dado.orient_montagem,
                "orient_desmont" => dado.orient_desmont,
                "carga" => dado.carga,
                _ => null
            };
        }

        private static bool ValoresIguais(object? original, object? atual)
        {
            if (original is null && atual is null)
                return true;

            if (original is null || atual is null)
                return false;

            if (original is double originalDouble && atual is double atualDouble)
                return Math.Abs(originalDouble - atualDouble) < 0.000001;

            return string.Equals(Convert.ToString(original, CultureInfo.InvariantCulture)?.Trim(),
                                 Convert.ToString(atual, CultureInfo.InvariantCulture)?.Trim(),
                                 StringComparison.Ordinal);
        }

        private async void dgCheckListGeral_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            var mensagen = MessageBox.Show("Deseja deletar a linha e seu(s) complemento(s)?","Deletar Check-List",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (mensagen == MessageBoxResult.Yes)
            {
                var item = e.Items.FirstOrDefault() as QryCheckListGeralModel;
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                try
                {
                    if (item?.codcompl is not null)
                    {
                        await vm.DeleteCheckListAsync((long)item.codcompl);
                    }
                }
                catch(Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro ao deletar checklist");
                    e.Cancel = true;
                }
            }
            else 
            { 
                e.Cancel = true;
            }
        }

    }

    public class NameButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "CRIAR";
            else
                return "ABRIR";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
