using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using Producao.Views.CheckList;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Producao.Views.Construcao
{
    /// <summary>
    /// Interação lógica para EtiquetaConstrucao.xam
    /// </summary>
    public partial class EtiquetaConstrucao : UserControl
    {
        enum Etiqueta
        {
            Primeira,
            Segunda,
            Terceira,
            Quarta
        }

        public EtiquetaConstrucao()
        {
            InitializeComponent();
            DataContext = new EtiquetaConstrucaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                vm.Tamanhos = new ObservableCollection<string>() { "P", "M", "G", "GG" };
                vm.Dificuldades = new ObservableCollection<string>() { "FÁCIL", "MÉDIO", "DIFÍCIL", "MUITO DIFÍCIL" };
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
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
                EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
                PlanilhaConstrucaoModel? planilha = e.NewValue as PlanilhaConstrucaoModel;
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

                //txtUnidade.Text = string.Empty;

                vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(planilha?.planilha));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                clearMedidas();
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
                EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
                ProdutoModel? produto = e.NewValue as ProdutoModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                txtDescricaoAdicional.SelectedItem = null;
                txtDescricaoAdicional.Text = string.Empty;

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                //txtUnidade.Text = string.Empty;

                vm.DescAdicionais = await Task.Run(() => vm.GetDescAdicionaisAsync(produto?.codigo));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                clearMedidas();
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
                EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
                TabelaDescAdicionalModel? adicional = e.NewValue as TabelaDescAdicionalModel;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                txtComplementoAdicional.SelectedItem = null;
                txtComplementoAdicional.Text = string.Empty;

                //txtUnidade.Text = string.Empty;

                vm.CompleAdicionais = await Task.Run(() => vm.GetCompleAdicionaisAsync(adicional?.coduniadicional));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                clearMedidas();
                txtComplementoAdicional.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSelectedComplementoAdicional(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
                TblComplementoAdicionalModel? complemento = e.NewValue as TblComplementoAdicionalModel;
                vm.Compledicional = complemento;



                txt_largura.Text = String.Format("{0:F}", complemento?.largura);//complemento.largura.ToString();
                txt_area.Text = String.Format("{0:F}", complemento?.area);
                txt_profundidade.Text = String.Format("{0:F}", complemento?.profundidade);
                txt_volume.Text = String.Format("{0:F}", complemento?.volume);
                txt_altura.Text = String.Format("{0:F}", complemento?.altura);
                txt_peso.Text = String.Format("{0:F}", complemento?.peso);
                txt_tamanho.SelectedItem = complemento?.tamanho_construcao;
                txt_dificuldade.SelectedItem = complemento?.dificuldade;
                

                vm.Pecas = await Task.Run(() => vm.GetPecasAsync(complemento?.codcompladicional));
                if (vm.Pecas.Count == 0)
                {
                    vm.Detalhes = await Task.Run(() => vm.GetDetalhesAsync(complemento?.codcompladicional));
                    foreach (var det in vm.Detalhes)
                    {
                        await Task.Run(() => vm.SaveConstrucaoDetalheAsync(
                            new ConstrucaoPecaModel
                            {
                                ano = DateTime.Now.Year,
                                codcompladicional = det.codcompladicional,
                                item = det.item,
                                descricao_peca = det.descricao_peca,
                                volume_etiqueta = det.volume,
                            }));
                    }
                    vm.Pecas = await Task.Run(() => vm.GetPecasAsync(complemento?.codcompladicional));
                }

                vm.Historicos = await Task.Run(() => vm.GetHistoricoConstrucaoAsync(complemento?.codcompladicional));


                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
            
        }

        private void itens_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
            ((ConstrucaoPecaModel)e.NewObject).codcompladicional = vm.Compledicional?.codcompladicional;
        }

        private void itens_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            ConstrucaoPecaModel rowData = (ConstrucaoPecaModel)e.RowData;
            if (!rowData.codcompladicional.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("item", "Não é possível adicionar peça na construção.");
                e.ErrorMessages.Add("descricao_peca", "Não é possível adicionar peça na construção.");
                e.ErrorMessages.Add("volume", "Não é possível adicionar peça na construção.");
            }
            else if (!rowData.item.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("item", "Informe a sequência ordinal da Peça.");
            }
            else if (rowData.descricao_peca == "")
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descricao_peca", "Informe a descrição da Peça.");
            }
            else if (!rowData.volume_etiqueta.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("volume", "Informe o agrupamento dos itens");
            }
        }

        private async void itens_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ConstrucaoPecaModel data = (ConstrucaoPecaModel)e.RowData;

                vm.Peca = await Task.Run(() => vm.SaveConstrucaoDetalheAsync(data));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.Pecas.Where(x => x.id_detalhes == null).ToList();
                foreach (var item in toRemove)
                    vm.Pecas.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnEtiquetaClick(object sender, RoutedEventArgs e)
        {
            EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;

            if (vm.Compledicional == null)
            {
                MessageBox.Show("Produto não selecionado!");
                return;
            }

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            //var filteredResult = grid.View.Records.Select(recordentry => recordentry.Data);
            var volumes = vm.Pecas.OrderBy(o => o.volume_etiqueta).GroupBy(user => user.volume_etiqueta).ToList();
            var count = volumes.Count;

            using ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;
            IWorkbook workbook = application.Workbooks.Open("Modelos/ETIQUETA_REQUISICAO_MODELO.xlsx");
            IWorksheet worksheet = workbook.Worksheets[0];

            var etiqueta = Enum.Parse(typeof(Etiqueta), "Primeira");
            int paginas = (int)Math.Ceiling(Decimal.Divide(count, 4));
            int _etiqueta = 1;
            int _pagina = 1;

            vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync(vm.Compledicional.codcompladicional));
            vm.ChecklistPrduto = await Task.Run(() => vm.GetChecklistPrdutoAsync(vm.Compledicional.codcompladicional));

            //foreach (EtiquetaEmitidaModel item in filteredResult)
            for (int i = 0; i < count; i++)
            {
                try
                {
                    long volume = (long)volumes[i].Key;
                    switch (etiqueta)
                    {
                        case Etiqueta.Primeira:
                            {
                                worksheet.Range["A1"].Text = vm.ChecklistPrduto.sigla;
                                worksheet.Range["B2"].Text = $"{volume} / {count}"; //item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                worksheet.Range["C2"].Number = DateTime.Now.Year;
                                worksheet.Range["D2"].Number = (double)vm.ChecklistPrduto.coddetalhescompl;
                                worksheet.Range["B5"].Text = vm.ChecklistPrduto.local_shoppings;
                                worksheet.Range["A8"].Text = vm.Descricao.descricao_completa.Replace("ÚNICO", null);

                                var inx = 12;
                                worksheet.Range["A12"].Text = "";
                                worksheet.Range["A13"].Text = "";
                                worksheet.Range["A14"].Text = "";
                                worksheet.Range["A15"].Text = "";
                                worksheet.Range["A16"].Text = "";
                                foreach (var item in GetProdutosEtiqueta(volume, 5))
                                {
                                    worksheet.Range[$"A{inx}"].Text = item.descricao_peca;
                                    inx++;
                                }

                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["PRIMEIRA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                etiqueta = Enum.Parse(typeof(Etiqueta), "Segunda");
                                _etiqueta++;
                                workbook.SaveAs($"Impressos/ETIQUETA_REQUISICAO_MODELO_{_pagina}.xlsx");
                                break;
                            }
                        case Etiqueta.Segunda:
                            {
                                worksheet.Range["F1"].Text = vm.ChecklistPrduto.sigla;
                                worksheet.Range["G2"].Text = $"{volume} / {count}"; //item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                worksheet.Range["H2"].Number = DateTime.Now.Year;
                                worksheet.Range["I2"].Number = (double)vm.ChecklistPrduto.coddetalhescompl;
                                worksheet.Range["G5"].Text = vm.ChecklistPrduto.local_shoppings;
                                worksheet.Range["F8"].Text = vm.Descricao.descricao_completa.Replace("ÚNICO", null);

                                var inx = 12;
                                worksheet.Range["F12"].Text = "";
                                worksheet.Range["F13"].Text = "";
                                worksheet.Range["F14"].Text = "";
                                worksheet.Range["F15"].Text = "";
                                worksheet.Range["F16"].Text = "";
                                foreach (var item in GetProdutosEtiqueta(volume, 5))
                                {
                                    worksheet.Range[$"F{inx}"].Text = item.descricao_peca;
                                    inx++;
                                }

                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["SEGUNDA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                etiqueta = Enum.Parse(typeof(Etiqueta), "Terceira");
                                _etiqueta++;
                                workbook.SaveAs($"Impressos/ETIQUETA_REQUISICAO_MODELO_{_pagina}.xlsx");
                                break;
                            }
                        case Etiqueta.Terceira:
                            {
                                worksheet.Range["A18"].Text = vm.ChecklistPrduto.sigla;
                                worksheet.Range["B19"].Text = $"{volume} / {count}"; //item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                worksheet.Range["C19"].Number = DateTime.Now.Year;
                                worksheet.Range["D19"].Number = (double)vm.ChecklistPrduto.coddetalhescompl;
                                worksheet.Range["B22"].Text = vm.ChecklistPrduto.local_shoppings;
                                worksheet.Range["A25"].Text = vm.Descricao.descricao_completa.Replace("ÚNICO", null);

                                var inx = 29;
                                worksheet.Range["A29"].Text = "";
                                worksheet.Range["A30"].Text = "";
                                worksheet.Range["A31"].Text = "";
                                worksheet.Range["A32"].Text = "";
                                worksheet.Range["A33"].Text = "";
                                foreach (var item in GetProdutosEtiqueta(volume, 5))
                                {
                                    worksheet.Range[$"A{inx}"].Text = item.descricao_peca;
                                    inx++;
                                }

                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["TERCEIRA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                etiqueta = Enum.Parse(typeof(Etiqueta), "Quarta");
                                _etiqueta++;
                                workbook.SaveAs($"Impressos/ETIQUETA_REQUISICAO_MODELO_{_pagina}.xlsx");
                                break;
                            }
                        case Etiqueta.Quarta:
                            {
                                worksheet.Range["F18"].Text = vm.ChecklistPrduto.sigla;
                                worksheet.Range["G19"].Text = $"{volume} / {count}"; //item.volumes_total > 1 ? item.volumes + " / " + item.volumes_total : item.qtd.ToString();
                                worksheet.Range["H19"].Number = DateTime.Now.Year;
                                worksheet.Range["I19"].Number = (double)vm.ChecklistPrduto.coddetalhescompl;
                                worksheet.Range["G22"].Text = vm.ChecklistPrduto.local_shoppings;
                                worksheet.Range["F25"].Text = vm.Descricao.descricao_completa.Replace("ÚNICO", null);

                                var inx = 29;
                                worksheet.Range["F29"].Text = "";
                                worksheet.Range["F30"].Text = "";
                                worksheet.Range["F31"].Text = "";
                                worksheet.Range["F32"].Text = "";
                                worksheet.Range["F33"].Text = "";
                                foreach (var item in GetProdutosEtiqueta(volume, 5))
                                {
                                    worksheet.Range[$"F{inx}"].Text = item.descricao_peca;
                                    inx++;
                                }

                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                worksheet.Range["QUARTA"].CellStyle.Font.Color = ExcelKnownColors.Black;

                                etiqueta = Enum.Parse(typeof(Etiqueta), "Primeira");
                                _etiqueta = 1;
                                workbook.SaveAs($"Impressos/ETIQUETA_REQUISICAO_MODELO_{_pagina}.xlsx");

                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["PRIMEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["PRIMEIRA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["SEGUNDA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["SEGUNDA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["TERCEIRA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["TERCEIRA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["QUARTA"].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                worksheet.Range["QUARTA"].CellStyle.Font.Color = ExcelKnownColors.None;

                                _pagina++;
                                break;
                            }
                        default: break;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }

            }


            try
            {
                int idx = 1;
                for (int i = 0; i < paginas; i++)
                {
                    Process.Start(new ProcessStartInfo($"Impressos\\ETIQUETA_REQUISICAO_MODELO_{idx}.xlsx")
                    {
                        Verb = "Print",
                        UseShellExecute = true
                    });
                    idx++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }

        private ObservableCollection<ConstrucaoPecaModel> GetProdutosEtiqueta(long volume, int itens)
        {
            EtiquetaConstrucaoViewModel vm = (EtiquetaConstrucaoViewModel)DataContext;
            return new ObservableCollection<ConstrucaoPecaModel>(vm.Pecas.Where(p => p.volume_etiqueta == volume).Take(itens));
        }

        private async void OnDescricaoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EtiquetaConstrucaoViewModel? vm = (EtiquetaConstrucaoViewModel)DataContext;

                if (vm.Compledicional == null)
                {
                    MessageBox.Show("Produto não selecionado!");
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open("Modelos/DESCRICOES_CONSTRUCAO_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];

                vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync(vm.Compledicional.codcompladicional));
                vm.ChecklistPrduto = await Task.Run(() => vm.GetChecklistPrdutoAsync(vm.Compledicional.codcompladicional));

                worksheet.Range["A1"].Text = vm.Descricao.descricao_completa;
                worksheet.Range["A4"].Text = vm.ChecklistPrduto.sigla;
                worksheet.Range["A5"].Number = DateTime.Now.Year;
                worksheet.Range["A6"].Text = "CÓD. DET. COMPL.:";
                worksheet.Range["A7"].Number = (double)vm.ChecklistPrduto.coddetalhescompl;
                worksheet.Range["A8"].Text = vm.ChecklistPrduto.local_shoppings;
                worksheet.Range["A9"].Text = $"( {vm.Pecas.Count()} VOLUMES )"; //itens.View.Records.Count()
                worksheet.Range["A10"].Text = $"PEÇAS";
                //worksheet.Range["C3"].Text = Modelo.planilha;
                //worksheet.Range["C4"].Text = Modelo.descricao_completa;
                //worksheet.Range["C5"].Text = Modelo.tema;
                //worksheet.Range["A7"].Text = Modelo.obs_modelo;
                //worksheet.Range["H4"].Number = Convert.ToDouble(Modelo.multiplica);//String.Format(new CultureInfo("pt-BR"), "{0:G}", Modelo.multiplica); //Modelo.multiplica.ToString();
                //worksheet.Range["H4"].NumberFormat = "0.0";
                //Console.WriteLine(String.Format(new CultureInfo("pt-BR"), "{0:C}", 189.99));

                //var itens = (from i in vm.ItensReceita where i.quantidade > 0 select new { i.quantidade, i.planilha, i.descricao_completa, i.unidade }).ToList(); //new { a.Name, a.Age }

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
                bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                bodyStyle.Font.Size = 10;
                bodyStyle.EndUpdate();



                var index = 12;
                
                foreach (var item in vm.Pecas)
                {
                    worksheet.Range[$"A{index}"].Number = (double)item.item;
                    worksheet.Range[$"A{index}"].CellStyle = bodyStyle;
                    worksheet.Range[$"A{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    worksheet.Range[$"B{index}:J{index}"].Text = item.descricao_peca;
                    worksheet.Range[$"B{index}:J{index}"].CellStyle = bodyStyle;
                    worksheet.Range[$"B{index}:J{index}"].Merge();
                    worksheet.Range[$"B{index}:J{index}"].RowHeight = 26;
                    worksheet.Range[$"B{index}:J{index}"].WrapText = true;

                    //worksheet.Range[$"K{index}"].Number = (double)item.volume_etiqueta;
                    worksheet.Range[$"K{index}"].CellStyle = bodyStyle;
                    worksheet.Range[$"K{index}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    index++;
                }
                
                workbook.SaveAs($"Impressos/DESCRICOES_CONSTRUCAO_MODELO_{vm.ChecklistPrduto.coddetalhescompl}.xlsx");
                workbook.Close();

                Process.Start(new ProcessStartInfo($"Impressos\\DESCRICOES_CONSTRUCAO_MODELO_{vm.ChecklistPrduto.coddetalhescompl}.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnEtiquetaTermicaClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EtiquetaConstrucaoViewModel? vm = (EtiquetaConstrucaoViewModel)DataContext;
                if (vm.Compledicional == null)
                {
                    MessageBox.Show("Produto não selecionado!");
                    return;
                }

                var rDados = new Dictionary<string, object>();

                string ip = "192.168.2.192";
                int porta = 9100;
                var temperatura = "";

                var printQueue = new Queue<string>();
                int stepSize = 3;

                if (string.IsNullOrEmpty(ip))
                    throw new InvalidOperationException("Impressora não informada.");
                /*
                if (string.IsNullOrEmpty(temperatura))
                    throw new InvalidOperationException("Temperatura não informada.");
                */
                var volumes = vm.Pecas.OrderBy(o => o.volume_etiqueta).GroupBy(user => user.volume_etiqueta).ToList();
                vm.Descricao = await Task.Run(() => vm.GetDescricaoAsync(vm.Compledicional.codcompladicional));
                vm.ChecklistPrduto = await Task.Run(() => vm.GetChecklistPrdutoAsync(vm.Compledicional.codcompladicional));
                var fieldCount = volumes.Count;
                
                for (int i = 0; i < fieldCount; i++)
                {
                    long volume = (long)volumes[i].Key;
                    rDados.Add(volume.ToString(), Convert.IsDBNull(volumes[i]) ? null : volumes[i]);
                    //ConstrucaoPecaModel objt = (ConstrucaoPecaModel)volumes[i];
                    /*
                    foreach (var item in GetProdutosEtiqueta(volume))
                    {
                        worksheet.Range[$"A{inx}"].Text = item.descricao_peca;
                        inx++;
                    }
                    */
                    var dataToSend = new StringBuilder();
                    dataToSend.AppendLine("^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR4,4~SD10^JUS^LRN^CI28");
                    dataToSend.AppendLine("^XA");
                    dataToSend.AppendLine("^MMT");
                    dataToSend.AppendLine("^PW799");
                    dataToSend.AppendLine("^LL0799");
                    dataToSend.AppendLine("^LS0");
                    dataToSend.AppendLine("^FO0,0^GFA,12288,12288,00048,:Z64:");
                    dataToSend.AppendLine("eJztyqENADAIADA+51U+2SwWR0irGwGL5Rso3/d93/d93/d93/d93/d93/d93/d93/d93/d93/d937/1ofmEaMte:694B");
                    dataToSend.AppendLine("^FO0,224^GFA,32000,32000,00100,:Z64:");
                    dataToSend.AppendLine("eJztzUERAAAEADDN1fYjheOxFVgEADyVvawcDofD4XA4HA6Hw+FwOBwOh8PhcDgcDofD4XA4HA6Hw+FwOBwOh8PhcDgcDofD4XA4HA6Hw+FwOBwOh8PhcDgcDofD4XA4HA6Hw+FwOBwOh8PhcDgcDofD4XA4HA6Hw+FwOBwfD+DUAF19huw=:090B");

                    int inx = 1;
                    foreach (var item in GetProdutosEtiqueta(volume, 4))
                    {
                        if(inx == 1)
                        {
                            dataToSend.AppendLine("^FO0,512^GFA,09600,09600,00100,:Z64:");
                            dataToSend.AppendLine("eJztz0ENgDAAxdAdUIOKL2J4JKjkMAX/UEiWPgNNx5AkSZI2dtHmNx+hAwcdWEIH/KiEDvhRCR3woxI64EcldMCPSuiAH5XQAT8qoQN+VEIHNvo4H9hNT0iSJEm/egHVImYo:4EB6");
                            dataToSend.AppendLine($@"^FT182,570^AAN,27,15^FH\^FD{item.descricao_peca}^FS");
                        }

                        if (inx == 2)
                        {
                            dataToSend.AppendLine("^FO0,576^GFA,06400,06400,00100,:Z64:");
                            dataToSend.AppendLine("eJztz0ENgDAAxdAdUIOKL2J4JKjkMAX/UJaQPgNNx5C01UWb33yEDhx0YAkd8KMSOuBHJXTAj0rogB+V0AE/KqEDflRCB/yohA74UQkd+NHH+cBuekKStMEL7C1mKA==:ED81");
                            dataToSend.AppendLine($@"^FT182,614^AAN,27,15^FH\^FD{item.descricao_peca}^FS");
                        }

                        if (inx == 3)
                        {
                            dataToSend.AppendLine("^FO0,608^GFA,06400,06400,00100,:Z64:");
                            dataToSend.AppendLine("eJztz0ENgDAAxdAdUDMVXwR4XFDJAQX/0IWQPgNNx5AkSfqIi3bu+QgdOOjAK3TAj0rogB+V0AE/KqEDflRCB/yohA74UQkd8KMSOuBHJXTgRx/zhi16YpsHD8dmKA==:1636");
                            dataToSend.AppendLine($@"^FT182,660^AAN,27,15^FH\^FD{item.descricao_peca}^FS");
                        }

                        if (inx == 4)
                        {
                            dataToSend.AppendLine("^FO0,672^GFA,06400,06400,00100,:Z64:");
                            dataToSend.AppendLine("eJztzzERgDAAxdAOqEHFFwEeOVQy1ED/EIZenoFcxtCym3b98xE6cNCBKXTAj0rogB+V0AE/KqEDflRCB/yohA74UQkd8KMSOuBHJXRgo4/zhT30hCRJ2sQHZaVmKA==:278C");
                            dataToSend.AppendLine($@"^FT182,705^AAN,27,15^FH\^FDMOLDURA (FRONTAL) 1/4^FS{item.descricao_peca}^FS");
                        }

                        if (inx == 5)
                        {
                            dataToSend.AppendLine("^FO0,704^GFA,06400,06400,00100,:Z64:");
                            dataToSend.AppendLine("eJztz0ENgDAAxdAdpgYVX8TwSKaSAwr+oYSQPgNNx5Ak6aNO2nrnI3Rg0oFH6IAfldABPyqhA35UQgf8qIQO+FEJHfCjEjrgRyV0wI9K6MCPPo4Nu+gJCXMDn5xmKA==:B335");
                            dataToSend.AppendLine($@"^FT182,750^AAN,27,15^FH\^FDMOLDURA (FRONTAL) 1/5^FS{item.descricao_peca}^FS");
                        }

                        inx++;

                    }

                    dataToSend.AppendLine("^FO352,0^GFA,02560,02560,00020,:Z64:");
                    dataToSend.AppendLine("eJxjYBgFFAKmVShgAU51qFyFUXWj6kbVjaobVTeqbqSrYwxFAQ649I4CKgMANg4U8g==:E778");
                    dataToSend.AppendLine("^FO608,0^GFA,03072,03072,00024,:Z64:");
                    dataToSend.AppendLine("eJxjYBgFVAZMqzDAChxKWcDqiTd6VP2o+lH1o+pH1Y+qH1U/qp6q6hlDMUAI8UaPgiEJAClLFAI=:E9D0");
                    dataToSend.AppendLine("^FO352,128^GFA,07168,07168,00056,:Z64:");
                    dataToSend.AppendLine("eJzt2MEJACAMBMEg9l+HWKVYQoKI4GwDc++LkDK1mW8Urb69k+N5PB6Px+PxeDwej8fj8Xg8Hu8l7+LfKumnFiD0RjU=:9CB5");
                    dataToSend.AppendLine($@"^FT242,791^A0N,28,28^FH\^FDTOTAL DE {fieldCount} VOLUMES^FS");
                    dataToSend.AppendLine(@"^FT415,48^A0N,28,28^FB51,1,0,C^FH\^FDANO^FS");
                    dataToSend.AppendLine($@"^FT395,95^AAN,36,20^FB97,1,0,C^FH\^FD{DateTime.Now.Year}^FS");
                    dataToSend.AppendLine(@"^FT653,46^A0N,28,28^FB123,1,0,C^FH\^FDCÓD. DET.^FS");
                    dataToSend.AppendLine($@"^FT655,94^AAN,40,20^FB145,1,0,C^FH\^FD{vm.ChecklistPrduto.coddetalhescompl}^FS");
                    dataToSend.AppendLine(@"^FT480,134^A0N,23,24^FB223,1,0,C^FH\^FDLOCAL SHOPPING^FS");
                    dataToSend.AppendLine($@"^FT378,225^AAN,30,10^FB409,3,0,C^FH\^FD{vm.ChecklistPrduto.local_shoppings}^FS");
                    dataToSend.AppendLine(@"^FT59,80^A0N,60,28^FB263,1,0,C^FR^FH\^FDETIQUETA PRODUÇÃO^FS");
                    dataToSend.AppendLine($@"^FT15,190^A0N,80,40^FB351,1,0,C^FR^FH\^FD{vm.ChecklistPrduto.sigla}^FS");
                    dataToSend.AppendLine($@"^FT80,500^A0N,65,45^FB650,3,0,C^FR^FH\^FD{vm.Descricao.descricao_completa}^FS");
                    dataToSend.AppendLine("^PQ1,0,1,Y^XZ");
                    dataToSend.AppendLine();
                    printQueue.Enqueue(dataToSend.ToString());

                    //rDados.Clear();
                }

                while (printQueue.Any())
                {
                    using var client = new TcpClient();
                    var serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), porta);
                    client.Connect(serverEndPoint);
                    using var clientStream = client.GetStream();
                    //StreamWriter clientStream = new StreamWriter(@"C:\Temp\print.txt");
                    for (int i = 0; i < fieldCount; i++)
                    {
                        //var encoder = new ASCIIEncoding();
                        var encoder = new UTF8Encoding();
                        byte[] buffer = encoder.GetBytes(printQueue.Dequeue());
                        clientStream.Write(buffer, 0, buffer.Length);
                        //clientStream.BaseStream.Write(buffer, 0, buffer.Length);
                        if (!printQueue.Any()) break;
                    }
                    clientStream.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            /*
            EtiquetaConstrucaoViewModel? vm = (EtiquetaConstrucaoViewModel)DataContext;
            if (vm.Compledicional == null)
            {
                MessageBox.Show("Produto não selecionado!");
                return;
            }
            MessageBox.Show("Impressão em impressora térmica, ainda não foi implementado.");
            */
        }

        private void clearMedidas()
        {
            txt_largura.Text = null;
            txt_area.Text = null;
            txt_profundidade.Text = null;
            txt_volume.Text = null;
            txt_altura.Text = null;
            txt_peso.Text = null;
            txt_tamanho.Text = null;
            txt_dificuldade.Text = null;
        }
    }

    public class EtiquetaConstrucaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<PlanilhaConstrucaoModel> _planilhas;
        public ObservableCollection<PlanilhaConstrucaoModel> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
        }
        private PlanilhaConstrucaoModel _planilha;
        public PlanilhaConstrucaoModel Planilha
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
        private ChecklistPrdutoConstrucaoModel _checklistPrduto;
        public ChecklistPrdutoConstrucaoModel ChecklistPrduto
        {
            get { return _checklistPrduto; }
            set { _checklistPrduto = value; RaisePropertyChanged("ChecklistPrduto"); }
        }
        private ObservableCollection<ConstrucaoDetalheModel> _detalhes;
        public ObservableCollection<ConstrucaoDetalheModel> Detalhes
        {
            get { return _detalhes; }
            set { _detalhes = value; RaisePropertyChanged("Detalhes"); }
        }
        private ConstrucaoDetalheModel _detalhe;
        public ConstrucaoDetalheModel Detalhe
        {
            get { return _detalhe; }
            set { _detalhe = value; RaisePropertyChanged("Detalhe"); }
        }
        
        private ObservableCollection<ConstrucaoPecaModel> _pecas;
        public ObservableCollection<ConstrucaoPecaModel> Pecas
        {
            get { return _pecas; }
            set { _pecas = value; RaisePropertyChanged("Pecas"); }
        }
        private ConstrucaoPecaModel _peca;
        public ConstrucaoPecaModel Peca
        {
            get { return _peca; }
            set { _peca = value; RaisePropertyChanged("Peca"); }
        }

        private ObservableCollection<string> _tamanhos;
        public ObservableCollection<string> Tamanhos
        {
            get { return _tamanhos; }
            set { _tamanhos = value; RaisePropertyChanged("Tamanhos"); }
        }

        private ObservableCollection<string> _dificuldades;
        public ObservableCollection<string> Dificuldades
        {
            get { return _dificuldades; }
            set { _dificuldades = value; RaisePropertyChanged("Dificuldades"); }
        }

        private IList _historicos;
        public IList Historicos
        {
            get { return _historicos; }
            set { _historicos = value; RaisePropertyChanged("Historicos"); }
        }

        public async Task<ObservableCollection<PlanilhaConstrucaoModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                return new ObservableCollection<PlanilhaConstrucaoModel>(await db.PlanilhasConstrucao.OrderBy(c => c.planilha).ToListAsync());
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
                Produtos = new ObservableCollection<ProdutoModel>();
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
                DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
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

        public async Task<ObservableCollection<ConstrucaoDetalheModel>> GetDetalhesAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ConstrucaoDetalhes
                    .OrderBy(c => c.item)
                    .Where(c => c.codcompladicional == codcompladicional)
                    .ToListAsync();
                return new ObservableCollection<ConstrucaoDetalheModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<ObservableCollection<ConstrucaoPecaModel>> GetPecasAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ConstrucaoPecas
                    .OrderBy(c => c.item)
                    .Where(c => c.codcompladicional == codcompladicional)
                    .ToListAsync();
                return new ObservableCollection<ConstrucaoPecaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ConstrucaoPecaModel> SaveConstrucaoDetalheAsync(ConstrucaoPecaModel construcaoPeca)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ConstrucaoPecas.SingleMergeAsync(construcaoPeca);
                await db.SaveChangesAsync();
                return construcaoPeca;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QryDescricao> GetDescricaoAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.Descricoes.Where(c => c.codcompladicional == codcompladicional).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<ChecklistPrdutoConstrucaoModel> GetChecklistPrdutoAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.ChecklistPrdutoConstrucaos.Where(c => c.codcompladicional == codcompladicional).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList> GetHistoricoConstrucaoAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.HistoricoCheckList
                    .Where(r => r.planilha == "CONSTRUÇÃO" && r.qtd_completada > 0 && r.codcompladicional == codcompladicional)
                    .GroupBy(g => new { g.sigla, g.tema, g.ano })
                    .Select(p => new { p.Key.sigla, p.Key.tema, p.Key.ano })
                    .OrderBy(g => g.ano)
                    .ToListAsync();
                return data;

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
