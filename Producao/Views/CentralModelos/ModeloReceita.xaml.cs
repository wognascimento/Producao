using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Producao.Views.PopUp;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Lógica interna para ModeloReceita.xaml
    /// </summary>
    public partial class ModeloReceita : Window
    {
        private QryModeloModel Modelo { get; set; }
        private QryReceitaDetalheCriadoModel Receita { get; set; }

        public ModeloReceita(QryModeloModel Modelo)
        {
            InitializeComponent();
            this.Modelo = Modelo;
            this.DataContext = new ModeloReceitaViewModel();

            if (Modelo?.planilha == "FIADA" || Modelo?.planilha == "ADEREÇO" || Modelo?.planilha == "ENF PISO")
            {
                btnModeloFiada.Visibility = Visibility.Visible;
                GBQtdFiadas.Visibility = Visibility.Visible;
                btnDistribuicaoFiada.Visibility = Visibility.Visible;
            }
            /*else
            {
                btnModeloFiada.Visibility = Visibility.Collapsed;
                GBQtdFiadas.Visibility = Visibility.Collapsed;
            }*/
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                vm.ItensReceita = await Task.Run(() => vm.GetReceitaDetalhes(Modelo.id_modelo));
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                txtPlanilha.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnBuscaProduto(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
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
                    txtCodigoProduto.Text = vm.Descricao.codcompladicional.ToString();
                    txtPlanilha.Text = vm.Descricao.planilha;
                    txtDescricao.Text = vm.Descricao.descricao;
                    txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                    txtComplementoAdicional.Text = vm.Descricao.complementoadicional;

                    vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(vm.Descricao.planilha));
                    vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(vm.Descricao.codigo));
                    vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm.Descricao.coduniadicional));

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    txtObservacao.Focus();
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

        private async void OnOpenDescricoes(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                var window = new BuscaProduto();
                window.Owner = App.Current.MainWindow;
                if (window.ShowDialog() == true)
                {
                    vm.Descricao = window.descricao;

                    txtCodigoProduto.Text = vm.Descricao.codcompladicional.ToString();
                    txtPlanilha.Text = vm.Descricao.planilha;
                    txtDescricao.Text = vm.Descricao.descricao;
                    txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                    txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                    txtObservacao.Focus();

                    vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(vm.Descricao.planilha));
                    vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(vm.Descricao.codigo));
                    vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm.Descricao.coduniadicional));
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
           
        }

        private async void OnSelectedPlanilha(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                RelplanModel? planilha = e.NewValue as RelplanModel;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(planilha?.planilha));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricao(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                ProdutoModel? produto = e.NewValue as ProdutoModel;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(produto?.codigo));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                txtDescricaoAdicional.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedDescricaoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                TabelaDescAdicionalModel? adicional = e.NewValue as TabelaDescAdicionalModel;
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(adicional?.coduniadicional));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSelectedComplementoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
            TblComplementoAdicionalModel? complemento = e.NewValue as TblComplementoAdicionalModel;
            vm.Compledicional = complemento;
            txtCodigoProduto.Text = complemento?.codcompladicional.ToString();
            txtObservacao.Focus();
        }

        private void OnLimparClick(object sender, RoutedEventArgs e)
        {
            Limpar();
        }

        private void Limpar()
        {
            txtCodigoProduto.Text = string.Empty;
            txtPlanilha.Text = string.Empty;
            txtDescricao.Text = string.Empty;
            txtDescricaoAdicional.Text = string.Empty;
            txtComplementoAdicional.Text = string.Empty;
            txtObservacao.Text = string.Empty;
            txtQtdModelo.Value = double.NaN;
            txtQtdProducao.Value = double.NaN;
            mod01.Value = null;
            mod02.Value = null;
            mod03.Value = null;
            mod04.Value = null;
            mod05.Value = null;
            mod06.Value = null;
            mod07.Value = null;
            mod08.Value = null;
            mod09.Value = null;
            mod10.Value = null;
            //Receita = new QryReceitaDetalheCriadoModel();
            //txtPlanilha.Focus();
        }

        private async void OnAdicionarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;

                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                var dados = new ModeloReceitaModel
                {
                    id_modelo = Modelo.id_modelo,
                    codcompladicional = long.Parse(txtCodigoProduto.Text),
                    qtd_modelo = txtQtdModelo.Value,
                    qtd_producao = txtQtdProducao.Value,
                    observacao = txtObservacao.Text,
                    cadastrado_por = Environment.UserName,
                    data_cadastro = DateTime.Now,
                    mod1 = (int?)mod01.Value,
                    mod2 = (int?)mod02.Value,
                    mod3 = (int?)mod03.Value,
                    mod4 = (int?)mod04.Value,
                    mod5 = (int?)mod05.Value,
                    mod6 = (int?)mod06.Value,
                    mod7 = (int?)mod07.Value,
                    mod8 = (int?)mod08.Value,
                    mod9 = (int?)mod09.Value,
                    mod10 = (int?)mod10.Value,
                };
                vm.ModeloReceita = await Task.Run(() => vm.AddReceita(dados));

                //if (Receita == null)
                vm.ItensReceita = await Task.Run(() => vm.GetReceitaDetalhes(Modelo.id_modelo));

                Limpar();
                
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

        private async void OnAlterarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;

                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                var dados = new ModeloReceitaModel
                {
                    id_linha = Receita?.id_linha,
                    id_modelo = Modelo.id_modelo,
                    codcompladicional = long.Parse(txtCodigoProduto.Text),
                    qtd_modelo = txtQtdModelo.Value,
                    qtd_producao = txtQtdProducao.Value,
                    observacao = txtObservacao.Text,
                    cadastrado_por = Environment.UserName,
                    data_cadastro = DateTime.Now,
                    mod1 = (int?)mod01.Value,
                    mod2 = (int?)mod02.Value,
                    mod3 = (int?)mod03.Value,
                    mod4 = (int?)mod04.Value,
                    mod5 = (int?)mod05.Value,
                    mod6 = (int?)mod06.Value,
                    mod7 = (int?)mod07.Value,
                    mod8 = (int?)mod08.Value,
                    mod9 = (int?)mod09.Value,
                    mod10 = (int?)mod10.Value,
                };
                vm.ModeloReceita = await Task.Run(() => vm.AddReceita(dados));

                //if (Receita == null)
                vm.ItensReceita = await Task.Run(() => vm.GetReceitaDetalhes(Modelo.id_modelo));

                Limpar();

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

        private async void OnExcluirClick(object sender, RoutedEventArgs e)
        {
            if (Receita == null)
            {
                MessageBox.Show("Precisa selecionar uma linha para excluir", "Deletar item", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show("Deseja deletar o item selecionado?", "Deletar item", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.No)
            { return; }

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;

                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                var dados = new ModeloReceitaModel
                {
                    id_linha = Receita?.id_linha,
                    id_modelo = Modelo.id_modelo,
                    codcompladicional = long.Parse(txtCodigoProduto.Text),
                    qtd_modelo = txtQtdModelo.Value,
                    qtd_producao = txtQtdProducao.Value,
                    observacao = txtObservacao.Text,
                    cadastrado_por = Environment.UserName,
                    data_cadastro = DateTime.Now,
                };
                await Task.Run(() => vm.ExcluirAsync(dados));
                vm.ItensReceita = await Task.Run(() => vm.GetReceitaDetalhes(Modelo.id_modelo));
                Limpar();
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

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open("Modelos/RECEITA_CENTRAL_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];
                worksheet.Range["C2"].Text = Modelo.id_modelo.ToString();
                worksheet.Range["C3"].Text = Modelo.planilha;
                worksheet.Range["C4"].Text = Modelo.descricao_completa;
                worksheet.Range["C5"].Text = Modelo.tema;
                worksheet.Range["A7"].Text = Modelo.obs_modelo;
                worksheet.Range["H4"].Number = Convert.ToDouble( Modelo.multiplica );//String.Format(new CultureInfo("pt-BR"), "{0:G}", Modelo.multiplica); //Modelo.multiplica.ToString();
                worksheet.Range["H4"].NumberFormat = "0.0";
                //Console.WriteLine(String.Format(new CultureInfo("pt-BR"), "{0:C}", 189.99));

                //var itens = (from i in vm.ItensReceita where i.quantidade > 0 select new { i.quantidade, i.planilha, i.descricao_completa, i.unidade }).ToList(); //new { a.Name, a.Age }

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
                //bodyStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                //bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                //bodyStyle.Font.Bold = true;
                //bodyStyle.WrapText = true;
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
                headerStyle.Font.Bold = true;
                //headerStyle.WrapText = true;
                headerStyle.EndUpdate();

                var index = 9;

                foreach (var item in vm.ItensReceita)
                {
                    worksheet.Range[$"A{index}"].Number = (double)item.codcompladicional;
                    //worksheet.Range[$"A{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"A{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"A{index}"].CellStyle = bodyStyle;

                    worksheet.Range[$"B{index}"].Text = item.planilha;
                    //worksheet.Range[$"B{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"B{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"B{index}"].CellStyle = bodyStyle;

                    worksheet.Range[$"C{index}"].Text = item.descricao_completa;
                    //worksheet.Range[$"C{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"C{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"C{index}:E{index}"].CellStyle = bodyStyle;
                    worksheet.Range[$"C{index}:E{index}"].Merge();
                    worksheet.Range[$"C{index}:E{index}"].WrapText = true;

                    worksheet.Range[$"F{index}"].Text = item.observacao;
                    //worksheet.Range[$"F{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"F{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"F{index}"].CellStyle = bodyStyle;
                    //worksheet.Range[$"F{index}:G{index}"].Merge();
                    worksheet.Range[$"F{index}"].WrapText = true;

                    worksheet.Range[$"G{index}"].Number = (double)item.qtd_modelo;
                    //worksheet.Range[$"G{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"G{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"G{index}"].CellStyle = bodyStyle;
                    worksheet.Range[$"G{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    worksheet.Range[$"H{index}"].Number = (double)item.qtd_producao;
                    //worksheet.Range[$"H{index}"].CellStyle.Font.FontName = "Arial";
                    //worksheet.Range[$"H{index}"].CellStyle.Font.Size = 8;
                    worksheet.Range[$"H{index}"].CellStyle = bodyStyle;
                    worksheet.Range[$"H{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    index++;
                }

                workbook.SaveAs($"Impressos/RECEITA_CENTRAL_MODELO_{Modelo.id_modelo}.xlsx");
                workbook.Close();

                Process.Start(new ProcessStartInfo($"Impressos\\RECEITA_CENTRAL_MODELO_{Modelo.id_modelo}.xlsx")
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnModelosFiadaClick(object sender, RoutedEventArgs e)
        {
            var window = new ModeloFiada(Modelo);
            window.Owner = App.Current.MainWindow;
            window.ShowDialog();
        }

        private async void OnCopyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
                var window = new ModeloReceitaCopiar(Modelo);
                window.Owner = App.Current.MainWindow;
                if (window.ShowDialog() == true)
                {

                    var dados = window.itens;
                    if (vm.ItensReceita.Count > 0)
                    {
                        var result = MessageBox.Show("Existem itens no Modelo, desejá copiar assim mesmo?", "Copia de receita", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                            //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                            await Task.Run(() => vm.AddCopiaReceita(dados));
                        }
                        else
                            MessageBox.Show("COPIA DE ITENS CANCELADA!!!");
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                        await Task.Run(() => vm.AddCopiaReceita(dados));
                    }
                }

                vm.ItensReceita = await Task.Run(() => vm.GetReceitaDetalhes(Modelo.id_modelo));
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void dgModelos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Receita = (QryReceitaDetalheCriadoModel)dgModelos.SelectedItem;
            ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync((long)Receita?.codcompladicional));
                txtCodigoProduto.Text = vm.Descricao.codcompladicional.ToString();
                txtPlanilha.Text = vm.Descricao.planilha;
                txtDescricao.Text = vm.Descricao.descricao;
                txtDescricaoAdicional.Text = vm.Descricao.descricao_adicional;
                txtComplementoAdicional.Text = vm.Descricao.complementoadicional;
                txtObservacao.Text = Receita?.observacao;
                txtQtdModelo.Value = Receita?.qtd_modelo;
                txtQtdProducao.Value = Receita?.qtd_producao;

                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(vm.Descricao.planilha));
                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(vm.Descricao.codigo));
                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(vm.Descricao.coduniadicional));

                mod01.Value = Receita?.mod1;
                mod02.Value = Receita?.mod2;
                mod03.Value = Receita?.mod3;
                mod04.Value = Receita?.mod4;
                mod05.Value = Receita?.mod5;
                mod06.Value = Receita?.mod6;
                mod07.Value = Receita?.mod7;
                mod08.Value = Receita?.mod8;
                mod09.Value = Receita?.mod9;
                mod10.Value = Receita?.mod10;

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

        private async void OnDistribuicaoFiadaClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                ModeloReceitaViewModel? vm = (ModeloReceitaViewModel)DataContext;

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
                bodyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Dashed;
                bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Dashed;
                bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Dashed;
                bodyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Dashed;
                bodyStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                bodyStyle.WrapText = true;
                bodyStyle.EndUpdate();

                headerStyle = workbook.Styles.Add("headerStyle");
                headerStyle.BeginUpdate();
                headerStyle.Color = Color.FromArgb(239, 243, 247);
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Dashed;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Dashed;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Dashed;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Dashed;
                headerStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                headerStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                headerStyle.Font.Bold = true;
                headerStyle.WrapText = true;
                headerStyle.EndUpdate();

                var info = await Task.Run(() => vm.GetInfClienteAsync(Modelo.id_modelo));
                
                worksheet.Range["A1:B1"].Text = "SHOPPING";
                worksheet.Range["A1:B1"].Merge();
                worksheet.Range["A1:B1"].CellStyle = headerStyle;

                worksheet.Range["C1:E1"].Text = info.sigla_serv;
                worksheet.Range["C1:E1"].Merge();
                worksheet.Range["C1:E1"].CellStyle = bodyStyle;

                worksheet.Range["F1"].Text = "NÍVEL";
                worksheet.Range["F1"].Merge();
                worksheet.Range["F1"].CellStyle = headerStyle;

                worksheet.Range["G1"].Text = info.nivel.ToString();
                worksheet.Range["G1"].Merge();
                worksheet.Range["G1"].CellStyle = bodyStyle;

                worksheet.Range["H1"].Text = "MODELO";
                worksheet.Range["H1"].Merge();
                worksheet.Range["H1"].CellStyle = headerStyle;

                worksheet.Range["I1"].Text = info.id_modelo.ToString();
                worksheet.Range["I1"].Merge();
                worksheet.Range["I1"].CellStyle = bodyStyle;

                worksheet.Range["K1:O1"].Text = "MATERIAIS";
                worksheet.Range["K1:O1"].Merge();
                worksheet.Range["K1:O1"].CellStyle = headerStyle;

                worksheet.Range["P1"].Text = "QTDE";
                worksheet.Range["P1"].Merge();
                worksheet.Range["P1"].CellStyle = headerStyle;

                var produtos = await Task.Run(() => vm.GetDetalhesModeloAsync(Modelo.id_modelo));
                for (int i = 0; i < produtos.Count; i++)
                {
                    var produto = produtos[i];
                    worksheet.Range[$"K{(i+2)}:O{(i+2)}"].Text = produto.descricao_completa;
                    worksheet.Range[$"K{(i+2)}:O{(i+2)}"].Merge();
                    worksheet.Range[$"K{(i+2)}:O{(i+2)}"].CellStyle = bodyStyle;

                    worksheet.Range[$"P{(i+2)}"].Text = produto.qtd.ToString();
                    worksheet.Range[$"P{(i+2)}"].Merge();
                    worksheet.Range[$"P{(i+2)}"].CellStyle = bodyStyle;
                }

                worksheet.Range["A2:B2"].Text = "TEMA";
                worksheet.Range["A2:B2"].Merge();
                worksheet.Range["A2:B2"].CellStyle = headerStyle;

                worksheet.Range["C2:I2"].Text = info.tema;
                worksheet.Range["C2:I2"].Merge();
                worksheet.Range["C2:I2"].CellStyle = bodyStyle;


                worksheet.Range["A3:B4"].Text = "LOCAL";
                worksheet.Range["A3:B4"].Merge();
                worksheet.Range["A3:B4"].CellStyle = headerStyle;

                worksheet.Range["C3:F4"].Text = info.local_shoppings;
                worksheet.Range["C3:F4"].Merge();
                worksheet.Range["C3:F4"].CellStyle = bodyStyle;

                worksheet.Range["G3"].Text = "QTD";
                worksheet.Range["G3"].Merge();
                worksheet.Range["G3"].CellStyle = headerStyle;

                worksheet.Range["G4"].Text = info.qtd.ToString();
                worksheet.Range["G4"].Merge();
                worksheet.Range["G4"].CellStyle = bodyStyle;

                worksheet.Range["H3:I3"].Text = "CÓD.DET.COMPL";
                worksheet.Range["H3:I3"].Merge();
                worksheet.Range["H3:I3"].CellStyle = headerStyle;

                worksheet.Range["H4:I4"].Text = info.coddetalhescompl.ToString();
                worksheet.Range["H4:I4"].Merge();
                worksheet.Range["H4:I4"].CellStyle = bodyStyle;

                var modelos = await Task.Run(() => vm.GetModelosFiadaAsync(Modelo.id_modelo));
                var index = 5;
                for (int i = 0; i < modelos.Count; i++)
                {
                    var modelo = modelos[i];
                    worksheet.Range[$"A{index}"].Text = modelo.modelofiada;
                    worksheet.Range[$"A{index}"].Merge();
                    worksheet.Range[$"A{index}"].CellStyle = headerStyle;

                    worksheet.Range[$"B{index}"].Text = modelo.qtdmodelofiada.ToString();
                    worksheet.Range[$"B{index}"].Merge();
                    worksheet.Range[$"B{index}"].CellStyle = bodyStyle;

                    worksheet.Range[$"C{index}:I{index}"].Text = "INFORME A DISTÂNCIA ENTRE OS ENFEITES.";
                    worksheet.Range[$"C{index}:I{index}"].Merge();
                    worksheet.Range[$"C{index}:I{index}"].CellStyle = bodyStyle;

                    index++;
                }

                worksheet.Range[$"A{index}"].Text = "Nº O.S.";
                worksheet.Range[$"A{index}"].Merge();
                worksheet.Range[$"A{index}"].CellStyle = headerStyle;

                worksheet.Range[$"B{index}"].Text = info.num_os_servico.ToString();
                worksheet.Range[$"B{index}"].Merge();
                worksheet.Range[$"B{index}"].CellStyle = bodyStyle;

                worksheet.Range[$"C{index}"].Text = "Nº REQ.";
                worksheet.Range[$"C{index}"].Merge();
                worksheet.Range[$"C{index}"].CellStyle = headerStyle;

                var mumLetra = 68;
                var requisicoes = await Task.Run(() => vm.GetRequisicoesAsync(info.num_os_servico));
                for (int i = 0; i < requisicoes.Count; i++)
                {
                    char letra = (char)mumLetra;
                    var requisicao = requisicoes[i];
                    worksheet.Range[$"{letra}{index}"].Text = requisicao.num_requisicao.ToString();
                    worksheet.Range[$"{letra}{index}"].Merge();
                    worksheet.Range[$"{letra}{index}"].CellStyle = bodyStyle;
                    mumLetra++;
                }
                index++;

                worksheet.Range[$"A{index}:I{index+7}"].Text = "INSIRA AQUI AS OBSERVAÇÕES.";
                worksheet.Range[$"A{index}:I{index+7}"].Merge();
                worksheet.Range[$"A{index}:I{index+7}"].CellStyle = bodyStyle;

                worksheet.Range[$"A{index+8}:I{index+8}"].Text = info.descricao_adicional;
                worksheet.Range[$"A{index+8}:I{index+8}"].Merge();
                worksheet.Range[$"A{index+8}:I{index+8}"].CellStyle = headerStyle;



                /*
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
                */



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

                workbook.SaveAs("Impressos/DISTRIBUICAO_FIADA.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\DISTRIBUICAO_FIADA.xlsx")
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

    }

    public class ModeloReceitaViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private QryReceitaDetalheCriadoModel _itemReceita;
        public QryReceitaDetalheCriadoModel ItemReceita
        {
            get { return _itemReceita; }
            set { _itemReceita = value; RaisePropertyChanged("ItemReceita"); }
        }
        private ObservableCollection<QryReceitaDetalheCriadoModel> _itensReceita;
        public ObservableCollection<QryReceitaDetalheCriadoModel> ItensReceita
        {
            get { return _itensReceita; }
            set { _itensReceita = value; RaisePropertyChanged("ItensReceita"); }
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

        private ObservableCollection<ProdutoModel> _produtos;
        public ObservableCollection<ProdutoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }
        private ProdutoModel _produto;
        public ProdutoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }
        private ObservableCollection<TabelaDescAdicionalModel> _descAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
        {
            get { return _descAdicionais; }
            set { _descAdicionais = value; RaisePropertyChanged("DescAdicionais"); }
        }
        private TabelaDescAdicionalModel _descAdicional;
        public TabelaDescAdicionalModel DescAdicional
        {
            get { return _descAdicional; }
            set { _descAdicional = value; RaisePropertyChanged("DescAdicional"); }
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

        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }

        private ModeloReceitaModel _modeloReceita;
        public ModeloReceitaModel ModeloReceita
        {
            get { return _modeloReceita; }
            set { _modeloReceita = value; RaisePropertyChanged("ModeloReceita"); }
        }

        public async Task<ObservableCollection<QryReceitaDetalheCriadoModel>> GetReceitaDetalhes(long? idmodelo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.qryReceitas.Where(c => c.id_modelo == idmodelo).ToListAsync();
                return new ObservableCollection<QryReceitaDetalheCriadoModel>(data);
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
                var data = await db.Relplans
                    .OrderBy(c => c.planilha)
                    .Where(c => c.ativo.Equals("1"))
                    .ToListAsync();
                return new ObservableCollection<RelplanModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProdutoModel>> GetProdutosAsync(string? planilha)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Produtos
                    .OrderBy(c => c.descricao)
                    .Where(c => c.planilha.Equals(planilha))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                return new ObservableCollection<ProdutoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TabelaDescAdicionalModel>> GetDescAdicionaisAsync(long? codigo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.DescAdicionais
                    .OrderBy(c => c.descricao_adicional)
                    .Where(c => c.codigoproduto.Equals(codigo))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                return new ObservableCollection<TabelaDescAdicionalModel>(data);
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

        public async Task<QryDescricao> GetDescricaoAsync(long codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Descricoes.Where(c => c.codcompladicional == codcompladicional).FirstOrDefaultAsync();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ModeloReceitaModel> AddReceita(ModeloReceitaModel receita)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ReceitaModelos.SingleMergeAsync(receita);
                await db.SaveChangesAsync();
                return receita;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ExcluirAsync(ModeloReceitaModel receita)
        {
            try
            {
                
                using DatabaseContext db = new();
                db.ReceitaModelos.Attach(receita);
                db.ReceitaModelos.Remove(receita);
                await db.SaveChangesAsync();
                /*
                db.ReceitaModelos.Remove(receita);
                await db.SaveChangesAsync();
                */
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddCopiaReceita(ObservableCollection<ModeloReceitaModel> receita)
        {
            using DatabaseContext db = new();
            var strategy = db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () => 
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    foreach (var item in receita)
                    {
                        await db.ReceitaModelos.SingleMergeAsync(item);
                        await db.SaveChangesAsync();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            });
        }

        public async Task<ExportEnfeitesInfClienteModel> GetInfClienteAsync(long? idmodelo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.exportEnfeitesInfClientes
                    .Where(c => c.id_modelo.Equals(idmodelo))
                    .FirstOrDefaultAsync();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<DetalhesModeloModel>> GetDetalhesModeloAsync(long? idmodelo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.DetalhesModelo
                    .Where(c => c.id_modelo.Equals(idmodelo))
                    .ToListAsync();
                return new ObservableCollection<DetalhesModeloModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<ObservableCollection<ModeloFiadaModel>> GetModelosFiadaAsync(long? idmodelo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ModelosFiada
                    .Where(c => c.id_modelo.Equals(idmodelo))
                    .ToListAsync();
                return new ObservableCollection<ModeloFiadaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
         
        public async Task<ObservableCollection<RequisicaoModel>> GetRequisicoesAsync(long? num_os_servico)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Requisicoes
                    .Where(c => c.num_os_servico.Equals(num_os_servico))
                    .ToListAsync();
                return new ObservableCollection<RequisicaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
