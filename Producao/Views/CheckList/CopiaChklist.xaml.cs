using CommunityToolkit.Mvvm.ComponentModel;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Producao.DataBase.Model.Dto;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace Producao.Views.CheckList;

/// <summary>
/// Interação lógica para CopiaChklist.xam
/// </summary>
public partial class CopiaChklist : UserControl
{
    private SiglaChkListModel siglaChk;

    public CopiaChklist(SiglaChkListModel _siglaChk)
    {
        InitializeComponent();
        DataContext = new CopiaChklistViewModel();
        this.Loaded += CopiaChklist_Loaded;
        this.siglaChk = _siglaChk;
    }

    private async void CopiaChklist_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            CopiaChklistViewModel vm = (CopiaChklistViewModel)DataContext;

            await vm.CarregarSiglas();

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            MessageBox.Show($"Erro do banco: {pgEx.MessageText}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }

    private async void rcbSiglas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            CopiaChklistViewModel vm = (CopiaChklistViewModel)DataContext;

            var sigla = (rcbSiglas.SelectedItem as string);

            await vm.CarregarAnos(sigla);

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            MessageBox.Show($"Erro do banco: {pgEx.MessageText}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }

    private async void rcbAnos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            CopiaChklistViewModel vm = (CopiaChklistViewModel)DataContext;

            var sigla = rcbSiglas.SelectedItem as string;
            int ano = (int)rcbAnos.SelectedItem;
            rgViewItens.IsBusy = true;
            await vm.CarregarItens(sigla, ano);
            rgViewItens.IsBusy = false;
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            MessageBox.Show($"Erro do banco: {pgEx.MessageText}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        
        try
        {
            CopiaChklistViewModel vm = (CopiaChklistViewModel)DataContext;
            ObservableCollection<HistoricoCheckListDTO> itens = new([.. rgViewItens.Items.Cast<HistoricoCheckListDTO>()]);

            if (itens.Count == 0)
            {
                MessageBox.Show("Nenhum item para copiar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirma = MessageBox.Show($"Confirma a cópia de {itens.Count} itens para a sigla {siglaChk.sigla_serv}?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirma != MessageBoxResult.Yes)
                return;

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            await vm.CopiarItens(itens, siglaChk);
            MessageBox.Show("Itens copiados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            MessageBox.Show($"Erro do banco: {pgEx.MessageText}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }


    }
}

public partial class CopiaChklistViewModel : ObservableObject
{
    private readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;


    [ObservableProperty]
    private ObservableCollection<string> siglas = [];

    [ObservableProperty]
    private ObservableCollection<int> anos = [];

    [ObservableProperty]
    private ObservableCollection<HistoricoCheckListDTO> itens = [];

    public async Task CarregarSiglas()
    {
        const string sql = @"
            SELECT sigla FROM producao.view_checklist_completo_historico GROUP BY sigla ORDER BY sigla;
        ";

        await using var conn = new NpgsqlConnection(BaseSettings.connectionString);
        await conn.OpenAsync();
        Siglas = new ObservableCollection<string>( await conn.QueryAsync<string>(sql) );
    }

    public async Task CarregarAnos(string sigla)
    {
        const string sql = @"
            SELECT ano FROM producao.view_checklist_completo_historico WHERE sigla = @Sigla GROUP BY ano ORDER BY ano DESC;
        ";

        await using var conn = new NpgsqlConnection(BaseSettings.connectionString);
        await conn.OpenAsync();
        Anos = new ObservableCollection<int>(await conn.QueryAsync<int>(sql, new { Sigla = sigla }));
    }


    public async Task CarregarItens(string sigla, int ano)
    {
        const string sql = @"
            SELECT ano, sigla, tema, codcompl, ordem, item_memorial, local_shoppings, obs, orient_montagem, orient_desmont, codproduto, coduniadicional, planilha, qtd_chk, coddetalhescompl, codcompladicional, descricao_completa, qtd_comple
	        FROM producao.view_checklist_completo_historico
            WHERE sigla = @Sigla AND ano = @Ano;
        ";

        await using var conn = new NpgsqlConnection(BaseSettings.connectionString);
        await conn.OpenAsync();
        Itens = new ObservableCollection<HistoricoCheckListDTO>(await conn.QueryAsync<HistoricoCheckListDTO>(sql, new { Sigla = sigla, Ano = ano }));
    }

    public async Task CopiarItens(ObservableCollection<HistoricoCheckListDTO> itens, SiglaChkListModel siglaChk)
    {
        using var conn = new NpgsqlConnection(BaseSettings.connectionString);
        await conn.OpenAsync();
        using var tran = conn.BeginTransaction();
        try
        {
            var itensChk = new ObservableCollection<HistoricoCheckListDTO>([.. itens.DistinctBy(x => new { x.ano, x.sigla, x.tema, x.codcompl, x.ordem, x.item_memorial, x.local_shoppings, x.codproduto, x.coduniadicional, x.qtd_chk })]);
            foreach (var chk in itensChk)
            {
                // INSERT na tabela pai
                var codcompl = await conn.ExecuteScalarAsync<int>(
                    @"INSERT INTO producao.t_complemento_chk(
                        ordem, sigla, local_shoppings, codproduto, item_memorial, qtd, coduniadicional,
                        id_aprovado, inserido_por, inserido_em)
                      VALUES (@ordem, @sigla, @local_shoppings, @codproduto, @item_memorial, @qtd_chk, @coduniadicional,
                        @id_aprovado, @inserido_por, NOW())
                      RETURNING codcompl;",
                    new
                    {
                        chk.ordem,
                        sigla = siglaChk.sigla_serv,
                        chk.local_shoppings,
                        chk.codproduto,
                        chk.item_memorial,
                        chk.qtd_chk,
                        chk.coduniadicional,
                        siglaChk.id_aprovado, // exemplo
                        inserido_por = BaseSettings.Username
                    },
                    transaction: tran
                );

                // Agora insere os "detalhes" (filhos)
                var itensComple = itens
                    .Where(x => x.codcompl == chk.codcompl)
                    .DistinctBy(x => new { x.coddetalhescompl, x.codcompladicional, x.qtd_comple })
                    .ToList();

                foreach (var item in itensComple)
                {
                    await conn.ExecuteAsync(
                        @"INSERT INTO producao.tbldetalhescomplemento(
                            codcompladicional, qtd, codcompl,
                            data_inserido, inserido_por, local_producao)
                          VALUES (@codcompladicional, @qtd, @codcompl,
                            NOW(), @inserido_por, @local_producao);",
                        new
                        {
                            item.codcompladicional,
                            qtd = item.qtd_comple,
                            codcompl,
                            inserido_por = BaseSettings.Username,
                            local_producao = "JACAREÍ"
                        },
                        transaction: tran
                    );
                }
            }

            await tran.CommitAsync();
        }
        catch (Exception ex)
        {
            await tran.RollbackAsync();
            throw new Exception("Erro ao inserir histórico de checklist", ex);
        }

    }
}