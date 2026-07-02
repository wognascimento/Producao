using Dapper;
using Npgsql;
using Producao.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views;

public partial class ViewAprovado : UserControl
{
    public ObservableCollection<AprovadoModel> AprovadosList { get; set; }

    static ViewAprovado()
    {
        SqlMapper.AddTypeHandler(new DateOnlyToDateTimeHandler());
    }

    public ViewAprovado()
    {
        InitializeComponent();
        DataContext = new ViewAprovadoViewModel();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var vm = (ViewAprovadoViewModel)DataContext;
            Mouse.OverrideCursor = Cursors.Wait;
            vm.Aprovados = await vm.GetAprovados();
        }
        catch (Exception ex)
        {
            Producao.ErrorDialog.Show(ex, "Erro");
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private async void itens_RowValidated(object sender, GridViewRowValidatedEventArgs e)
    {
        if (e.Row.Item is not AprovadoModel record)
            return;

        try
        {
            var vm = (ViewAprovadoViewModel)DataContext;
            Mouse.OverrideCursor = Cursors.Wait;
            await vm.SaveAsync(record);
        }
        catch (Exception ex)
        {
            Producao.ErrorDialog.Show(ex, "Erro");
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
    }
}

public class ViewAprovadoViewModel : INotifyPropertyChanged
{
    private AprovadoModel _aprovado;
    public AprovadoModel Aprovado
    {
        get { return _aprovado; }
        set { _aprovado = value; RaisePropertyChanged(nameof(Aprovado)); }
    }

    private ObservableCollection<AprovadoModel> _aprovados;
    public ObservableCollection<AprovadoModel> Aprovados
    {
        get { return _aprovados; }
        set { _aprovados = value; RaisePropertyChanged(nameof(Aprovados)); }
    }

    private ObservableCollection<string> _respPlantaPca = ["ANA LOPES", "AMANDA LIMA", "CARLA MATTEUCCI", "CARLA ROSIN", "DANIELLE BRAGA", "ELDER SILVA", "JACK MELLOR", "MARIANA JESUS", "RENATA CELANTE", "RENATA LINS", "NÃO TEM", ""];
    public ObservableCollection<string> RespPlantaPca { get { return _respPlantaPca; } set { _respPlantaPca = value; RaisePropertyChanged(nameof(RespPlantaPca)); } }

    private ObservableCollection<string> _respPlantaBase = ["AMANDA LIMA", "CARLA ROSIN", "DANIELLE BRAGA", "MARIANA JESUS", "RENATA CELANTE", "RENATA LINS", "NÃO TEM", ""];
    public ObservableCollection<string> RespPlantaBase { get { return _respPlantaBase; } set { _respPlantaBase = value; RaisePropertyChanged(nameof(RespPlantaBase)); } }

    private ObservableCollection<string> _respPlantaMall = ["AMANDA LIMA", "DANIELLE BRAGA", "MARIANA JESUS", "RENATA CELANTE", "RENATA LINS", "NÃO TEM", ""];
    public ObservableCollection<string> RespPlantaMall { get { return _respPlantaMall; } set { _respPlantaMall = value; RaisePropertyChanged(nameof(RespPlantaMall)); } }

    private ObservableCollection<string> _respPlantaFachada = ["AMANDA LIMA", "DANIELLE BRAGA", "MARIANA JESUS", "RENATA CELANTE", "RENATA LINS", "NÃO TEM", ""];
    public ObservableCollection<string> RespPlantaFachada { get { return _respPlantaFachada; } set { _respPlantaFachada = value; RaisePropertyChanged(nameof(RespPlantaFachada)); } }

    public async Task<ObservableCollection<AprovadoModel>> GetAprovados()
    {
        var baseSettings = DataBaseSettings.Instance;
        await using var db = new NpgsqlConnection(baseSettings.ConnectionString);
        var data = await db.QueryAsync<AprovadoModel>(
            @"SELECT *
                  FROM producao.qry_aprovados
                  ORDER BY ordem;");

        return new ObservableCollection<AprovadoModel>(data);
    }

    public async Task SaveAsync(AprovadoModel aprovado)
    {
        if (aprovado?.id_aprovado is null)
            return;

        var baseSettings = DataBaseSettings.Instance;
        await using var db = new NpgsqlConnection(baseSettings.ConnectionString);
        await db.ExecuteAsync(
            @"UPDATE producao.t_aprovados SET
                    ordem = @ordem,
                    ordem_sigla_serv = @ordem_sigla_serv,
                    nivel = @nivel,
                    projeto_novo = @projeto_novo,
                    obs_especial = @obs_especial,
                    memo_resp = @memo_resp,
                    memo_data = @memo_data,
                    cronog_data = @cronog_data,
                    cronog_resp = @cronog_resp,
                    rel_inflamabilidade = @rel_inflamabilidade,
                    data_rel_inflamabilidade = @data_rel_inflamabilidade,
                    meta_rel_inflamabilidade = @meta_rel_inflamabilidade,
                    pa = @pa,
                    acabamento_construcao = @acabamento_construcao,
                    acabamento_fibra = @acabamento_fibra,
                    acabamento_moveis = @acabamento_moveis,
                    laco = @laco,
                    cor_predominante = @cor_predominante,
                    iluminacao = @mlamp_led,
                    obs_iluminacao = @obs_iluminacao,
                    resp_memo_visual = @resp_memo_visual,
                    data_memo_visual = @data_memo_visual,
                    data_reuniao_conceito = @data_reuniao_conceito,
                    resp_estruturas = @resp_estruturas,
                    alteradopor = @alteradoPor,
                    dataaltera = now()
                  WHERE id_aprovado = @id_aprovado;",
            new
            {
                aprovado.id_aprovado,
                aprovado.ordem,
                aprovado.ordem_sigla_serv,
                aprovado.nivel,
                aprovado.projeto_novo,
                aprovado.obs_especial,
                aprovado.memo_resp,
                aprovado.memo_data,
                aprovado.cronog_data,
                aprovado.cronog_resp,
                aprovado.rel_inflamabilidade,
                aprovado.data_rel_inflamabilidade,
                aprovado.meta_rel_inflamabilidade,
                aprovado.pa,
                aprovado.acabamento_construcao,
                aprovado.acabamento_fibra,
                aprovado.acabamento_moveis,
                aprovado.laco,
                aprovado.cor_predominante,
                aprovado.mlamp_led,
                aprovado.obs_iluminacao,
                aprovado.resp_memo_visual,
                aprovado.data_memo_visual,
                aprovado.data_reuniao_conceito,
                aprovado.resp_estruturas,
                alteradoPor = baseSettings.Username
            });
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string propName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
