using Producao.Views.PopUp;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.OrdemServico.Requisicao
{
    /// <summary>
    /// Lógica interna para RequisicaoMaterial.xaml
    /// </summary>
    public partial class RequisicaoMaterial : Window
    {
        List<string> lVoltagem = new List<string>{"", "220V", "110V" };
        List<string> lLocalShopping = new List<string>{ "", "INTERNO", "EXTERNO" };
        bool dbClick;

        public RequisicaoMaterial(object obj)
        {
            InitializeComponent();
            DataContext = new RequisicaoViewModel();

            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            vm.ProdutoServico = (ProdutoServicoModel)obj;
            
            //cbVoltagem.ItemsSource= lVoltagem;
            //cbLocalShopping.ItemsSource= lLocalShopping;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                vm.Requisicao = await Task.Run(() => vm.GetRequisicaoAsync(vm.ProdutoServico.num_os_servico));
                vm.QryRequisicaoDetalhes = await Task.Run(() => vm.GetRequisicaoDetalhesAsync(vm.Requisicao.num_requisicao));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        void Limpar()
        {
            /*
            tbCodproduto.Text = string.Empty;
            txtPlanilha.SelectedItem = null;
            txtDescricao.SelectedItem = null;
            txtDescricaoAdicional.SelectedItem = null;
            txtComplementoAdicional.SelectedItem = null;
            txtQuantidade.Text = null;
            txtPlanilha.Focus();
            */

            tbCodproduto.Text = string.Empty;
            txtPlanilha.Text = string.Empty;
            txtPlanilha.SelectedItem = null;
            txtDescricao.Text = string.Empty;
            txtDescricao.SelectedItem = null;
            txtDescricaoAdicional.Text = string.Empty;
            txtDescricaoAdicional.SelectedItem = null;
            txtComplementoAdicional.Text = string.Empty;
            txtComplementoAdicional.SelectedItem = null;
            txtQuantidade.Text = string.Empty;
            txtPlanilha.Focus();

        }

        private void OnLimparClick(object sender, RoutedEventArgs e)
        {
            Limpar();
        }

        private async void OnAdicionarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                vm.RequisicaoDetalhe = new DetalheRequisicaoModel
                {
                    cod_det_req = null,
                    num_requisicao = long.Parse(tbNumRequisicao.Text),
                    codcompladicional = long.Parse(tbCodproduto.Text),
                    quantidade = Convert.ToDouble(txtQuantidade.Text),
                    data = DateTime.Now,
                    alterado_por = Environment.UserName
                };

                var requi = await Task.Run(() => vm.AddProdutoRequisicaoAsync(vm.RequisicaoDetalhe));
                //await Task.Run(vm.GetRequisicaoDetalhesAsync);

                //vm.Requisicao = await Task.Run(() => vm.GetRequisicaoAsync(vm.ProdutoServico.num_os_servico));
                vm.QryRequisicaoDetalhes = await Task.Run(() => vm.GetRequisicaoDetalhesAsync(requi.num_requisicao));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Limpar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;

                /*
                vm.RequisicaoDetalhe = new DetalheRequisicaoModel
                {
                    //cod_det_req = vm.RequisicaoDetalhe.cod_det_req,
                    //num_requisicao = long.Parse(tbNumRequisicao.Text),
                    codcompladicional = long.Parse(tbCodproduto.Text),
                    quantidade = Convert.ToDouble(txtQuantidade.Text),
                    data = DateTime.Now,
                    alterado_por = Environment.UserName
                };
                */
                vm.RequisicaoDetalhe.codcompladicional = long.Parse(tbCodproduto.Text);
                vm.RequisicaoDetalhe.quantidade = Convert.ToDouble(txtQuantidade.Text);
                vm.RequisicaoDetalhe.data = DateTime.Now;
                vm.RequisicaoDetalhe.alterado_por = Environment.UserName;

                var requi = await Task.Run(() => vm.AddProdutoRequisicaoAsync(vm.RequisicaoDetalhe));
                //await Task.Run(vm.GetRequisicaoDetalhesAsync);

                //vm.Requisicao = await Task.Run(() => vm.GetRequisicaoAsync(vm.ProdutoServico.num_os_servico));
                vm.QryRequisicaoDetalhes = await Task.Run(() => vm.GetRequisicaoDetalhesAsync(requi.num_requisicao));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Limpar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;

                QryRequisicaoDetalheModel requi = (from r in vm.QryRequisicaoDetalhes select r).FirstOrDefault();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open("Modelos/REQUISICAO_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];
                worksheet.Range["C2"].Text = requi.num_requisicao.ToString();
                worksheet.Range["C3"].Text = requi.alterado_por;
                worksheet.Range["G3"].Text = requi.setor_caminho;
                worksheet.Range["C4"].Text = String.Format("{0:dd/MM/yyyy}", requi.data);  // "03/09/2008" //vm.Requisicao.data.ToString("MM/dd/yyyy");
                worksheet.Range["G4"].Text = requi.cliente;
                worksheet.Range["M4"].Text = requi.coddetalhescompl.ToString();
                worksheet.Range["C5"].Text = requi.item_memorial;
                worksheet.Range["G5"].Text = requi.tema;
                worksheet.Range["C6"].Text = requi.num_os_servico.ToString();
                worksheet.Range["F6"].Text = requi.produtocompleto;

                var itens = (from i in vm.QryRequisicaoDetalhes where i.quantidade > 0  select new { i.quantidade, i.planilha, i.descricao_completa, i.unidade, i.observacao, i.codcompladicional }).ToList(); //new { a.Name, a.Age }
                var index = 9;
                foreach (var item in itens) 
                {
                    worksheet.Range[$"A{index}"].Number = (double)item.quantidade;
                    worksheet.Range[$"A{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$"A{index}"].CellStyle.Font.Size = 7;

                    worksheet.Range[$"B{index}"].Number = (double)item.codcompladicional;
                    worksheet.Range[$"B{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$"B{index}"].CellStyle.Font.Size = 7;

                    worksheet.Range[$"C{index}:D{index}"].Text = item.planilha;
                    worksheet.Range[$"C{index}:D{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"C{index}:D{index}"].CellStyle.Font.Size = 7;
                    worksheet.Range[$"C{index}:D{index}"].Merge();
                    worksheet.Range[$"C{index}:D{index}"].WrapText = true;

                    worksheet.Range[$"E{index}:K{index}"].Text = item.descricao_completa;
                    worksheet.Range[$"E{index}:K{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"E{index}:K{index}"].CellStyle.Font.Size = 7;
                    worksheet.Range[$"E{index}:K{index}"].Merge();
                    worksheet.Range[$"E{index}:K{index}"].WrapText = true;

                    worksheet.Range[$"L{index}"].Text = item.unidade;
                    worksheet.Range[$"L{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$"L{index}"].CellStyle.Font.Size = 7;

                    worksheet.Range[$"M{index}:N{index}"].Text = item.observacao;
                    worksheet.Range[$"M{index}:N{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range[$"M{index}:N{index}"].CellStyle.Font.Size = 7;
                    worksheet.Range[$"M{index}:N{index}"].Merge();
                    worksheet.Range[$"M{index}:N{index}"].WrapText = true;
                    index++;
                    //worksheet.Range["D1:E1"].Merge();
                }

                //ExcelImportDataOptions importDataOptions = new ExcelImportDataOptions();
                //importDataOptions.FirstRow = 9;
                //importDataOptions.IncludeHeader = false;
                //importDataOptions.NestedDataLayoutOptions = ExcelNestedDataLayoutOptions.Merge;
                //importDataOptions.NestedDataLayoutOptions = ExcelNestedDataLayoutOptions.Repeat;

                //Import data from the nested collection.
                //worksheet.ImportData(itens, importDataOptions);
                //worksheet.ImportData(itens, 9, 1, false);

                //Save the Excel document
                workbook.SaveAs($"Impressos/REQUISICAO_{requi.num_requisicao}.xlsx");
                Process.Start(new ProcessStartInfo($"Impressos\\REQUISICAO_{requi.num_requisicao}.xlsx")
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
        /*
        private async void OnPlanilhaSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                if (!dbClick)
                    await Task.Run(async () => await vm.GetProdutosAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnDescricaoSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                if (!dbClick)
                    await Task.Run(async () => await vm.GetDescAdicionaisAsync());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnDescricaoAdicionalSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                if (!dbClick)
                    await Task.Run(async () => await vm.GetCompleAdicionaisAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnComplementoAdicionalSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        */
        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            dbClick = true;
            var visualcontainer = this.itens.GetVisualContainer();
            var rowColumnIndex = visualcontainer.PointToCellRowColumnIndex(e.GetPosition(visualcontainer));
            var recordindex = this.itens.ResolveToRecordIndex(rowColumnIndex.RowIndex);
            var recordentry = this.itens.View.GroupDescriptions.Count == 0 ? this.itens.View.Records[recordindex] : this.itens.View.TopLevelGroup.DisplayElements[recordindex];
            var record = ((RecordEntry)recordentry).Data as QryRequisicaoDetalheModel;
            /*
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            vm.Planilha = (from p in vm.Planilhas where p.planilha == record.planilha select p).FirstOrDefault();
            await Task.Run(async () => await vm.GetProdutosAsync());
            vm.Produto = (from p in vm.Produtos where p.codigo == record.codigo select p).FirstOrDefault();
            await Task.Run(async () => await vm.GetDescAdicionaisAsync());
            vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == record.coduniadicional select d).FirstOrDefault();
            await Task.Run(async () => await vm.GetCompleAdicionaisAsync());
            vm.Compledicional = (from c in vm.CompleAdicionais where c.codcompladicional == record.codcompladicional select c).FirstOrDefault();

            txtQuantidade.Text = record.quantidade.ToString();
            */
            
            
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            //vm.Item = (Item)itens.SelectedItem;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync((long)record?.codcompladicional));
                tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();
                txtPlanilha.Text = vm.Descricao.planilha;
                txtDescricao.Text = vm.Descricao.descricao;
                txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                txtQuantidade.Text = record?.quantidade.ToString();

                vm.RequisicaoDetalhe = new DetalheRequisicaoModel
                {
                    cod_det_req = record.cod_det_req,
                    num_requisicao = record.num_requisicao,
                    codcompladicional = record.codcompladicional,
                    quantidade = (float?)record.quantidade
                };

                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(vm.Descricao.planilha));
                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(vm.Descricao.codigo));
                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm.Descricao.coduniadicional));

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

        }
        /*
        private async void tbCodproduto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    var dado = long.Parse(((SfTextBoxExt)sender).Text);
                    RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                    await Task.Run(async () => await vm.GetDescricaoAsync(dado));

                    dbClick = true;
                    vm.Planilha = (from p in vm.Planilhas where p.planilha == vm.Descricao.planilha select p).FirstOrDefault();
                    await Task.Run(async () => await vm.GetProdutosAsync());
                    vm.Produto = (from p in vm.Produtos where p.codigo == vm.Descricao.codigo select p).FirstOrDefault();
                    await Task.Run(async () => await vm.GetDescAdicionaisAsync());
                    vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == vm.Descricao.coduniadicional select d).FirstOrDefault();
                    await Task.Run(async () => await vm.GetCompleAdicionaisAsync());
                    vm.Compledicional = (from c in vm.CompleAdicionais where c.codcompladicional == vm.Descricao.codcompladicional select c).FirstOrDefault();

                    txtQuantidade.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        */
        private void OnDropDownOpened(object sender, EventArgs e)
        {
            dbClick = false;
        }

        private async void OnBuscaProduto(object sender, KeyEventArgs e)
        {
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;

            if (e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    string text = ((TextBox)sender).Text;
                    vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync(long.Parse(text)));
                    if (vm.Descricao == null)
                    {
                        MessageBox.Show("Produto não encontrado", "Busca de produto");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();
                    txtPlanilha.Text = vm.Descricao.planilha;
                    txtDescricao.Text = vm.Descricao.descricao;
                    txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                    txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                    txtQuantidade.Focus();

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (FormatException ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private void OnOpenDescricoes(object sender, RoutedEventArgs e)
        {
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            var window = new BuscaProduto();
            window.Owner = App.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                vm.Descricao = window.descricao;
                tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();
                txtPlanilha.Text = vm.Descricao.planilha;
                txtDescricao.Text = vm.Descricao.descricao;
                txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                txtUnidade.Text = vm.Descricao.unidade;
                txtQuantidade.Focus();
            }
        }

        private async void OnSelectedPlanilha(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                RelplanModel? planilha = e.NewValue as RelplanModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.Produtos = new ObservableCollection<ProdutoModel>();
                txtDescricao.SelectedItem = null;
                txtDescricao.Text = string.Empty;

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                txtUnidade.Text = string.Empty;

                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(planilha?.planilha));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricao(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                ProdutoModel? produto = e.NewValue as ProdutoModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                txtUnidade.Text = string.Empty;

                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(produto?.codigo));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricaoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricaoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                TabelaDescAdicionalModel? adicional = e.NewValue as TabelaDescAdicionalModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                txtUnidade.Text = string.Empty;

                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(adicional?.coduniadicional));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSelectedComplementoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            TblComplementoAdicionalModel? complemento = e.NewValue as TblComplementoAdicionalModel;
            vm.Compledicional = complemento;
            tbCodproduto.Text = complemento?.codcompladicional.ToString();
            txtUnidade.Text = complemento?.unidade;
            txtQuantidade.Focus();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }
}
