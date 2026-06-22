using CommunityToolkit.Mvvm.ComponentModel;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.DataBase.Model;
using Producao.Views.Helper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Controle;

/// <summary>
/// Interação lógica para Planta.xam
/// </summary>
public partial class Planta : UserControl
{
    private DataBaseSettings _dataBaseSettings = DataBaseSettings.Instance;

    public Planta()
    {
        try
        {
            InitializeComponent();
            this.DataContext = new ViewPlantaViewModel();
        }
        catch (Exception ex)
        {
            System.IO.File.WriteAllText("crash_constructor.log", ex.ToString());
            MessageBox.Show($"Erro no construtor: {ex.Message}");
        }
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

    private void OnCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
    {
        _ = AtualizarConclusaoAsync(sender as RadGridView, e.Cell?.DataContext as AprovadoModel, e.Cell?.Column?.UniqueName);
    }

    private async Task AtualizarConclusaoAsync(RadGridView dataGrid, AprovadoModel record, string columnName)
    {
        if (dataGrid is null || record is null || string.IsNullOrWhiteSpace(columnName))
        {
            return;
        }

        var vm = (ViewPlantaViewModel)DataContext;
        
        try
        {
            if (columnName.Equals("ok_planta_base"))
            {
                record.planta_base = _dataBaseSettings.Username;
                record.liberacao_planta_base = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("conclusao_planta_cerca"))
            {
                record.planta_cercas_concluida_por = _dataBaseSettings.Username;
                record.data_conclusao_planta_cercas = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("conclusao_revisao_planta_base"))
            {
                record.revisao_planta_base_concluida_por = _dataBaseSettings.Username;
                record.data_revisao_planta_base = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("ok_planta_pca"))
            {
                record.planta_pca = _dataBaseSettings.Username;
                record.liberacao_planta_pca = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }

            else if (columnName.Equals("conclusao_revisao_planta_praca"))
            {
                record.revisao_planta_praca_concluida_por = _dataBaseSettings.Username;
                record.data_conclusso_revisao_planta_praca = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("conclusao_revisao_final"))
            {
                record.revisao_final_concluida_por = _dataBaseSettings.Username;
                record.data_conclusao_revisao_final = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("conclusao_retorno_vt"))
            {
                record.resp_retorno_vt = _dataBaseSettings.Username;
                record.data_retorno_vt = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("ok_planta_mall"))
            {
                record.planta_mall = _dataBaseSettings.Username;
                record.conclusao_planta_mall = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("ok_planta_fachada"))
            {
                record.planta_fachada = _dataBaseSettings.Username;
                record.conclusao_planta_fachada = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("conclusao_planta_corte_elevacao"))
            {
                record.planta_corte_elevacao_concluida_por = _dataBaseSettings.Username;
                record.data_conclusao_planta_corte_elevacao = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }
            else if (columnName.Equals("conclusao_planta_as_built"))
            {
                record.as_built_plantas = _dataBaseSettings.Username;
                record.as_built_plantas_data = DateTime.Now;
                await SalvarAsync(vm, record);
                dataGrid.Items.Refresh();
            }  
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}");
        }

    }

    private async void itens_RowValidated(object sender, GridViewRowValidatedEventArgs e)
    {
        try
        {
            var vm = (ViewPlantaViewModel)DataContext;
            if (e.Row?.Item is AprovadoModel aprovado)
            {
                await SalvarAsync(vm, aprovado);
            }
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Erro: {ex.InnerException.Message}");
        }
    }

    private static async Task SalvarAsync(ViewPlantaViewModel vm, AprovadoModel record)
    {
        var newEntity = new TAprovadoModel();
        MapperHelper.CopyMatchingProperties(record, newEntity);
        await vm.SaveAsync(newEntity);
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
