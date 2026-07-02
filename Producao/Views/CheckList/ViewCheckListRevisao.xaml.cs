using ClosedXML.Excel;
using Dapper;
using Npgsql;
using Producao.Utils;
using System;
using System.Collections.Generic;
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

namespace Producao.Views;

/// <summary>
/// Interação lógica para ViewCheckListRevisao.xam
/// </summary>
public partial class ViewCheckListRevisao : UserControl
{

    static ViewCheckListRevisao()
    {
        SqlMapper.AddTypeHandler(new DateOnlyToDateTimeHandler());
    }

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
            Producao.ErrorDialog.Show(ex, "Erro");
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
            Producao.ErrorDialog.Show(ex, "Erro");
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
            Producao.ErrorDialog.Show(ex, "Erro");
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
            Producao.ErrorDialog.Show(ex, "Erro");
        }
    }

    private static void ExportarExcel(RadGridView grid, string path)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Revisão");
            Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
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
    static ViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

    private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
    {
        await using var conn = CreateConnection();
        var data = await conn.QueryAsync<T>(sql, param);
        return data.ToList();
    }

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
            const string sql = """
                SELECT *
                FROM producao.view_controle_memorial;
                """;

            var data = await QueryAsync<ControleMemorialModel>(sql);
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
            const string sql = """
                SELECT *
                FROM producao.tbl_revisores
                ORDER BY revisores;
                """;

            var data = await QueryAsync<RevisorModel>(sql);
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
            await using var conn = CreateConnection();

            if (controle.cod_linha_qdfecha is null or 0)
            {
                const string insertSql = """
                    INSERT INTO producao.view_controle_memorial
                        (data_aprovado, sigla, sigla_serv, memo_data, data_memo_visual, item, tema, familia, qtd,
                         descricaocomercial, dimensao, bloco, data_revisado, obs_memorial, obs_fecha, obs_interna,
                         obs_alteracao, status, liberado, ok, resp_revisao, prazo_revisao, obs_revisao, local,
                         altera_ok, confirma_alteracao_por, confirma_alteracao_data, memorial_alterado_por,
                         memorial_data_alterado, fechamento_shopp, revisado_por, data_revisado_por,
                         data_de_expedicao, conclusao_planta_pca, motivo_alt_pos_revisao, ok_revisao_alterada,
                         revisao_alt_por, data_alt_revisao, detalhe_local, pendencia)
                    VALUES
                        (@data_aprovado, @sigla, @sigla_serv, @memo_data, @data_memo_visual, @item, @tema, @familia, @qtd,
                         @descricaocomercial, @dimensao, @bloco, @data_revisado, @obs_memorial, @obs_fecha, @obs_interna,
                         @obs_alteracao, @status, @liberado, @ok, @resp_revisao, @prazo_revisao, @obs_revisao, @local,
                         @altera_ok, @confirma_alteracao_por, @confirma_alteracao_data, @memorial_alterado_por,
                         @memorial_data_alterado, @fechamento_shopp, @revisado_por, @data_revisado_por,
                         @data_de_expedicao, @conclusao_planta_pca, @motivo_alt_pos_revisao, @ok_revisao_alterada,
                         @revisao_alt_por, @data_alt_revisao, @detalhe_local, @pendencia);
                    """;

                await conn.ExecuteAsync(insertSql, controle);
            }
            else
            {
                const string updateSql = """
                    UPDATE producao.view_controle_memorial
                    SET data_aprovado = @data_aprovado,
                        sigla = @sigla,
                        sigla_serv = @sigla_serv,
                        memo_data = @memo_data,
                        data_memo_visual = @data_memo_visual,
                        item = @item,
                        tema = @tema,
                        familia = @familia,
                        qtd = @qtd,
                        descricaocomercial = @descricaocomercial,
                        dimensao = @dimensao,
                        bloco = @bloco,
                        data_revisado = @data_revisado,
                        obs_memorial = @obs_memorial,
                        obs_fecha = @obs_fecha,
                        obs_interna = @obs_interna,
                        obs_alteracao = @obs_alteracao,
                        status = @status,
                        liberado = @liberado,
                        ok = @ok,
                        resp_revisao = @resp_revisao,
                        prazo_revisao = @prazo_revisao,
                        obs_revisao = @obs_revisao,
                        local = @local,
                        altera_ok = @altera_ok,
                        confirma_alteracao_por = @confirma_alteracao_por,
                        confirma_alteracao_data = @confirma_alteracao_data,
                        memorial_alterado_por = @memorial_alterado_por,
                        memorial_data_alterado = @memorial_data_alterado,
                        fechamento_shopp = @fechamento_shopp,
                        revisado_por = @revisado_por,
                        data_revisado_por = @data_revisado_por,
                        data_de_expedicao = @data_de_expedicao,
                        conclusao_planta_pca = @conclusao_planta_pca,
                        motivo_alt_pos_revisao = @motivo_alt_pos_revisao,
                        ok_revisao_alterada = @ok_revisao_alterada,
                        revisao_alt_por = @revisao_alt_por,
                        data_alt_revisao = @data_alt_revisao,
                        detalhe_local = @detalhe_local,
                        pendencia = @pendencia
                    WHERE cod_linha_qdfecha = @cod_linha_qdfecha;
                    """;

                await conn.ExecuteAsync(updateSql, controle);
            }
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



