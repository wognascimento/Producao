using ClosedXML.Excel;
using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

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
        foreach (var column in itens.Columns)
            column.IsReadOnly = column.UniqueName != "peso";
    }

    private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
    {
        if (e.EditOperationType == GridViewEditOperationType.None || e.Row.Item is not QryDescricao item)
            return;
        if (!item.codcompladicional.HasValue ||
            (item.peso.HasValue && (!double.IsFinite(item.peso.Value) || item.peso.Value < 0)))
        {
            e.IsValid = false;
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = "peso", ErrorMessage = "Informe um peso valido, maior ou igual a zero, em um produto com codigo."
            });
            return;
        }
        DescricaoPesoGridSave.Save(sender, e,
            () => ((TodasDescricoesViewModel)DataContext).SalvarPesoAsync(item));
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            TodasDescricoesViewModel vm = (TodasDescricoesViewModel)DataContext;
            vm.Descricoes = await vm.GetDescricoesAsync();
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
            Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);

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

            var filePath = DataBaseSettings.Instance.ResolveImpressosPath("CADASTRO_PRODUTO.xlsx");
            Producao.Utils.ExcelExportHelper.SaveWithoutFormatting(workbook, filePath);
            Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Producao.ErrorDialog.Show(ex, "Erro");
        }
    }
}

public class TodasDescricoesViewModel : INotifyPropertyChanged
{
    private readonly Dictionary<QryDescricao, double?> pesosOriginais = new();
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
            var descricoes = new ObservableCollection<QryDescricao>(data);
            pesosOriginais.Clear();
            foreach (var item in descricoes)
                pesosOriginais[item] = item.peso;
            return descricoes;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task SalvarPesoAsync(QryDescricao item)
    {
        if (!pesosOriginais.TryGetValue(item, out var original))
            throw new InvalidOperationException("Recarregue a lista antes de alterar o peso.");
        if (Nullable.Equals(original, item.peso))
            return;

        await using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();
        var affected = await conn.ExecuteAsync(@"
            UPDATE producao.tblcomplementoadicional
            SET peso = @peso
            WHERE codcompladicional = @codcompladicional
              AND peso IS NOT DISTINCT FROM @original;",
            new { item.peso, item.codcompladicional, original }, transaction);
        if (affected != 1)
            throw new InvalidOperationException("Produto nao encontrado ou peso alterado por outro usuario. Recarregue a lista.");
        await transaction.CommitAsync();
        pesosOriginais[item] = item.peso;
    }
}

