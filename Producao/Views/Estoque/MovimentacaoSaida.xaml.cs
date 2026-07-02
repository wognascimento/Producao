using Npgsql;
using Producao.DataBase.Model.Dto;
using Producao.Views.PopUp;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Estoque;

public partial class MovimentacaoSaida : UserControl
{
    public MovimentacaoSaida()
    {
        InitializeComponent();
        DataContext = new MovimentacaoSaidaViewModel();
    }

    private MovimentacaoSaidaViewModel ViewModel => (MovimentacaoSaidaViewModel)DataContext;

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await ExecutarAsync(ViewModel.CarregarPlanilhasAsync);
    }

    private async void OnBuscaProduto(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        await ExecutarAsync(async () =>
        {
            if (!long.TryParse(tbCodproduto.Text, out var codigo))
                throw new InvalidOperationException("Informe um codigo de produto valido.");

            ViewModel.Descricao = await ViewModel.GetDescricaoAsync(codigo);
            if (ViewModel.Descricao is null)
                throw new InvalidOperationException("Produto nao encontrado.");

            PreencherProduto(ViewModel.Descricao);
        });
    }

    private void OnOpenDescricoes(object sender, RoutedEventArgs e)
    {
        var window = new BuscaProduto { Owner = Application.Current.MainWindow };
        if (window.ShowDialog() != true || window.descricao is null)
            return;

        ViewModel.Descricao = window.descricao;
        PreencherProduto(window.descricao);
    }

    private void PreencherProduto(QryDescricao descricao)
    {
        tbCodproduto.Text = descricao.codcompladicional?.ToString() ?? string.Empty;
        txtPlanilha.Text = descricao.planilha ?? string.Empty;
        txtDescricao.Text = descricao.descricao ?? string.Empty;
        txtDescricaoAdicional.Text = descricao.descricao_adicional ?? string.Empty;
        txtComplementoAdicional.Text = descricao.complementoadicional ?? string.Empty;
        txtQuantidade.Focus();
    }

    private async void OnSelectedPlanilha(object sender, SelectionChangedEventArgs e)
    {
        if (txtPlanilha.SelectedItem is not RelplanModel planilha)
            return;

        await ExecutarAsync(async () =>
        {
            ViewModel.Produtos = [];
            ViewModel.DescAdicionais = [];
            ViewModel.CompleAdicionais = [];
            txtDescricao.SelectedItem = null;
            txtDescricaoAdicional.SelectedItem = null;
            txtComplementoAdicional.SelectedItem = null;

            await ViewModel.CarregarProdutosAsync(planilha.planilha);
            await ViewModel.CarregarItensAsync(planilha.planilha);
            txtDescricao.Focus();
        });
    }

    private async void OnSelectedDescricao(object sender, SelectionChangedEventArgs e)
    {
        if (txtDescricao.SelectedItem is not ProdutoModel produto)
            return;

        await ExecutarAsync(async () =>
        {
            ViewModel.DescAdicionais = [];
            ViewModel.CompleAdicionais = [];
            txtDescricaoAdicional.SelectedItem = null;
            txtComplementoAdicional.SelectedItem = null;
            await ViewModel.CarregarDescricoesAdicionaisAsync(produto.codigo);
            txtDescricaoAdicional.Focus();
        });
    }

    private async void OnSelectedDescricaoAdicional(object sender, SelectionChangedEventArgs e)
    {
        if (txtDescricaoAdicional.SelectedItem is not TabelaDescAdicionalModel adicional)
            return;

        await ExecutarAsync(async () =>
        {
            ViewModel.CompleAdicionais = [];
            txtComplementoAdicional.SelectedItem = null;
            await ViewModel.CarregarComplementosAsync(adicional.coduniadicional);
            txtComplementoAdicional.Focus();
        });
    }

    private void OnSelectedComplementoAdicional(object sender, SelectionChangedEventArgs e)
    {
        if (txtComplementoAdicional.SelectedItem is not TblComplementoAdicionalModel complemento)
            return;

        ViewModel.Compledicional = complemento;
        tbCodproduto.Text = complemento.codcompladicional?.ToString() ?? string.Empty;
        txtQuantidade.Focus();
    }

    private async void OnAdicionarClick(object sender, RoutedEventArgs e)
    {
        await ExecutarAsync(async () =>
        {
            if (!double.TryParse(txtQuantidade.Text, out var quantidade) || quantidade <= 0)
                throw new InvalidOperationException("Informe uma quantidade valida.");
            if (!long.TryParse(tbCodproduto.Text, out var produto))
                throw new InvalidOperationException("Informe um produto valido.");
            if (procedencias.SelectedItem is not string operacao)
                throw new InvalidOperationException("Selecione o destino.");

            var movimentacao = new SaidaEstoqueModel
            {
                quantidade = quantidade,
                destino = operacao,
                saida_data = DateTime.Now,
                saida_por = Environment.UserName,
                codcompladicional = produto,
                processado = processamento.IsChecked == true ? "-1" : "0"
            };

            if (operacao == "ACERTO ESTOQUE" &&
                await ViewModel.GetBloqueioAsync(produto) is not null)
            {
                throw new InvalidOperationException("PRODUTO BLOQUEADO PARA ACERTO DE ESTOQUE.");
            }

            await ViewModel.SaveAsync(movimentacao);
            if (operacao == "ACERTO ESTOQUE")
            {
                await ViewModel.SaveAcertoAsync(new ControleAcertoEstoque
                {
                    cod_movimentacao = movimentacao.codigo_saida,
                    processado = movimentacao.processado,
                    codcompladicional = produto,
                    quantidade = quantidade,
                    data = DateTime.Now,
                    hora = DateTime.Now.TimeOfDay,
                    operacao = operacao,
                    processo = "SAIDA",
                    incluido_por = Environment.UserName,
                    incluido_data = DateTime.Now,
                    bloqueado = "-1"
                });
            }

            var item = await ViewModel.GetItemAsync(movimentacao.codigo_saida);
            if (item is not null)
                ViewModel.Itens.Insert(0, item);

            txtQuantidade.Clear();
            txtQuantidade.Focus();
        });
    }

    private void itens_RowValidating(object sender, GridViewRowValidatingEventArgs e)
    {
        if (e.EditOperationType == GridViewEditOperationType.None ||
            e.Row.Item is not SaidaDTO item ||
            item.destino != "ACERTO ESTOQUE")
        {
            return;
        }

        e.IsValid = false;
        e.ValidationResults.Add(new GridViewCellValidationResult
        {
            ErrorMessage = "Nao e possivel alterar uma movimentacao de ACERTO ESTOQUE.",
            PropertyName = nameof(item.quantidade)
        });
    }

    private async void itens_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
    {
        if (e.EditAction != GridViewEditAction.Commit || e.EditedItem is not SaidaDTO item)
            return;

        await ExecutarAsync(async () =>
        {
            await ViewModel.UpdateAsync(item);
            item.saida_por = Environment.UserName;
            item.saida_data = DateTime.Now;
            itens.Rebind();
        });
    }

    private static async Task ExecutarAsync(Func<Task> action)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await action();
        }
        catch (PostgresException ex)
        {
            Producao.ErrorDialog.Show(ex, "Erro do banco");
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

internal sealed class MovimentacaoSaidaViewModel : MovimentacaoEstoqueViewModelBase<SaidaDTO>
{
    public MovimentacaoSaidaViewModel()
    {
        Procedencias = ["ACERTO ESTOQUE", "ACERTO INCENDIO", "TRANSFORMACAO", "DESCARTE", "DESCARTE CUPIM", "MARKETING", "KIT SOLUCAO", "ACERTO DE REQUISICAO", "INVENTARIO ROTATIVO"];
    }

    public async Task CarregarItensAsync(string? planilha) =>
        Itens = new ObservableCollection<SaidaDTO>(await Service.GetSaidasAsync(planilha));

    public Task<SaidaDTO?> GetItemAsync(long? codigo) => Service.GetSaidaAsync(codigo);
    public Task SaveAsync(SaidaEstoqueModel item) => Service.SaveSaidaAsync(item);
    public Task UpdateAsync(SaidaDTO item) => Service.UpdateSaidaAsync(item);
}
