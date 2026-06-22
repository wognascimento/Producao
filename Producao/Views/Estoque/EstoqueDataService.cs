using Dapper;
using Npgsql;
using Producao.DataBase.Model.Dto;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Producao.Views.Estoque;

internal sealed class EstoqueDataService
{
    private static NpgsqlConnection CreateConnection() =>
        new(DataBaseSettings.Instance.ConnectionString);

    public async Task<IEnumerable<RelplanModel>> GetPlanilhasAsync()
    {
        const string sql = "SELECT * FROM producao.relplan WHERE ativo = '1' ORDER BY planilha;";
        await using var connection = CreateConnection();
        return await connection.QueryAsync<RelplanModel>(sql);
    }

    public async Task<IEnumerable<ProdutoModel>> GetProdutosAsync(string? planilha)
    {
        const string sql = """
            SELECT * FROM producao.produtos
            WHERE planilha = @planilha AND COALESCE(inativo, '0') <> '-1'
            ORDER BY descricao;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryAsync<ProdutoModel>(sql, new { planilha });
    }

    public async Task<IEnumerable<TabelaDescAdicionalModel>> GetDescricoesAdicionaisAsync(long? codigo)
    {
        const string sql = """
            SELECT * FROM producao.tabela_desc_adicional
            WHERE codigoproduto = @codigo AND COALESCE(inativo, '0') <> '-1'
            ORDER BY descricao_adicional;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryAsync<TabelaDescAdicionalModel>(sql, new { codigo });
    }

    public async Task<IEnumerable<TblComplementoAdicionalModel>> GetComplementosAsync(long? codigo)
    {
        const string sql = """
            SELECT * FROM producao.tblcomplementoadicional
            WHERE coduniadicional = @codigo AND COALESCE(inativo, '0') <> '-1'
            ORDER BY complementoadicional;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryAsync<TblComplementoAdicionalModel>(sql, new { codigo });
    }

    public async Task<QryDescricao?> GetDescricaoAsync(long codigo)
    {
        const string sql = """
            SELECT * FROM producao.qry3descricoes
            WHERE codcompladicional = @codigo AND inativo = '0'
            LIMIT 1;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<QryDescricao>(sql, new { codigo });
    }

    public async Task<IEnumerable<EntradaDTO>> GetEntradasAsync(string? planilha)
    {
        const string sql = """
            SELECT entrada.codigo_entrada, entrada.codcompladicional, entrada.quantidade,
                   entrada.procedencia, entrada.processado, entrada.entrada_data,
                   entrada.entrada_por, descricao.descricao_completa, descricao.unidade
            FROM producao.t_entrada_estoque entrada
            JOIN producao.qry3descricoes descricao
              ON descricao.codcompladicional = entrada.codcompladicional
            WHERE descricao.planilha = @planilha
            ORDER BY entrada.codigo_entrada DESC;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryAsync<EntradaDTO>(sql, new { planilha });
    }

    public async Task<EntradaDTO?> GetEntradaAsync(long? codigo)
    {
        const string sql = """
            SELECT entrada.codigo_entrada, entrada.codcompladicional, entrada.quantidade,
                   entrada.procedencia, entrada.processado, entrada.entrada_data,
                   entrada.entrada_por, descricao.descricao_completa, descricao.unidade
            FROM producao.t_entrada_estoque entrada
            JOIN producao.qry3descricoes descricao
              ON descricao.codcompladicional = entrada.codcompladicional
            WHERE entrada.codigo_entrada = @codigo;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<EntradaDTO>(sql, new { codigo });
    }

    public async Task<IEnumerable<SaidaDTO>> GetSaidasAsync(string? planilha)
    {
        const string sql = """
            SELECT saida.codigo_saida, saida.codcompladicional, saida.quantidade,
                   saida.destino, saida.processado, saida.saida_data,
                   saida.saida_por, descricao.descricao_completa, descricao.unidade
            FROM producao.t_saida saida
            JOIN producao.qry3descricoes descricao
              ON descricao.codcompladicional = saida.codcompladicional
            WHERE descricao.planilha = @planilha
            ORDER BY saida.codigo_saida DESC;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryAsync<SaidaDTO>(sql, new { planilha });
    }

    public async Task<SaidaDTO?> GetSaidaAsync(long? codigo)
    {
        const string sql = """
            SELECT saida.codigo_saida, saida.codcompladicional, saida.quantidade,
                   saida.destino, saida.processado, saida.saida_data,
                   saida.saida_por, descricao.descricao_completa, descricao.unidade
            FROM producao.t_saida saida
            JOIN producao.qry3descricoes descricao
              ON descricao.codcompladicional = saida.codcompladicional
            WHERE saida.codigo_saida = @codigo;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<SaidaDTO>(sql, new { codigo });
    }

    public async Task SaveEntradaAsync(EntradaEstoqueModel entrada)
    {
        const string insertSql = """
            INSERT INTO producao.t_entrada_estoque
                (quantidade, procedencia, entrada_data, entrada_por, codcompladicional,
                 local_galpao, endereco, quantidade_fisica, processado)
            VALUES
                (@quantidade, @procedencia, @entrada_data, @entrada_por, @codcompladicional,
                 @local_galpao, @endereco, @quantidade_fisica, @processado)
            RETURNING codigo_entrada;
            """;
        await using var connection = CreateConnection();
        entrada.codigo_entrada = await connection.ExecuteScalarAsync<long>(insertSql, entrada);
    }

    public async Task SaveSaidaAsync(SaidaEstoqueModel saida)
    {
        const string insertSql = """
            INSERT INTO producao.t_saida
                (quantidade, destino, saida_data, saida_por, observacao, codcompladicional,
                 local_galpao, num_requisicao, caminho, endereco, quantidade_fisica, processado)
            VALUES
                (@quantidade, @destino, @saida_data, @saida_por, @observacao, @codcompladicional,
                 @local_galpao, @num_requisicao, @caminho, @endereco, @quantidade_fisica, @processado)
            RETURNING codigo_saida;
            """;
        await using var connection = CreateConnection();
        saida.codigo_saida = await connection.ExecuteScalarAsync<long>(insertSql, saida);
    }

    public async Task UpdateEntradaAsync(EntradaDTO entrada)
    {
        const string sql = """
            UPDATE producao.t_entrada_estoque
            SET quantidade = @quantidade, entrada_por = @usuario, entrada_data = @data
            WHERE codigo_entrada = @codigo_entrada;
            """;
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            entrada.codigo_entrada,
            entrada.quantidade,
            usuario = System.Environment.UserName,
            data = System.DateTime.Now
        });
    }

    public async Task UpdateSaidaAsync(SaidaDTO saida)
    {
        const string sql = """
            UPDATE producao.t_saida
            SET quantidade = @quantidade, saida_por = @usuario, saida_data = @data
            WHERE codigo_saida = @codigo_saida;
            """;
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            saida.codigo_saida,
            saida.quantidade,
            usuario = System.Environment.UserName,
            data = System.DateTime.Now
        });
    }

    public async Task<ControleAcertoEstoque?> GetBloqueioAsync(long? produto)
    {
        const string sql = """
            SELECT * FROM producao.tbl_controle_acerto_estoque
            WHERE codcompladicional = @produto AND bloqueado = '-1'
            LIMIT 1;
            """;
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ControleAcertoEstoque>(sql, new { produto });
    }

    public async Task SaveAcertoAsync(ControleAcertoEstoque acerto)
    {
        const string sql = """
            INSERT INTO producao.tbl_controle_acerto_estoque
                (cod_movimentacao, processado, codcompladicional, quantidade, data, hora,
                 operacao, processo, local, incluido_por, incluido_data, bloqueado)
            VALUES
                (@cod_movimentacao, @processado, @codcompladicional, @quantidade, @data, @hora,
                 @operacao, @processo, @local, @incluido_por, @incluido_data, @bloqueado)
            RETURNING codigo;
            """;
        await using var connection = CreateConnection();
        acerto.codigo = await connection.ExecuteScalarAsync<long>(sql, acerto);
    }
}

internal abstract class MovimentacaoEstoqueViewModelBase<TItem> : INotifyPropertyChanged
{
    protected readonly EstoqueDataService Service = new();
    private ObservableCollection<RelplanModel> planilhas = [];
    private ObservableCollection<ProdutoModel> produtos = [];
    private ObservableCollection<TabelaDescAdicionalModel> descAdicionais = [];
    private ObservableCollection<TblComplementoAdicionalModel> compleAdicionais = [];
    private ObservableCollection<TItem> itens = [];

    public ObservableCollection<RelplanModel> Planilhas
    {
        get => planilhas;
        set => SetField(ref planilhas, value);
    }

    public ObservableCollection<ProdutoModel> Produtos
    {
        get => produtos;
        set => SetField(ref produtos, value);
    }

    public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
    {
        get => descAdicionais;
        set => SetField(ref descAdicionais, value);
    }

    public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
    {
        get => compleAdicionais;
        set => SetField(ref compleAdicionais, value);
    }

    public ObservableCollection<TItem> Itens
    {
        get => itens;
        set => SetField(ref itens, value);
    }

    public QryDescricao? Descricao { get; set; }
    public TblComplementoAdicionalModel? Compledicional { get; set; }
    public ObservableCollection<string> Procedencias { get; protected set; } = [];

    public async Task CarregarPlanilhasAsync() =>
        Planilhas = new ObservableCollection<RelplanModel>(await Service.GetPlanilhasAsync());

    public async Task CarregarProdutosAsync(string? planilha) =>
        Produtos = new ObservableCollection<ProdutoModel>(await Service.GetProdutosAsync(planilha));

    public async Task CarregarDescricoesAdicionaisAsync(long? produto) =>
        DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>(
            await Service.GetDescricoesAdicionaisAsync(produto));

    public async Task CarregarComplementosAsync(long? adicional) =>
        CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>(
            await Service.GetComplementosAsync(adicional));

    public Task<QryDescricao?> GetDescricaoAsync(long codigo) => Service.GetDescricaoAsync(codigo);
    public Task<ControleAcertoEstoque?> GetBloqueioAsync(long? produto) => Service.GetBloqueioAsync(produto);
    public Task SaveAcertoAsync(ControleAcertoEstoque acerto) => Service.SaveAcertoAsync(acerto);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;
        RaisePropertyChanged(propertyName);
    }
}
