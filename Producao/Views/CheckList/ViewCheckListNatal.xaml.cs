using Producao.Views.popup;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewCheckList.xam
    /// </summary>
    public partial class ViewCheckListNatal : UserControl
    {
        private bool dbClick;
        public ViewCheckListNatal()
        {
            DataContext = new CheckListViewModel();
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnSelectionClient(object sender, Syncfusion.UI.Xaml.Grid.SelectionChangedEventArgs e)
        {

        }

        private async void OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //CheckListViewModel vm = (CheckListViewModel)DataContext;
            //vm.CheckListGeralComplemento = new QryCheckListGeralComplementoModel();
            //vm.CheckListGeralComplementos = new ObservableCollection<QryCheckListGeralComplementoModel>();
            //btnAddicionar.Visibility = Visibility.Collapsed;

            try
            {
               
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                dbClick = true;
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
                    return;

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

                vm.Planilha = (from p in vm.Planilhas where p.planilha == record?.planilha select p).FirstOrDefault();
                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(vm?.Planilha?.planilha));
                vm.Produto = (from p in vm.Produtos where p.codigo == record?.codigo select p).FirstOrDefault();
                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(vm?.Produto?.codigo));
                vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == record?.coduniadicional select d).FirstOrDefault();

                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm?.CheckListGeral?.coduniadicional));
                vm.CheckListGeralComplementos = await Task.Run(() => vm.GetCheckListGeralComplementoAsync(vm?.CheckListGeral?.codcompl));

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                //btnAddicionar.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

        }

        private async void OnPlanilhaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CheckListViewModel vm = (CheckListViewModel)DataContext;

                vm.ComplementoCheckList.codproduto = null;
                vm.Produtos = new ObservableCollection<ProdutoModel>();
                cbDescricao.SelectedItem = null;
                cbDescricao.Text = string.Empty;

                vm.ComplementoCheckList.coduniadicional = null;
                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                cbDescricaoAdicional.SelectedItem = null;
                cbDescricaoAdicional.Text = string.Empty;

                //if (!dbClick)
                vm.Produtos = await Task.Run(async () => await vm.GetProdutosAsync(vm.Planilha?.planilha));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnPlanilhaSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;
            if (!dbClick)
                vm.Produtos = await Task.Run(async () => await vm.GetProdutosAsync(vm.Planilha.planilha));

        }

        private async void OnDescricaoSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                CheckListViewModel vm = (CheckListViewModel)DataContext;

                vm.ComplementoCheckList.coduniadicional = null;
                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                cbDescricaoAdicional.SelectedItem = null;

                //if (!dbClick)
                vm.DescAdicionais = await Task.Run(async () => await vm.GetDescAdicionaisAsync(vm.Produto?.codigo));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnDescricaoSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;
            if (!dbClick)
                vm.DescAdicionais = await Task.Run(async () => await vm.GetDescAdicionaisAsync(vm.Produto.codigo));

        }

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                dgCheckListGeral.SelectedItem = null;
                dgComplemento.SelectedItem = null;

                CheckListViewModel vm = (CheckListViewModel)DataContext;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                vm.Locaisshopping = await Task.Run(async () => await vm.GetLocaisShoppAsync(vm?.Sigla?.id_aprovado));
                vm.CheckListGerais = await Task.Run(async () => await vm.GetCheckListGeralAsync(vm?.Sigla?.id_aprovado));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                vm.Locaisshopping = await Task.Run(async () => await vm.GetLocaisShoppAsync(vm?.Sigla?.id_aprovado));
                vm.CheckListGerais = await Task.Run(async () => await vm.GetCheckListGeralAsync(vm?.Sigla?.id_aprovado));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            vm.ComplementoCheckList = new ComplementoCheckListModel();

            tbId.Focus();
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                dbClick = true;
                var visualcontainer = this.dgCheckListGeral.GetVisualContainer();
                var rowColumnIndex = visualcontainer.PointToCellRowColumnIndex(e.GetPosition(visualcontainer));
                var recordindex = this.dgCheckListGeral.ResolveToRecordIndex(rowColumnIndex.RowIndex);
                var recordentry = this.dgCheckListGeral.View.GroupDescriptions.Count == 0 ? this.dgCheckListGeral.View.Records[recordindex] : this.dgCheckListGeral.View.TopLevelGroup.DisplayElements[recordindex];
                var record = ((RecordEntry)recordentry).Data as QryCheckListGeralModel;

                CheckListViewModel vm = (CheckListViewModel)DataContext;

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
                    qtd = vm.CheckListGeral.qtd,
                    coduniadicional = vm.CheckListGeral.coduniadicional,
                    codcompl = vm.CheckListGeral.codcompl,
                    nivel = vm.CheckListGeral.nivel,
                    carga = vm.CheckListGeral.carga,
                    class_solucao = vm.CheckListGeral.class_solucao,
                    id_aprovado = vm.CheckListGeral.id_aprovado,
                    historico = vm.CheckListGeral.historico,
                    agrupar = vm.CheckListGeral.agrupar
                };

                vm.Planilha = (from p in vm.Planilhas where p.planilha == record.planilha select p).FirstOrDefault();
                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(vm.Planilha.planilha));
                vm.Produto = (from p in vm.Produtos where p.codigo == record.codigo select p).FirstOrDefault();
                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(vm.Produto.codigo));
                vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == record.coduniadicional select d).FirstOrDefault();

                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm.CheckListGeral.coduniadicional));
                vm.CheckListGeralComplementos = await Task.Run(() => vm.GetCheckListGeralComplementoAsync(vm.CheckListGeral.codcompl));

                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                //btnAddicionar.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                vm.ComplementoCheckList.inserido_por = Environment.UserName;
                vm.ComplementoCheckList.inserido_em = DateTime.Now;

                ComplementoCheckListModel compl = await vm.AddComplementoCheckListAsync(vm.ComplementoCheckList);

                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                var locais = await Task.Run(() => vm.GetLocaisShoppAsync(vm.Sigla.id_aprovado));
                vm.Locaisshopping = locais;

                vm.CheckListGerais = await Task.Run(() => vm.GetCheckListGeralAsync(vm.Sigla.id_aprovado));

                RowColumnIndex rowColumnIndex = new RowColumnIndex();
                this.dgCheckListGeral.SelectedItems.Clear();
                this.dgCheckListGeral.SearchHelper.Search(compl.codcompl.ToString());
                this.dgCheckListGeral.SearchHelper.FindNext(compl.codcompl.ToString());
                rowColumnIndex.RowIndex = this.dgCheckListGeral.SearchHelper.CurrentRowColumnIndex.RowIndex;
                dgCheckListGeral.ScrollInView(rowColumnIndex);
                dgCheckListGeral.View.Refresh();
                dgCheckListGeral.SelectedIndex = rowColumnIndex.RowIndex-1;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro ao inserir", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            
        }

        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.ComplementoCheckList.alterado_por = Environment.UserName;
                vm.ComplementoCheckList.alterado_em = DateTime.Now;
                ComplementoCheckListModel compl = await vm.AddComplementoCheckListAsync(vm.ComplementoCheckList);

                SiglaChkListModel valor = (SiglaChkListModel)this.cbSiglaShopping.SelectedItem;
                var locais = await Task.Run(() => vm.GetLocaisShoppAsync(vm.Sigla.id_aprovado));
                vm.Locaisshopping = locais;

                vm.CheckListGerais = await Task.Run(() => vm.GetCheckListGeralAsync(vm.Sigla.id_aprovado));

                RowColumnIndex rowColumnIndex = new RowColumnIndex();
                this.dgCheckListGeral.SelectedItems.Clear();
                this.dgCheckListGeral.SearchHelper.Search(compl.codcompl.ToString());
                this.dgCheckListGeral.SearchHelper.FindNext(compl.codcompl.ToString());
                rowColumnIndex.RowIndex = this.dgCheckListGeral.SearchHelper.CurrentRowColumnIndex.RowIndex;
                dgCheckListGeral.ScrollInView(rowColumnIndex);
                dgCheckListGeral.View.Refresh();
                dgCheckListGeral.SelectedIndex = rowColumnIndex.RowIndex-1;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro ao inserir", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnCurrentCellDropDownSelectionChanged(object sender, CurrentCellDropDownSelectionChangedEventArgs e)
        {
            /*
            QryCheckListGeralComplementoModel record;
            var sfdatagrid = sender as SfDataGrid;
            var viewModel = sfdatagrid.DataContext as CheckListViewModel;
            int rowIndex = sfdatagrid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);

            //Console.Out.WriteLine("ADD NEW " + sfdatagrid.IsAddNewIndex(e.RowColumnIndex.RowIndex));

            Debug.WriteLine("something" + sfdatagrid.IsAddNewIndex(e.RowColumnIndex.RowIndex));

            if (rowIndex == -1)
                record = sfdatagrid.View.CurrentAddItem as QryCheckListGeralComplementoModel;
            //record = new();
            else
                record = (sfdatagrid.View.Records[rowIndex] as RecordEntry).Data as QryCheckListGeralComplementoModel;

            sfdatagrid.View.BeginInit();
            record.unidade = ((TblComplementoAdicionalModel)e.SelectedItem).unidade;
            record.saldoestoque = ((TblComplementoAdicionalModel)e.SelectedItem).saldo_estoque;
            sfdatagrid.View.EndInit();
            sfdatagrid.View.Refresh();

            //RowColumnIndex rowColumnIndex = new RowColumnIndex(3, 2);
            //this.dataGrid.MoveCurrentCell(rowColumnIndex);
            //this.dataGrid.SelectionController.CurrentCellManager.BeginEdit();

            //sfdatagrid.SelectionController.CurrentCellManager.BeginEdit();

            //this.dgComplemento.UpdateDataRow(e.RowColumnIndex.RowIndex);

            //record.UnitPrice = viewModel.UnitPriceDict[e.SelectedItem.ToString()];

            //record.Quantity = viewModel.QuantityDict[e.SelectedItem.ToString()];
            */

            var sfdatagrid = sender as SfDataGrid;
            var viewModel = (CheckListViewModel)sfdatagrid.DataContext;
            int rowIndex = sfdatagrid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
            //var record = (sfdatagrid.View.Records[rowIndex] as RecordEntry).Data as QryCheckListGeralComplementoModel;
            QryCheckListGeralComplementoModel record;

            if (rowIndex == -1)
                record = (QryCheckListGeralComplementoModel)sfdatagrid.View.CurrentAddItem;
            //record = new();
            else
                record = (QryCheckListGeralComplementoModel)(sfdatagrid.View.Records[rowIndex] as RecordEntry).Data;

            record.unidade = ((TblComplementoAdicionalModel)e.SelectedItem).unidade; //viewModel.UnitPriceDict[e.SelectedItem.ToString()];
            record.saldoestoque = ((TblComplementoAdicionalModel)e.SelectedItem).saldo_estoque; //viewModel.QuantityDict[e.SelectedItem.ToString()];
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
                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
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

                worksheet.Range["G2"].Text = $"ORIENTAÇÃO DE MONTAGEM";
                worksheet.Range["G2"].ColumnWidth = 30;

                worksheet.Range["H2"].Text = $"COD DETALHES COMPL";
                worksheet.Range["H2"].ColumnWidth = 10;
                worksheet.Range["H2"].WrapText = true;

                worksheet.Rows[1].CellStyle = bodyStyle;

                var dados = await Task.Run(() => vm.GetChkGeralRelatorioAsync(vm.Sigla.id_aprovado));
                worksheet.ImportData(dados, 3, 1, false);

                worksheet.Range[$"A3:H{dados.Count + 2}"].CellStyle = headerStyle;

                worksheet.Range[$"A3:A{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"A3:A{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"B3:B{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"C3:C{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"D3:D{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"E3:E{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"E3:E{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"F3:G{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"F3:G{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"G3:G{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"H3:H{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"H3:H{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.PageSetup.PrintTitleColumns = "$A:$H";
                worksheet.PageSetup.PrintTitleRows = "$1:$2";
                worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                worksheet.PageSetup.LeftMargin = 0.0;
                worksheet.PageSetup.RightMargin = 0.0;
                worksheet.PageSetup.TopMargin = 0.0;
                worksheet.PageSetup.BottomMargin = 0.5;
                worksheet.PageSetup.RightFooter = "&P";
                worksheet.PageSetup.LeftFooter = "&D";
                //worksheet.PageSetup.CenterVertically = true;
                worksheet.PageSetup.CenterHorizontally = true;

                workbook.SaveAs("Impressos/CHECKLIST.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\CHECKLIST.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            

        }

        private void OnDropDownOpened(object sender, EventArgs e)
        {
            dbClick = false;
        }

        private async void dgComplemento_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

            QryCheckListGeralComplementoModel? dado = e.Record as QryCheckListGeralComplementoModel; //e.Record = {Producao.QryCheckListGeralComplementoModel}
            CheckListViewModel vm = (CheckListViewModel)DataContext;

            SfDataGrid? grid = sender as SfDataGrid;
            int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            var column = grid.Columns[columnindex];
            var rowIndex = grid.ResolveToRecordIndex(e.RowColumnIndex.RowIndex);
            //var record = grid.View.Records[rowIndex].Data as QryCheckListGeralComplementoModel;

            try
            {
                if (column.GetType() == typeof(GridCheckBoxColumn) && column.MappingName == "confirmado")
                {
                    vm.DetCompl.coddetalhescompl = dado.coddetalhescompl;
                    vm.DetCompl.confirmado = dado.confirmado;
                    vm.DetCompl.confirmado_data = dado.confirmado == "-1" ? DateTime.Now : dado.confirmado_data;
                    vm.DetCompl.confirmado_por = dado.confirmado == "-1" ? Environment.UserName : dado.confirmado_por;
                    vm.DetCompl.desabilitado_confirmado_data = dado.confirmado == "0" ? DateTime.Now : dado.desabilitado_confirmado_data;
                    vm.DetCompl.desabilitado_confirmado_por = dado.confirmado == "0" ? Environment.UserName : dado.desabilitado_confirmado_por;
                    vm.DetCompl = await Task.Run(() => vm.ConfirmarComplementoCheckListAsync(vm.DetCompl));
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            
        }

        private async void dgComplemento_RowValidated(object sender, RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            CheckListViewModel vm = (CheckListViewModel)DataContext;
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
                    desabilitado_confirmado_data = data.confirmado == "0" ? DateTime.Now : null,
                    desabilitado_confirmado_por = data.confirmado == "0" ? Environment.UserName : null,
                    local_producao = "JACAREÍ",
                    os = data.os
                };

                vm.DetCompl = await Task.Run(() => vm.AddDetalhesComplementoCheckListAsync(vm.DetCompl));
                //QryCheckListGeralComplementoModel record = (QryCheckListGeralComplementoModel)sfdatagrid.View.CurrentAddItem;
                //record.coddetalhescompl = vm.DetCompl.coddetalhescompl;
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

        private void dgComplemento_RowValidating(object sender, RowValidatingEventArgs e)
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

        private void dgComplemento_AddNewRowInitiating(object sender, AddNewRowInitiatingEventArgs e)
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;

            ((QryCheckListGeralComplementoModel)e.NewObject).codcompl = vm.CheckListGeral.codcompl;
        }

        private void chklist_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private async void dgCheckListGeral_RowValidating(object sender, RowValidatingEventArgs e)
        {
            try
            {
                CheckListViewModel vm = (CheckListViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var dado = e.RowData as QryCheckListGeralModel; //e.RowData = {Producao.QryCheckListGeralModel}
                ComplementoCheckListModel CompleChkList = new()
                {
                    codcompl = dado?.codcompl,
                    obs = dado?.obs,
                    orient_montagem = dado?.orient_montagem,
                    orient_desmont = dado?.orient_desmont,
                    ordem = dado?.id,
                    carga = dado?.carga,
                    alterado_por = Environment.UserName,
                    alterado_em = DateTime.Now
                };
                await vm.EditComplementoCheckListAsync(CompleChkList);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex?.InnerException?.Message, "Erro ao inserir", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
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
