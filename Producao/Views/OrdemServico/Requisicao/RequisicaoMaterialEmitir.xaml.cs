using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Producao.Views.OrdemServico.Requisicao
{
    /// <summary>
    /// Interação lógica para RequisicaoMaterialEmitir.xam
    /// </summary>
    public partial class RequisicaoMaterialEmitir : UserControl
    {
        public RequisicaoMaterialEmitir()
        {
            DataContext = new RequisicaoMaterialEmitirViewModel();
            InitializeComponent();
        }

        private async void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    string text = ((TextBox)sender).Text;
                    RequisicaoMaterialEmitirViewModel vm = (RequisicaoMaterialEmitirViewModel)DataContext;
                    //vm.ProdutoServico = await vm.GetProdutoServicoAsync(long.Parse(text));
                    vm.TGlobal = await vm.GetGlobalAsync(long.Parse(text));
                    if (vm.TGlobal == null) //descricao_setor = "TODOS - TODOS"
                    {
                        MessageBox.Show("Número de serviço não encontrado", "Busca de número de serviço");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    txtData.Text = DateTime.Now.ToString("MM/dd/yyyy");
                    txtEmitente.Text = Environment.UserName;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                RequisicaoMaterialEmitirViewModel vm = (RequisicaoMaterialEmitirViewModel)DataContext;
                //var requisicao = await vm.SaveRequisicaoAsync(new RequisicaoModel { num_os_servico = vm.ProdutoServico.num_os_servico, data = DateTime.Now, alterado_por = Environment.UserName});
                var requisicao = await vm.SaveRequisicaoAsync(new RequisicaoModel { num_os_servico = vm.TGlobal.num_os, data = DateTime.Now, alterado_por = Environment.UserName});
                //RequisicaoMaterial detailsWindow = new RequisicaoMaterial(vm.ProdutoServico); //ProdutoServico
                RequisicaoMaterial detailsWindow = new RequisicaoMaterial(vm.TGlobal); //ProdutoServico
                detailsWindow.Owner = Window.GetWindow((DependencyObject)sender);  //(Window)obj;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                detailsWindow.Width = 800;
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class RequisicaoMaterialEmitirViewModel : INotifyPropertyChanged
    {
        static RequisicaoMaterialEmitirViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private ProdutoServicoModel _produtoServico;
        public ProdutoServicoModel ProdutoServico
        {
            get { return _produtoServico; }
            set { _produtoServico = value; RaisePropertyChanged("ProdutoServico"); }
        }

        private TGlobalModel _tGlobal;
        public TGlobalModel TGlobal
        {
            get { return _tGlobal; }
            set { _tGlobal = value; RaisePropertyChanged("TGlobal"); }
        }

        public async Task<ProdutoServicoModel> GetProdutoServicoAsync(long num_os_servico)
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    SELECT *
                    FROM producao.tbl_produtos_servico
                    WHERE num_os_servico = @num_os_servico
                    LIMIT 1;
                    """;

                return await conn.QueryFirstOrDefaultAsync<ProdutoServicoModel>(sql, new { num_os_servico });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TGlobalModel> GetGlobalAsync(long num_os_servico)
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    SELECT *
                    FROM ht.t_global
                    WHERE num_os = @num_os_servico
                    LIMIT 1;
                    """;

                return await conn.QueryFirstOrDefaultAsync<TGlobalModel>(sql, new { num_os_servico });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RequisicaoModel> SaveRequisicaoAsync(RequisicaoModel? requisicao)
        {
            try
            {
                if (requisicao is null)
                    throw new ArgumentNullException(nameof(requisicao));

                await using var conn = CreateConnection();

                if (requisicao.num_requisicao is null or 0)
                {
                    const string insertSql = """
                        INSERT INTO producao.t_requisicao
                            (num_os_servico, data, alterado_por, concluida)
                        VALUES
                            (@num_os_servico, @data, @alterado_por, @concluida)
                        RETURNING num_requisicao;
                        """;

                    requisicao.num_requisicao = await conn.ExecuteScalarAsync<long>(insertSql, requisicao);
                }
                else
                {
                    const string updateSql = """
                        UPDATE producao.t_requisicao
                        SET num_os_servico = @num_os_servico,
                            data = @data,
                            alterado_por = @alterado_por,
                            concluida = @concluida
                        WHERE num_requisicao = @num_requisicao;
                        """;

                    await conn.ExecuteAsync(updateSql, requisicao);
                }

                return requisicao;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}

