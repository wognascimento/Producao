using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para ProgramacaoProducao.xam
    /// </summary>
    public partial class ProgramacaoProducao : UserControl
    {
        public ProgramacaoProducao()
        {
            InitializeComponent();
            DataContext = new ProgramacaoProducaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                vm.Locais = await Task.Run(vm.GetLocalicacoesAsync);
                vm.Programacoes = await Task.Run(vm.GetProgramacaoItensAsync);

                txtFila.Text = vm.Programacoes.Where(p => p.programacao_status == "FILA/M.O").Count().ToString(); //DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'FILA/M.O'")
                txtDiretoria.Text = vm.Programacoes.Where(p => p.programacao_status == "ESPAÇO FÍSICO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'ESPAÇO FÍSICO'")
                txtProjeto.Text = vm.Programacoes.Where(p => p.programacao_status == "EMBALAGEM/EXPEDIÇÃO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'EMBALAGEM/EXPEDIÇÃO'")
                txtAndamento.Text = vm.Programacoes.Where(p => p.programacao_status == "EM ANDAMENTO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'EM ANDAMENTO'")
                txtIndefinido.Text = vm.Programacoes.Where(p => p.programacao_status == "INDEFINIDO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'INDEFINIDO'")
                txtProjetos.Text = vm.Programacoes.Where(p => p.programacao_status == "PROJETOS").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'PROJETOS'")
                txtFaltaMaterialTranf.Text = vm.Programacoes.Where(p => p.programacao_status == "FALTA MATERIAL INTERNO / TRANSF").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'FALTA MATERIAL INTERNO / TRANSF'")
                txtFaltaMaterialCompras.Text = vm.Programacoes.Where(p => p.programacao_status == "FALTA MATERIAL EXTERNO / COMPRAS").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'FALTA MATERIAL EXTERNO / COMPRAS'")


                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void LocaisSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBoxAdv)sender; //= { Syncfusion.Windows.Tools.Controls.ComboBoxAdv Items.Count: 5}
            var local = (SetorProducaoModel)comboBox.SelectedItem;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                vm.Setores = await Task.Run(() => vm.GetSetorsAsync(local.localizacao));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void SetoresSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SelectedItems = Count = 2
            var comboBox = (ComboBoxAdv)sender; //= { Syncfusion.Windows.Tools.Controls.ComboBoxAdv Items.Count: 5}
            var setores = comboBox.SelectedItems;

            /*
             * 'System.Collections.ObjectModel.ObservableCollection`1[System.Object]' 
             * 'System.Collections.Generic.List`1[Producao.SetorModel]'.'
            */
        }

        private void OnAddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {

        }

        private void OnCurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {

        }

        private void OnRowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            //UpdateProgramacaoAsync(ProdutoServicoModel produtoServico)

        }

        private async void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            try
            {
                // RowData = { Producao.DataBase.Model.ProgramacaoProducaoModel}
                ProgramacaoProducaoModel data = (ProgramacaoProducaoModel)e.RowData;
                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(
                    () => vm.UpdateProgramacaoAsync(
                        new TGlobalModel
                        {
                            num_os = data.num_os, 
                            programacao_ordem = data.programacao_ordem,
                            programacao_observacao = data.programacao_observacao,
                            programacao_inserido_por = Environment.UserName, 
                            programacao_inserido_data = DateTime.Now
                        })
                    );
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                var filteredResult = programacao.View.Records.Select(recordentry => recordentry.Data);
                var itens = programacao.View.Records.Count;

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open("Modelos/PROGRAMACAO_PROGRAMACAO_MODELO.xlsx");
                IWorksheet worksheet = workbook.Worksheets[0];

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
                headerStyle.Font.Size = 10;
                headerStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                headerStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                //headerStyle.Font.FontName = "Calibri (Detalhe)";
                headerStyle.WrapText = true;
                //headerStyle.ShrinkToFit = true;
                headerStyle.EndUpdate();

                int _l = 9;

                foreach (ProgramacaoProducaoModel item in filteredResult)
                {

                    worksheet.Range[$"B{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"B{_l}"].Number = item.programacao_ordem.GetValueOrDefault(); //item.programacao_ordem.Value;

                    worksheet.Range[$"C{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"C{_l}"].DateTime = item.data_de_expedicao.GetValueOrDefault();
                    
                    worksheet.Range[$"D{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"D{_l}"].Value = item.cliente_os;
                    
                    worksheet.Range[$"E{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"E{_l}"].Number = item.cod_compl_adicional.GetValueOrDefault();
                    
                    worksheet.Range[$"F{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"F{_l}"].Value = item.planilha;
                    
                    worksheet.Range[$"G{_l}:K{_l}"].Merge();
                    worksheet.Range[$"G{_l}:K{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"G{_l}:K{_l}"].Value = item.descricao_completa;
                    worksheet.Range[$"G{_l}:K{_l}"].RowHeight = 26;

                    worksheet.Range[$"L{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"L{_l}"].Number = item.num_os.GetValueOrDefault();
                    
                    worksheet.Range[$"M{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"M{_l}"].Number = item.quantidade_os.GetValueOrDefault();

                    worksheet.Range[$"N{_l}:O{_l}"].Merge();
                    worksheet.Range[$"N{_l}:O{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"N{_l}:O{_l}"].Value = item.programacao_status;

                    worksheet.Range[$"P{_l}:Q{_l}"].Merge();
                    worksheet.Range[$"P{_l}:Q{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"P{_l}:Q{_l}"].Value = item.setor_caminho;

                    worksheet.Range[$"R{_l}:S{_l}"].Merge();
                    worksheet.Range[$"R{_l}:S{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"R{_l}:S{_l}"].Value = item.programacao_observacao;
                    
                    worksheet.Range[$"T{_l}"].CellStyle = headerStyle;
                    worksheet.Range[$"T{_l}"].Number = item.ht.GetValueOrDefault();

                    _l++;
                }

                workbook.SaveAs("Impressos/PROGRAMACAO_PROGRAMACAO_MODELO.xlsx");
                
                Process.Start(new ProcessStartInfo("Impressos\\PROGRAMACAO_PROGRAMACAO_MODELO.xlsx")
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

    class ProgramacaoProducaoViewModel : INotifyPropertyChanged
    {
        private SetorModel _setor;
        public SetorModel Setor
        {
            get { return _setor; }
            set { _setor = value; RaisePropertyChanged("Setor"); }
        }
        private ObservableCollection<SetorModel> _setores;
        public ObservableCollection<SetorModel> Setores
        {
            get { return _setores; }
            set { _setores = value; RaisePropertyChanged("Setores"); }
        }

        private SetorProducaoModel _local;
        public SetorProducaoModel Local
        {
            get { return _local; }
            set { _local = value; RaisePropertyChanged("Local"); }
        }
        private ObservableCollection<SetorProducaoModel> _locais;
        public ObservableCollection<SetorProducaoModel> Locais
        {
            get { return _locais; }
            set { _locais = value; RaisePropertyChanged("Locais"); }
        }

        private ProgramacaoProducaoModel _programacao;
        public ProgramacaoProducaoModel Programacao
        {
            get { return _programacao; }
            set { _programacao = value; RaisePropertyChanged("Programacao"); }
        }
        private ObservableCollection<ProgramacaoProducaoModel> _programacoes;
        public ObservableCollection<ProgramacaoProducaoModel> Programacoes
        {
            get { return _programacoes; }
            set { _programacoes = value; RaisePropertyChanged("Programacoes"); }
        }

        public async Task<ObservableCollection<SetorProducaoModel>> GetLocalicacoesAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.SetorProducaos
                    .GroupBy(p => p.localizacao)
                    .Select(g => g.OrderBy(p => p.localizacao).FirstOrDefault())
                    .ToListAsync();

                return new ObservableCollection<SetorProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<SetorModel>> GetSetorsAsync(string localizacao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await (from s in db.SetorProducaos orderby s.setor where s.inativo == "0    " && s.localizacao == localizacao select new SetorModel { setor = s.setor + " - " + s.galpao, codigo_setor = s.codigo_setor }).ToListAsync();
                return new ObservableCollection<SetorModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProgramacaoProducaoModel>> GetProgramacaoItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ProgramacaoProducoes
                    .Where(p => p.quantidade_os > 0)
                    .ToListAsync();

                return new ObservableCollection<ProgramacaoProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateProgramacaoAsync(TGlobalModel global)
        {
            try
            {
                using DatabaseContext db = new();
                TGlobalModel servico = await db.Globais.FindAsync(global.num_os);
                servico.programacao_ordem = global.programacao_ordem;
                servico.programacao_observacao = global.programacao_observacao;
                servico.programacao_inserido_por = global.programacao_inserido_por;
                servico.programacao_inserido_data = global.programacao_inserido_data;

                await db.SaveChangesAsync();
                //await db.Globais.SingleUpdateAsync(servico);
            }
            catch (Exception)
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

    public class ProgramacaoProducaoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //value == null || ((value != null) && double.Parse(value.ToString()) == 0)
            var data = value as ProgramacaoProducaoModel;
            if (data?.dias_expedicao < 0)
                return new SolidColorBrush(Colors.Red);
            //else if (data?.condicao == 2)
                //return new SolidColorBrush(Colors.LightGreen);
            //else if (data?.condicao == 3)
                //return new SolidColorBrush(Colors.LightSkyBlue);
            else
                return DependencyProperty.UnsetValue;
            //return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
