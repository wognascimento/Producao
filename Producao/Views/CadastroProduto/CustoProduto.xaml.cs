using Dapper;
using Microsoft.EntityFrameworkCore;
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
            vm.Produtos = await Task.Run(vm.GetProdutosAsync);

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }

    private async void itens_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
    {
        try
        {
            CustoProdutoViewModel vm = (CustoProdutoViewModel)DataContext;
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            await vm.AtualizarCustoProdutoAsync((CustoProdutoDTO)e.RowData);
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
        try
        {
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
                    custo25, 
                    custo24, 
                    custo23, 
                    custo_rec23, 
                    tipocusto, 
                    custo_rec25, 
                    custo_rec24, 
                    process_25, 
                    process_24, 
                    process_23, 
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
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao buscar os produtos", ex);
        }
    }

    public async Task AtualizarCustoProdutoAsync(CustoProdutoDTO custoDTO)
    {
        try
        {
            using DatabaseContext db = new();
            var custoExistente = await db.CustoDescs.FindAsync(custoDTO.codcompladicional);
            var custoNovo = custoExistente;
            custoNovo.codcompladicional = custoDTO.codcompladicional;
            custoNovo.tipocusto = custoDTO.tipocusto;
            custoNovo.custo = custoDTO.custo25;
            custoNovo.custo_recuperacao = custoDTO.custo_rec25;
            custoNovo.process = custoDTO.process_25;
            custoNovo.alteradopor = Environment.UserName;
            custoNovo.dataaltera = DateTime.Now.Date;

            if (custoExistente == null)
                db.CustoDescs.Add(custoNovo);
            else
                db.Entry(custoExistente).CurrentValues.SetValues(custoNovo);

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao atualizar custo do produto", ex);
        }
    }
}