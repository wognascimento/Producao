using Npgsql;
using Producao.Views.PopUp;
using Producao.Views.CentralModelos.Compat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
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
        List<string> lVoltagem = ["", "220V", "110V"];
        List<string> lLocalShopping = ["", "INTERNO", "EXTERNO"];
        bool dbClick;
        private bool carregandoProdutoPorCodigo;
        static DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public RequisicaoMaterial(object obj)
        {
            InitializeComponent();
            DataContext = new RequisicaoViewModel();

            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            //vm.ProdutoServico = (ProdutoServicoModel)obj;
            vm.TGlobal = (TGlobalModel)obj;
            
            //cbVoltagem.ItemsSource= lVoltagem;
            //cbLocalShopping.ItemsSource= lLocalShopping;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                vm.Planilhas = await vm.GetPlanilhasAsync();
                //vm.Requisicao = await vm.GetRequisicaoAsync(vm.ProdutoServico.num_os_servico);
                vm.Requisicao = await vm.GetRequisicaoAsync(vm.TGlobal.num_os);
                vm.QryRequisicaoDetalhes = await vm.GetRequisicaoDetalhesAsync(vm.Requisicao.num_requisicao);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
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
            txtObservacao.Text = string.Empty;
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
                    observacao = txtObservacao.Text,
                    data = DateTime.Now,
                    alterado_por = Environment.UserName
                };

                var requi = await vm.AddProdutoRequisicaoAsync(vm.RequisicaoDetalhe);
                //await vm.GetRequisicaoDetalhesAsync();

                //vm.Requisicao = await vm.GetRequisicaoAsync(vm.ProdutoServico.num_os_servico);
                vm.QryRequisicaoDetalhes = await vm.GetRequisicaoDetalhesAsync(requi.num_requisicao);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Limpar();

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
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
                vm.RequisicaoDetalhe.observacao = txtObservacao.Text;
                vm.RequisicaoDetalhe.data = DateTime.Now;
                vm.RequisicaoDetalhe.alterado_por = Environment.UserName;

                var requi = await vm.AddProdutoRequisicaoAsync(vm.RequisicaoDetalhe);
                //await vm.GetRequisicaoDetalhesAsync();

                //vm.Requisicao = await vm.GetRequisicaoAsync(vm.ProdutoServico.num_os_servico);
                vm.QryRequisicaoDetalhes = await vm.GetRequisicaoDetalhesAsync(requi.num_requisicao);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Limpar();

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
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

                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(BaseSettings.ResolveModeloPath("REQUISICAO_MODELO.xlsx"));
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.Range["C2"].Number = Convert.ToDouble(requi?.num_requisicao);
                worksheet.Range["E2"].DateTime = Convert.ToDateTime(requi?.data);
                worksheet.Range["C3"].Text = requi?.alterado_por;
                worksheet.Range["G3"].Text = requi?.setor_caminho;
                worksheet.Range["C4"].Text = requi?.cliente;
                worksheet.Range["F4"].Text = requi?.tema;
                worksheet.Range["C5"].Text = requi?.item_memorial;
                worksheet.Range["F5"].Text = requi?.local_shoppings;
                worksheet.Range["C6"].Number = Convert.ToDouble(requi?.num_os_servico);
                worksheet.Range["E6"].Text = requi?.produtocompleto;
                worksheet.Range["N6"].Number = Convert.ToDouble(requi?.coddetalhescompl);

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
                    worksheet.Range[$"M{index}:N{index}"].AdjustRowHeightToText(15, 9.75);
                    worksheet.Range[$"A{index}:N{index}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
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
                workbook.SaveAs(BaseSettings.ResolveImpressosPath($"REQUISICAO_{requi.num_requisicao}.xlsx"));
                Process.Start(new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"REQUISICAO_{requi.num_requisicao}.xlsx"))
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
        /*
        private async void OnPlanilhaSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                if (!dbClick)
                    await vm.GetProdutosAsync();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnDescricaoSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                if (!dbClick)
                    await vm.GetDescAdicionaisAsync();

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnDescricaoAdicionalSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                if (!dbClick)
                    await vm.GetCompleAdicionaisAsync();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void OnComplementoAdicionalSelectedItemChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        */
        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            dbClick = true;
            var record = itens.SelectedItem as QryRequisicaoDetalheModel;
            if (record is null)
            {
                return;
            }
            /*
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            vm.Planilha = (from p in vm.Planilhas where p.planilha == record.planilha select p).FirstOrDefault();
            await vm.GetProdutosAsync();
            vm.Produto = (from p in vm.Produtos where p.codigo == record.codigo select p).FirstOrDefault();
            await vm.GetDescAdicionaisAsync();
            vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == record.coduniadicional select d).FirstOrDefault();
            await vm.GetCompleAdicionaisAsync();
            vm.Compledicional = (from c in vm.CompleAdicionais where c.codcompladicional == record.codcompladicional select c).FirstOrDefault();

            txtQuantidade.Text = record.quantidade.ToString();
            */
            
            
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            //vm.Item = (Item)itens.SelectedItem;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Descricao = await vm.GetDescricaoAsync((long)record?.codcompladicional);
                tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();
                txtPlanilha.Text = vm.Descricao.planilha;
                txtDescricao.Text = vm.Descricao.descricao;
                txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                txtQuantidade.Text = record?.quantidade.ToString();
                txtObservacao.Text = record?.observacao ?? string.Empty;

                vm.RequisicaoDetalhe = new DetalheRequisicaoModel
                {
                    cod_det_req = record.cod_det_req,
                    num_requisicao = record.num_requisicao,
                    codcompladicional = record.codcompladicional,
                    quantidade = (float?)record.quantidade
                };

                vm.Produtos = await vm.GetProdutosAsync(vm.Descricao.planilha);
                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm.Descricao.codigo);
                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm.Descricao.coduniadicional);

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (FormatException ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
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
                    var dado = long.Parse(((TextBox)sender).Text);
                    RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                    await vm.GetDescricaoAsync(dado);

                    dbClick = true;
                    vm.Planilha = (from p in vm.Planilhas where p.planilha == vm.Descricao.planilha select p).FirstOrDefault();
                    await vm.GetProdutosAsync();
                    vm.Produto = (from p in vm.Produtos where p.codigo == vm.Descricao.codigo select p).FirstOrDefault();
                    await vm.GetDescAdicionaisAsync();
                    vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == vm.Descricao.coduniadicional select d).FirstOrDefault();
                    await vm.GetCompleAdicionaisAsync();
                    vm.Compledicional = (from c in vm.CompleAdicionais where c.codcompladicional == vm.Descricao.codcompladicional select c).FirstOrDefault();

                    txtQuantidade.Focus();
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
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
                    await PreencherProdutoPorCodigoAsync(long.Parse(text));

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (FormatException ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void OnOpenDescricoes(object sender, RoutedEventArgs e)
        {
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            var window = new BuscaProduto();
            window.Owner = App.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    if (window.descricao?.codcompladicional is long codigo)
                        await PreencherProdutoPorCodigoAsync(codigo);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void OnSelectedPlanilha(object sender, SelectionChangedEventArgs e)
        {
            if (carregandoProdutoPorCodigo)
                return;

            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                RelplanModel? planilha = e.AddedItems.Count > 0 ? e.AddedItems[0] as RelplanModel : null;
                if (planilha is null)
                {
                    return;
                }

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

                vm.Produtos = await vm.GetProdutosAsync(planilha?.planilha);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnSelectedDescricao(object sender, SelectionChangedEventArgs e)
        {
            if (carregandoProdutoPorCodigo)
                return;

            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                ProdutoModel? produto = e.AddedItems.Count > 0 ? e.AddedItems[0] as ProdutoModel : null;
                if (produto is null)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                txtUnidade.Text = string.Empty;

                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(produto?.codigo);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtDescricaoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnSelectedDescricaoAdicional(object sender, SelectionChangedEventArgs e)
        {
            if (carregandoProdutoPorCodigo)
                return;

            try
            {
                RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
                TabelaDescAdicionalModel? adicional = e.AddedItems.Count > 0 ? e.AddedItems[0] as TabelaDescAdicionalModel : null;
                if (adicional is null)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                txtUnidade.Text = string.Empty;

                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(adicional?.coduniadicional);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void OnSelectedComplementoAdicional(object sender, SelectionChangedEventArgs e)
        {
            if (carregandoProdutoPorCodigo)
                return;

            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            TblComplementoAdicionalModel? complemento = e.AddedItems.Count > 0 ? e.AddedItems[0] as TblComplementoAdicionalModel : null;
            vm.Compledicional = complemento;
            tbCodproduto.Text = complemento?.codcompladicional.ToString();
            txtUnidade.Text = complemento?.unidade;
            txtQuantidade.Focus();
        }

        private async Task PreencherProdutoPorCodigoAsync(long codcompladicional)
        {
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            vm.Descricao = await vm.GetDescricaoAsync(codcompladicional);
            if (vm.Descricao == null)
            {
                MessageBox.Show("Produto não encontrado", "Busca de produto");
                return;
            }

            carregandoProdutoPorCodigo = true;
            try
            {
                tbCodproduto.Text = vm.Descricao.codcompladicional.ToString();

                vm.Planilha = vm.Planilhas?.FirstOrDefault(p => p.planilha == vm.Descricao.planilha);
                txtPlanilha.SelectedItem = vm.Planilha;
                txtPlanilha.Text = vm.Descricao.planilha;

                vm.Produtos = await vm.GetProdutosAsync(vm.Descricao.planilha);
                vm.Produto = vm.Produtos.FirstOrDefault(p => p.codigo == vm.Descricao.codigo);
                txtDescricao.SelectedItem = vm.Produto;
                txtDescricao.Text = vm.Descricao.descricao;

                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm.Descricao.codigo);
                vm.DescAdicional = vm.DescAdicionais.FirstOrDefault(d => d.coduniadicional == vm.Descricao.coduniadicional);
                txtDescricaoAdicional.SelectedItem = vm.DescAdicional;
                txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;

                vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm.Descricao.coduniadicional);
                vm.Compledicional = vm.CompleAdicionais.FirstOrDefault(c => c.codcompladicional == vm.Descricao.codcompladicional);
                txtComplementoAdicional.SelectedItem = vm.Compledicional;
                txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                txtUnidade.Text = vm.Descricao.unidade;
            }
            finally
            {
                carregandoProdutoPorCodigo = false;
            }

            txtQuantidade.Focus();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }

        private async void OnReceitaClick(object sender, RoutedEventArgs e)
        {
            RequisicaoViewModel vm = (RequisicaoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await vm.GravarItensReceitaAsync(vm.Requisicao.num_requisicao);
                vm.QryRequisicaoDetalhes = await vm.GetRequisicaoDetalhesAsync(vm.Requisicao.num_requisicao);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (PostgresException ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro do banco");
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }
}

