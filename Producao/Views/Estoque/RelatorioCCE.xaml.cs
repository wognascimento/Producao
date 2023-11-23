using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Producao.Views.Estoque
{
    /// <summary>
    /// Interação lógica para RelatorioCCE.xam
    /// </summary>
    public partial class RelatorioCCE : UserControl
    {
        public RelatorioCCE()
        {
            InitializeComponent();
            DataContext = new RelatorioCCEViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                RelatorioCCEViewModel vm = (RelatorioCCEViewModel)DataContext;
                vm.Planilhas = await Task.Run(vm.GetPlanilhasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void ButtonAdv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                RelatorioCCEViewModel vm = (RelatorioCCEViewModel)DataContext;
                vm.Descricoes = await Task.Run(() => vm.GetDescricoesAsync(vm.Planilha.planilha));
                using (ExcelEngine excelEngine = new())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Create a new workbook
                    //IWorkbook workbook = application.Workbooks.Create(1);
                    IWorkbook workbook = application.Workbooks.OpenReadOnly("Modelos\\RELATORIO_CCE_MODELO.xlsx");
                    IWorksheet worksheet = workbook.Worksheets[0];

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
                    bodyStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    //bodyStyle.Font.Bold = true;
                    bodyStyle.WrapText = true;
                    bodyStyle.EndUpdate();

                    worksheet.Range["H1"].Text = vm.Planilha.planilha;
                    var row = 5;
                    foreach (var item in vm.Descricoes)
                    {
                        worksheet.Range[$"A{row}:B{row}"].Merge();
                        worksheet.Range[$"A{row}:B{row}"].Text = item.codcompladicional.ToString();
                        worksheet.Range[$"A{row}:B{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"A{row}:B{row}"].HorizontalAlignment = ExcelHAlign.HAlignCenter;


                        worksheet.Range[$"C{row}:K{row}"].Merge();
                        worksheet.Range[$"C{row}:K{row}"].Text = item.descricao_completa;
                        worksheet.Range[$"C{row}:K{row}"].CellStyle = bodyStyle;

                        worksheet.Range[$"L{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"M{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"N{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"O{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"P{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"Q{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"R{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"S{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"T{row}"].CellStyle = bodyStyle;
                        worksheet.Range[$"U{row}"].CellStyle = bodyStyle;

                        row++;
                    }

                    //Autofit the columns
                    //sheet.UsedRange.AutofitColumns();

                    workbook.SaveAs("Impressos\\RELATORIO_CCE.xlsx");

                    Process.Start(new ProcessStartInfo("Impressos\\RELATORIO_CCE.xlsx")
                    {
                        UseShellExecute = true
                    });


                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void RowDefinition_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class RelatorioCCEViewModel : INotifyPropertyChanged
    {

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

        private ObservableCollection<QryDescricao> _descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return _descricoes; }
            set { _descricoes = value; RaisePropertyChanged("Descricoes"); }
        }
        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }

        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                return new ObservableCollection<RelplanModel>(await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1")).ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryDescricao>> GetDescricoesAsync(string planilha)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Descricoes.Where(c => c.inativo.Equals("0") && c.planilha == planilha).ToListAsync();
                return new ObservableCollection<QryDescricao>(data);
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
}
