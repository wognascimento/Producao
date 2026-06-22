using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views;

/// <summary>
/// Interação lógica para ViewCheckListRevisao.xam
/// </summary>
public partial class ViewCheckListRevisao : UserControl
{


    public ViewCheckListRevisao()
    {
        InitializeComponent();
        this.DataContext = new ViewModel();
        //itens.Columns["ok"].FilterPredicates.Add(new FilterPredicate() { FilterType = FilterType.Equals, FilterValue = "0    " });
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
            ViewModel vm = (ViewModel)DataContext;
            await vm.GetDados();
            await vm.GetRevisores();
            //itens.Columns["ok"].FilterPredicates.Add(new FilterPredicate() { FilterType = FilterType.Equals, FilterValue = "0    " });
            ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
    }

    private async void itens_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
    {
        ViewModel vm = (ViewModel)DataContext;
        var grid = sender as RadGridView;
        var columnName = e.Cell?.Column?.UniqueName;
        var record = e.Cell?.DataContext as ControleMemorialModel;

        if (record is null || string.IsNullOrWhiteSpace(columnName))
        {
            return;
        }

        if (columnName == "altera_ok")
        {
            record.confirma_alteracao_por = Environment.UserName;
            record.confirma_alteracao_data = DateTime.Now.Date;
        }
        
        if (columnName == "motivo_alt_pos_revisao")
        {
            record.ok_revisao_alterada = "-1";
            record.data_alt_revisao = DateTime.Now.Date;
            record.revisao_alt_por = Environment.UserName;
        }
        
        if (columnName == "ok")
        {
            record.revisado_por = Environment.UserName;
            record.data_revisado_por = DateTime.Now.Date;
            record.ok_revisao_alterada = "-1";
        }

        try
        {
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            await vm.AtualizarControleAsync(record);
            grid?.Items.Refresh();
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }

    private async void itens_RowValidated(object sender, GridViewRowValidatedEventArgs e)
    {
        try
        {
            ViewModel vm = (ViewModel)DataContext;
            var record = e.Row?.Item as ControleMemorialModel;
            if (record is null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            await vm.AtualizarControleAsync(record);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }

    private void OnExportarExcelClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var path = DataBaseSettings.Instance.ResolveImpressosPath("REVISÃO_CHECKLIST.xlsx");
            ExportarExcel(itens, path);
            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private static void ExportarExcel(RadGridView grid, string path)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Revisão");
        var columns = grid.Columns
            .Cast<Telerik.Windows.Controls.GridViewColumn>()
            .Where(column => column.IsVisible && !string.IsNullOrWhiteSpace(column.UniqueName))
            .ToList();

        for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
        {
            worksheet.Cell(1, columnIndex + 1).Value = columns[columnIndex].Header?.ToString() ?? columns[columnIndex].UniqueName;
            worksheet.Cell(1, columnIndex + 1).Style.Font.Bold = true;
        }

        var rows = grid.Items.OfType<ControleMemorialModel>().ToList();
        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                var property = typeof(ControleMemorialModel).GetProperty(columns[columnIndex].UniqueName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                var value = property?.GetValue(rows[rowIndex]);
                worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = value switch
                {
                    null => string.Empty,
                    DateTime date => date,
                    _ => value.ToString()
                };
            }
        }

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(path);
    }
}

public class ViewModel : INotifyPropertyChanged
{
    private ObservableCollection<ControleMemorialModel> _dados;
    public ObservableCollection<ControleMemorialModel> Dados
    {
        get { return _dados; }
        set { _dados = value; RaisePropertyChanged("Dados"); }
    }

    private ObservableCollection<RevisorModel> _revisores;
    public ObservableCollection<RevisorModel> Revisores
    {
        get { return _revisores; }
        set { _revisores = value; RaisePropertyChanged("Revisores"); }
    }

    public ViewModel()
    {
        //Dados = new ObservableCollection<ControleMemorialModel>(); 
    }

    public async Task GetDados()
    {
        try
        {
            using DatabaseContext db = new();
            var data = await db.ControleMemorials.ToListAsync();
            Dados = new ObservableCollection<ControleMemorialModel>(data);

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task GetRevisores()
    {
        try
        {
            using DatabaseContext db = new();
            var data = await db.Revisores.OrderBy(r => r.revisores).ToListAsync();
            Revisores = new ObservableCollection<RevisorModel>(data);
        }
        catch (Exception)
        {
            throw;
        }
    }
    //ControleMemorialModel
    public async Task AtualizarControleAsync(ControleMemorialModel controle)
    {
        try
        {
            using DatabaseContext db = new();
            db.Entry(controle).State = controle.cod_linha_qdfecha == null ?
                               EntityState.Added :
                               EntityState.Modified;

            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string propName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }
}

public class PrazoColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var data = value as ControleMemorialModel;

        if (data == null)
            return DependencyProperty.UnsetValue;

        return data.ok.Trim().Contains('0')
               && data.prazo_revisao.HasValue
               && data.prazo_revisao.Value.Date < DateTime.Today
            ? new SolidColorBrush(Colors.Red)
            : DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SiglaColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
  
        var data = value as ControleMemorialModel;

        
        if (data == null)
            return DependencyProperty.UnsetValue;
        

        if (data.altera_ok.Trim().Contains('0') && data.memorial_alterado_por != null)
            return new SolidColorBrush(Colors.Yellow);
        else if(data.memorial_data_alterado > data.confirma_alteracao_data)
            return new SolidColorBrush(Colors.Red);
        else
            return DependencyProperty.UnsetValue;

        /*
        return data.altera_ok.Trim().Contains('0') && data.memorial_alterado_por != null
            ? new SolidColorBrush(Colors.Yellow)
            : DependencyProperty.UnsetValue;

        return data.memorial_data_alterado > data.confirma_alteracao_data
            ? new SolidColorBrush(Colors.Yellow)
            : DependencyProperty.UnsetValue;
        */

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


