using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CadastroProduto;

/// <summary>
/// Interação lógica para TodasDescricoes.xam
/// </summary>
public partial class TodasDescricoes : UserControl
{
    public TodasDescricoes()
    {
        DataContext = new TodasDescricoesViewModel();
        InitializeComponent();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            TodasDescricoesViewModel vm = (TodasDescricoesViewModel)DataContext;
            vm.Descricoes = await Task.Run(vm.GetDescricoesAsync);
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

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
    }
}

public class TodasDescricoesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string propName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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

    public async Task<ObservableCollection<QryDescricao>> GetDescricoesAsync()
    {
        try
        {
            using DatabaseContext db = new();
            var data = await db.Descricoes.ToListAsync();
            return new ObservableCollection<QryDescricao>(data);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public static class ContextMenuCommandsCadastroProduto
{

    static DataBaseSettings BaseSettings = DataBaseSettings.Instance;

    static BaseCommand? exportarExcel;
    public static BaseCommand ExportarExcel
    {
        get { exportarExcel ??= new BaseCommand(OnExportarExcel); return exportarExcel; }
    }
    private static void OnExportarExcel(object? obj)
    {
        try
        {
            if (obj is GridColumnContextMenuInfo grid)
            {
                //var excelEngine = grid.DataGrid.ExportToExcel(grid.DataGrid.View, options);
                //var workbook = excelEngine.Excel.Workbooks[0];
                //var sheet = workbook.Worksheets[0];
                // Supondo que seu SfDataGrid se chame "sfDataGrid"
                //var dados = grid.DataGrid.View.SourceCollection.Cast<object>().ToList();
                // 🔹 Pega apenas os itens filtrados e visíveis no grid
                var dados = grid.DataGrid.View.Records
                    .Select(r => r.Data)
                    .ToList();

                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];

                // 🔹 1. Cabeçalhos (nomes das colunas do SfDataGrid)
                int col = 1;
                foreach (var coluna in grid.DataGrid.Columns)
                {
                    worksheet.Range[1, col].Text = coluna.HeaderText;
                    col++;
                }

                // 🔹 2. Dados
                int row = 2;
                foreach (var item in dados)
                {
                    col = 1;
                    foreach (var coluna in grid.DataGrid.Columns)
                    {
                        var valor = item.GetType().GetProperty(coluna.MappingName)?.GetValue(item);

                        // Forçar como texto se contiver caracteres especiais
                        if (valor == null)
                            worksheet.Range[row, col].Value = "";
                        else if (valor is string texto)
                            worksheet.Range[row, col].Text = texto; // Texto puro
                        else if (double.TryParse(valor.ToString(), out double numero))
                            worksheet.Range[row, col].Number = numero; // Numérico
                        else
                            worksheet.Range[row, col].Text = valor.ToString();

                        col++;
                    }
                    row++;
                }

                // 🔹 Ajustar largura automática
                worksheet.UsedRange.AutofitColumns();

                workbook.SaveAs(@$"{BaseSettings.CaminhoSistema}\Impressos\CADASTRO_PRODUTO.xlsx");
                Process.Start(new ProcessStartInfo(@$"{BaseSettings.CaminhoSistema}\Impressos\CADASTRO_PRODUTO.xlsx")
                {
                    UseShellExecute = true
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}

