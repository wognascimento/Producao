using CommunityToolkit.Mvvm.ComponentModel;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.DataBase.Model;
using Producao.Views.Helper;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Producao.Views.Controle;

/// <summary>
/// Interação lógica para Planta.xam
/// </summary>
public partial class Planta : UserControl
{
    private DataBaseSettings _dataBaseSettings = DataBaseSettings.Instance;

    public Planta()
    {
        InitializeComponent();
        this.DataContext = new ViewPlantaViewModel();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var vm = (ViewPlantaViewModel)DataContext;
            await vm.GetAprovadosAsync();
            await vm.GetRespPlantaChapasAsync();
            await vm.GetRespPlantaCercasAsync();
            await vm.GetRespPlantaBasesAsync();
            await vm.GetRespPlantaPracasAsync();
            await vm.GetRespRevisaoPlantaPracasAsync();
            await vm.GetRespPlantaMallsAsync();
            await vm.GetRespPlantaFachadasAsync();
            await vm.GetRespPlantaCorteElevacoesAsync();
            await vm.GetRespPlantaAsBuiltsAsync();

        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Erro: {ex.InnerException.Message}");
        }
        catch (PostgresException ex)
        {
            MessageBox.Show($"Erro: {ex.InnerException.Message}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}");
        }
    }

    private void OnCurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
    {

    }

    private async void OnCurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
    {
        var dataGrid = sender as SfDataGrid;
        var vm = (ViewPlantaViewModel)DataContext;
        var provider = dataGrid.View.GetPropertyAccessProvider();
        var record = dataGrid.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex) as AprovadoModel;
        var column = dataGrid.Columns[dataGrid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex)];
        var dataRow = this.itens.RowGenerator.Items.FirstOrDefault(item => item.RowIndex == e.RowColumnIndex.RowIndex);
        var newEntity = new TAprovadoModel();
        MapperHelper.CopyMatchingProperties(record, newEntity);
        
        try
        {
            if (column.MappingName.Equals("ok_planta_base"))
            {
                record.planta_base = _dataBaseSettings.Username;
                record.liberacao_planta_base = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("conclusao_planta_cerca"))
            {
                record.planta_cercas_concluida_por = _dataBaseSettings.Username;
                record.data_conclusao_planta_cercas = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("conclusao_revisao_planta_base"))
            {
                record.revisao_planta_base_concluida_por = _dataBaseSettings.Username;
                record.data_revisao_planta_base = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("ok_planta_pca"))
            {
                record.planta_pca = _dataBaseSettings.Username;
                record.liberacao_planta_pca = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }

            else if (column.MappingName.Equals("conclusao_revisao_planta_praca"))
            {
                record.revisao_planta_praca_concluida_por = _dataBaseSettings.Username;
                record.data_conclusso_revisao_planta_praca = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("conclusao_revisao_final"))
            {
                record.revisao_final_concluida_por = _dataBaseSettings.Username;
                record.data_conclusao_revisao_final = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("conclusao_retorno_vt"))
            {
                record.resp_retorno_vt = _dataBaseSettings.Username;
                record.data_retorno_vt = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("ok_planta_mall"))
            {
                record.planta_mall = _dataBaseSettings.Username;
                record.conclusao_planta_mall = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("OkPlantaFachada"))
            {
                record.planta_fachada = _dataBaseSettings.Username;
                record.conclusao_planta_fachada = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("conclusao_planta_corte_elevacao"))
            {
                record.planta_corte_elevacao_concluida_por = _dataBaseSettings.Username;
                record.data_conclusao_planta_corte_elevacao = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }
            else if (column.MappingName.Equals("conclusao_planta_as_built"))
            {
                record.as_built_plantas = _dataBaseSettings.Username;
                record.as_built_plantas_data = DateTime.Now;
                await vm.SaveAsync(newEntity);
                dataGrid.View.Refresh();
            }  
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}");
        }

    }

    private async void itens_RowValidated(object sender, RowValidatedEventArgs e)
    {
        try
        {
            var vm = (ViewPlantaViewModel)DataContext;
            var newEntity = new TAprovadoModel();
            MapperHelper.CopyMatchingProperties(e.RowData, newEntity);
            await vm.SaveAsync(newEntity);
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Erro: {ex.InnerException.Message}");
        }
    }
}

public partial class ViewPlantaViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<AprovadoModel> aprovados;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaChapas;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaCercas;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaBases;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaPracas;
    [ObservableProperty]
    private ObservableCollection<string> respRevisaoPlantaPracas;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaMalls;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaFachadas;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaCorteElevacoes;
    [ObservableProperty]
    private ObservableCollection<string> respPlantaAsBuilts;

    private DataBaseSettings _dataBaseSettings = DataBaseSettings.Instance;

    public async Task GetAprovadosAsync()
    {
        using DatabaseContext _dbContext = new();
        var result = await _dbContext.Aprovados
            .OrderBy(f => f.sigla)
            .ToListAsync();
        Aprovados =  new ObservableCollection<AprovadoModel>(result);
    }

    public async Task GetRespPlantaChapasAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "CHAPAS" });
        RespPlantaChapas = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespPlantaCercasAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "CERCAS" });
        RespPlantaCercas = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespPlantaBasesAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "BASE" });
        RespPlantaBases = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespPlantaPracasAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "PRAÇA" });
        RespPlantaPracas = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespRevisaoPlantaPracasAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "REVISÃO PRAÇA" });
        RespRevisaoPlantaPracas = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespPlantaMallsAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "MALL" });
        RespPlantaMalls = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespPlantaFachadasAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "FACHADA" });
        RespPlantaFachadas = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespPlantaCorteElevacoesAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "CORTE/ELEVAÇÃO" });
        RespPlantaCorteElevacoes = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task GetRespPlantaAsBuiltsAsync()
    {
        using var connection = new NpgsqlConnection(_dataBaseSettings.connectionString);
        string sql = @"SELECT resp FROM projetos.resp_planta WHERE tipo = @Tipo ORDER BY resp;";
        var result = await connection.QueryAsync<string>(sql, new { Tipo = "AS BUILT" });
        RespPlantaAsBuilts = new ObservableCollection<string>(result)
        {
            "NÃO TEM"
        };
    }

    public async Task SaveAsync(TAprovadoModel model)
    {
            using DatabaseContext db = new();
            var modelExistente = await db.TAprovados.FindAsync(model.id_aprovado);
            if (modelExistente == null)
                await db.TAprovados.AddAsync(model);
            else
                db.Entry(modelExistente).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
    }
}
