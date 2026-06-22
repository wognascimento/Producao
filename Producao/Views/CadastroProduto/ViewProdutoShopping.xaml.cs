using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.CadastroProduto;

/// <summary>
/// Interação lógica para ViewProdutoShopping.xam
/// </summary>
public partial class ViewProdutoShopping : UserControl
{
    static ViewProdutoShopping()
    {
        SqlMapper.AddTypeHandler(new DateOnlyToDateTimeHandler());
    }

    public ViewProdutoShopping()
    {
        InitializeComponent();
        DataContext = new ViewProdutoShoppingViewModel();
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            ViewProdutoShoppingViewModel vm = (ViewProdutoShoppingViewModel)DataContext;

            await vm.InserirCustosAdicionaisAsync();
            vm.Produtos = await vm.GetProdutosAsync();


            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (NpgsqlException ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
    }

    private async void OnRowValidated(object sender, GridViewRowValidatedEventArgs e)
    {
        ViewProdutoShoppingViewModel vm = (ViewProdutoShoppingViewModel)DataContext;
        try
        {
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
            if (e.Row?.Item is ProdutoShoppingModel data)
            {
                vm.Produto = await vm.AddProdutoAsync(data);
            }
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (NpgsqlException ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }

    }
}

public class ViewProdutoShoppingViewModel : INotifyPropertyChanged
{
    readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;

    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string propName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    private ObservableCollection<ProdutoShoppingModel> _produtos;
    public ObservableCollection<ProdutoShoppingModel> Produtos
    {
        get { return _produtos; }
        set { _produtos = value; RaisePropertyChanged("Produtos"); }
    }
    private ProdutoShoppingModel _produto;
    public ProdutoShoppingModel Produto
    {
        get { return _produto; }
        set { _produto = value; RaisePropertyChanged("Produto"); }
    }

    public async Task InserirCustosAdicionaisAsync()
    {
        try
        {
            using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
            await conn.ExecuteAsync(@"
                    INSERT INTO comercial.tblcustodescadicional (codcompladicional, tipocusto)
                    SELECT c.codcompladicional, 'ESTIMADO'
                    FROM producao.tblcomplementoAdicional c
                    LEFT JOIN comercial.tblcustodescadicional d 
                        ON c.codcompladicional = d.codcompladicional
                    WHERE d.codcompladicional IS NULL;
                ");
        }
        catch (PostgresException)
        {
            throw;
        }


    }

    public async Task<ObservableCollection<ProdutoShoppingModel>> GetProdutosAsync()
    {
        try
        {
            using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
            var data = await conn.QueryAsync<ProdutoShoppingModel>(
                @"SELECT *
                      FROM producao.view_produto_shopping
                      WHERE ncm IS NULL
                         OR peso IS NULL
                         OR peso = 0
                         OR custo IS NULL
                         OR custo = 0;");
            return new ObservableCollection<ProdutoShoppingModel>(data);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ProdutoShoppingModel> AddProdutoAsync(ProdutoShoppingModel produto)
    {
        try
        {
            using var conn = new NpgsqlConnection(BaseSettings.ConnectionString);
            await conn.ExecuteAsync(
                @"UPDATE producao.tblcomplementoadicional
                      SET ncm = @ncm,
                          peso = @peso
                      WHERE codcompladicional = @codcompladicional;

                      UPDATE comercial.tblcustodescadicional
                      SET custo = @custo,
                          alteradopor = @alteradopor,
                          dataaltera = CURRENT_DATE
                      WHERE codcompladicional = @codcompladicional
                        AND tipocusto = 'ESTIMADO';",
                new
                {
                    produto.codcompladicional,
                    produto.ncm,
                    produto.peso,
                    produto.custo,
                    alteradopor = Environment.UserName
                });
            return produto;
        }
        catch (NpgsqlException)
        {
            throw;
        }
    }

}

public class DateOnlyToDateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override DateTime Parse(object value)
    {
        return value switch
        {
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            DateTime dateTime => dateTime,
            _ => Convert.ToDateTime(value)
        };
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
    }
}
