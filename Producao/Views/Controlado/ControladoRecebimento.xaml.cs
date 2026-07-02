using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.Controlado
{
    /// <summary>
    /// Interação lógica para ControladoRecebimento.xam
    /// </summary>
    public partial class ControladoRecebimento : UserControl
    {
        public ControladoRecebimento()
        {
            InitializeComponent();
            DataContext = new ControladoRecebimentoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControladoRecebimentoViewModel vm = (ControladoRecebimentoViewModel)DataContext;
                vm.Produtos = await vm.GetProdutosAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void RadGridView_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            ControladoRecebimentoViewModel vm = (ControladoRecebimentoViewModel)DataContext;
            try
            {
                if (e.Row?.Item is not ControladoRetornoGeralModel data)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Retorno = new()
                {
                    id_aprovado = data.id_aprovado,
                    codcompladicional = data.codcompladicional,
                    qtd = data.retorno,
                    atualizado_por = Environment.UserName,
                    atualizado_em = DateTime.Now,
                };

                await vm.SaveRetornoAsync(vm.Retorno);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class ControladoRecebimentoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControladoRetornoGeralModel> _produtos;
        public ObservableCollection<ControladoRetornoGeralModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }

        private ControladoRetornoGeralModel _produto;
        public ControladoRetornoGeralModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private ControladoRecebidoModel _retorno;
        public ControladoRecebidoModel Retorno
        {
            get { return _retorno; }
            set { _retorno = value; RaisePropertyChanged("Retorno"); }
        }

        public async Task<ObservableCollection<ControladoRetornoGeralModel>> GetProdutosAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<ControladoRetornoGeralModel>(
                    @"SELECT *
                      FROM expedicao.qry_controlados_retorno_geral;");
                return new ObservableCollection<ControladoRetornoGeralModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveRetornoAsync(ControladoRecebidoModel m)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                await conn.ExecuteAsync(
                    @"INSERT INTO expedicao.t_controlados_recebidos
                        (id_aprovado, codcompladicional, qtd, atualizado_por, atualizado_em,
                         cancelar_cobraca, justificativa, entrada_estoque, entrada_estoque_por, entrada_estoque_em)
                      VALUES
                        (@id_aprovado, @codcompladicional, @qtd, @atualizado_por, @atualizado_em,
                         @cancelar_cobraca, @justificativa, @entrada_estoque, @entrada_estoque_por, @entrada_estoque_em)
                      ON CONFLICT (id_aprovado, codcompladicional) DO UPDATE SET
                        qtd = EXCLUDED.qtd,
                        atualizado_por = EXCLUDED.atualizado_por,
                        atualizado_em = EXCLUDED.atualizado_em,
                        cancelar_cobraca = EXCLUDED.cancelar_cobraca,
                        justificativa = EXCLUDED.justificativa,
                        entrada_estoque = EXCLUDED.entrada_estoque,
                        entrada_estoque_por = EXCLUDED.entrada_estoque_por,
                        entrada_estoque_em = EXCLUDED.entrada_estoque_em;",
                    m);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
