using ClosedXML.Excel;
using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

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

    private void OnExportarExcelClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var dados = itens.Items.OfType<QryDescricao>().ToList();
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Produtos");

            var col = 1;
            foreach (var coluna in itens.Columns.OfType<GridViewDataColumn>())
            {
                worksheet.Cell(1, col).Value = coluna.Header?.ToString() ?? coluna.UniqueName;
                col++;
            }

            var row = 2;
            foreach (var item in dados)
            {
                col = 1;
                foreach (var coluna in itens.Columns.OfType<GridViewDataColumn>())
                {
                    var propertyName = coluna.UniqueName;
                    var valor = item.GetType().GetProperty(propertyName)?.GetValue(item);
                    worksheet.Cell(row, col).Value = valor switch
                    {
                        null => string.Empty,
                        string texto => texto,
                        int numero => numero,
                        long numero => numero,
                        double numero => numero,
                        decimal numero => numero,
                        DateTime data => data,
                        _ => valor.ToString()
                    };
                    col++;
                }
                row++;
            }

            var lastColumn = Math.Max(itens.Columns.Count, 1);
            var lastRow = Math.Max(row - 1, 1);
            worksheet.Range(1, 1, lastRow, lastColumn).CreateTable("CadastroProduto");
            worksheet.Columns().AdjustToContents();

            var filePath = DataBaseSettings.Instance.ResolveImpressosPath("CADASTRO_PRODUTO.xlsx");
            workbook.SaveAs(filePath);
            Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}

public class TodasDescricoesViewModel : INotifyPropertyChanged
{
    readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;

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
            using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
            var data = await conn.QueryAsync<QryDescricao>(
                @"SELECT *
                  FROM producao.qry3descricoes;");
            return new ObservableCollection<QryDescricao>(data);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

