using Dapper;
using Npgsql;
using Producao.DataBase.Model.Dto;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.CadastroProduto;

/// <summary>
/// Interação lógica para CustoProduto.xam
/// </summary>
public partial class CustoProduto : UserControl
{
    public CustoProduto()
    {
        InitializeComponent();
        this.DataContext = new CustoProdutoViewModel();
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            CustoProdutoViewModel vm = (CustoProdutoViewModel)DataContext;
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            vm.Produtos = await vm.GetProdutosAsync();

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch(PostgresException ex)
        {
            MessageBox.Show($"Erro ao conectar com o banco de dados: {ex.Message}");
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
            CustoProdutoViewModel vm = (CustoProdutoViewModel)DataContext;
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            if (e.Row?.Item is CustoProdutoDTO data)
            {
                await vm.AtualizarCustoProdutoAsync(data);
            }
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }
}

public class CustoProdutoViewModel : INotifyPropertyChanged
{
    readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;

    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string propName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    private ObservableCollection<CustoProdutoDTO> produtos;
    public ObservableCollection<CustoProdutoDTO> Produtos
    {
        get { return produtos; }
        set { produtos = value; RaisePropertyChanged("Produtos"); }
    }
    private CustoProdutoDTO produto;
    public CustoProdutoDTO Produto
    {
        get { return produto; }
        set { produto = value; RaisePropertyChanged("Produto"); }
    }


    public async Task<ObservableCollection<CustoProdutoDTO>> GetProdutosAsync()
    {
        //try
        //{
            string sql = @"
                SELECT 
                    planilha, 
                    descricao, 
                    descricao_adicional, 
                    complementoadicional, 
                    codcompladicional, 
                    descricao_completa, 
                    unidade, 
                    vida_util, 
                    inativo, 
                    prodcontrolado, 
                    custo_atual, 
                    custo_anterior, 
                    custo_retrasado, 
                    tipocusto, 
                    custo_rec_atual, 
                    custo_rec_anterior, 
                    custo_rec_retrasado, 
                    process_atual, 
                    process_anterior, 
                    process_retrasado, 
                    media_pn_pro1_v, 
                    media_pn_pro2_v, 
                    media_rc_pro2_v, 
                    media_pro3_v, 
                    media_total_process
                FROM producao.qry_custo;
            ";

            using var connection = new NpgsqlConnection(BaseSettings.connectionString);
            await connection.OpenAsync();

            var resultado = (await connection.QueryAsync<CustoProdutoDTO>(sql)).ToList();
            return new ObservableCollection<CustoProdutoDTO>(resultado);
        //}
        //catch (Exception ex)
        //{
            //throw new Exception("Erro ao buscar os produtos", ex);
        //}
    }

    public async Task AtualizarCustoProdutoAsync(CustoProdutoDTO custoDTO)
    {
        try
        {
            const string sql = @"
                INSERT INTO comercial.tblcustodescadicional
                    (codcompladicional, tipocusto, custo, custo_recuperacao, process, alteradopor, dataaltera)
                VALUES
                    (@codcompladicional, @tipocusto, @custo_atual, @custo_rec_atual, @process_atual, @alteradopor, CURRENT_DATE)
                ON CONFLICT (codcompladicional) DO UPDATE SET
                    tipocusto = EXCLUDED.tipocusto,
                    custo = EXCLUDED.custo,
                    custo_recuperacao = EXCLUDED.custo_recuperacao,
                    process = EXCLUDED.process,
                    alteradopor = EXCLUDED.alteradopor,
                    dataaltera = EXCLUDED.dataaltera;";

            using var connection = new NpgsqlConnection(BaseSettings.ConnectionString);
            await connection.ExecuteAsync(sql, new
            {
                custoDTO.codcompladicional,
                custoDTO.tipocusto,
                custoDTO.custo_atual,
                custoDTO.custo_rec_atual,
                custoDTO.process_atual,
                alteradopor = Environment.UserName
            });
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao atualizar custo do produto", ex);
        }
    }
}
