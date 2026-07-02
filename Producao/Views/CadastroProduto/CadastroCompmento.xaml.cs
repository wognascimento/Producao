using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.CadastroProduto
{
    /// <summary>
    /// Interação lógica para CadastroCompmento.xam
    /// </summary>
    public partial class CadastroCompmento : Window
    {
        private readonly TabelaDescAdicionalModel produtoAdicional;

        public CadastroCompmento(TabelaDescAdicionalModel produtoAdicional)
        {
            DataContext = new CadastroCompmentoViewModel();
            InitializeComponent();
            this.produtoAdicional = produtoAdicional;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var vm = (CadastroCompmentoViewModel)DataContext;
                vm.Unidades = await vm.GetUnidadesAsync();
                vm.ComplementoAdicionais = await vm.GetComplementoAdicionaisAsync(produtoAdicional.coduniadicional);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            finally
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnAddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.NewObject = new TblComplementoAdicionalModel
            {
                coduniadicional = produtoAdicional.coduniadicional,
                inativo = "0",
                prodcontrolado = "0"
            };
        }

        private async void OnRowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            if (e.Row is GridViewNewRow || e.Row?.Item is not TblComplementoAdicionalModel data)
                return;

            var vm = (CadastroCompmentoViewModel)DataContext;
            var isInsert = data.codcompladicional is null or 0;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var last = vm.ComplementoAdicionais
                    .Where(x => x.codcompladicional != data.codcompladicional && x.coduniadicional == produtoAdicional.coduniadicional)
                    .LastOrDefault();

                data.coduniadicional = produtoAdicional.coduniadicional;
                data.estoque_inicial ??= 0;
                data.estoque_inicial_processado ??= 0;
                data.peso ??= last?.peso ?? 0;
                data.cadastradopor = isInsert ? Environment.UserName : data.cadastradopor;
                data.cadastradoem = isInsert ? DateTime.Now : data.cadastradoem;
                data.alterado_por = isInsert ? null : Environment.UserName;
                data.alterado_em = isInsert ? null : DateTime.Now;
                data.pesobruto ??= last?.pesobruto ?? 0;
                data.origemcusto ??= last?.origemcusto;
                data.contabil ??= last?.contabil;
                data.produto_novo ??= DateTime.Now.Year.ToString();
                data.contabil_pldc ??= last?.contabil_pldc;
                data.inativo = string.IsNullOrWhiteSpace(data.inativo) ? "0" : data.inativo;
                data.prodcontrolado = string.IsNullOrWhiteSpace(data.prodcontrolado) ? "0" : data.prodcontrolado;
                data.preco_shopping ??= last?.preco_shopping;
                data.saldo_estoque ??= 0;

                var saved = await vm.SaveAsync(data);
                data.codcompladicional = saved.codcompladicional;
                adicionais.Items.Refresh();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.ComplementoAdicionais.Where(x => x.codcompladicional is null or 0).ToList();
                foreach (var item in toRemove)
                    vm.ComplementoAdicionais.Remove(item);
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row is GridViewNewRow || e.Row?.Item is not TblComplementoAdicionalModel rowData)
                return;

            if (rowData.coduniadicional == null)
                AddValidation(e, nameof(TblComplementoAdicionalModel.codcompladicional), "Descrição adicional não selecionada");

            if (string.IsNullOrWhiteSpace(rowData.complementoadicional))
                AddValidation(e, nameof(TblComplementoAdicionalModel.complementoadicional), "Informe o COMPLEMENTO ADICIONAL");

            if (string.IsNullOrWhiteSpace(rowData.descricaofiscal))
                AddValidation(e, nameof(TblComplementoAdicionalModel.descricaofiscal), "Informe a DESCRIÇÃO FISCAL");

            if (string.IsNullOrWhiteSpace(rowData.unidade))
                AddValidation(e, nameof(TblComplementoAdicionalModel.unidade), "Informe a UNIDADE");
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = propertyName,
                ErrorMessage = message
            });
        }
    }

    public class CadastroCompmentoViewModel : INotifyPropertyChanged
    {
        private readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private TblComplementoAdicionalModel _complementoAdicional;
        public TblComplementoAdicionalModel ComplementoAdicional
        {
            get { return _complementoAdicional; }
            set { _complementoAdicional = value; RaisePropertyChanged(nameof(ComplementoAdicional)); }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _complementoAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> ComplementoAdicionais
        {
            get { return _complementoAdicionais; }
            set { _complementoAdicionais = value; RaisePropertyChanged(nameof(ComplementoAdicionais)); }
        }

        private UnidadeModel _unidade;
        public UnidadeModel Unidade
        {
            get { return _unidade; }
            set { _unidade = value; RaisePropertyChanged(nameof(Unidade)); }
        }

        private ObservableCollection<UnidadeModel> _unidades;
        public ObservableCollection<UnidadeModel> Unidades
        {
            get { return _unidades; }
            set { _unidades = value; RaisePropertyChanged(nameof(Unidades)); }
        }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(BaseSettings.ConnectionString);
        }

        public async Task<ObservableCollection<TblComplementoAdicionalModel>> GetComplementoAdicionaisAsync(long? coduniadicional)
        {
            using var conn = CreateConnection();
            var data = await conn.QueryAsync<TblComplementoAdicionalModel>(
                """
                SELECT *
                FROM producao.tblcomplementoadicional
                WHERE coduniadicional = @coduniadicional
                ORDER BY acompanhamento;
                """,
                new { coduniadicional });

            return new ObservableCollection<TblComplementoAdicionalModel>(data);
        }

        public async Task<ObservableCollection<UnidadeModel>> GetUnidadesAsync()
        {
            using var conn = CreateConnection();
            var data = await conn.QueryAsync<UnidadeModel>(
                """
                SELECT *
                FROM producao.unidades
                ORDER BY unidade;
                """);

            return new ObservableCollection<UnidadeModel>(data);
        }

        public async Task<TblComplementoAdicionalModel> SaveAsync(TblComplementoAdicionalModel complemento)
        {
            using var conn = CreateConnection();

            if (complemento.codcompladicional is null or 0)
            {
                complemento.codcompladicional = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO producao.tblcomplementoadicional
                        (complementoadicional, status, estoque_inicial, desc_process,
                         estoque_inicial_processado, altura, largura, profundidade, vida_util,
                         diametro, peso, unidade, cadastradopor, cadastradoem, custo_real,
                         prodcontrolado, volume, area, precolocacao, descricaofiscal,
                         descricaoespanhol, estoque_min, v_unit, v_unit_dolar, ncm, tipo,
                         custoestimado, indicecorrecao, nf, pesobruto, coduniadicional,
                         codfornecedor, foralinhafornecedor, origemcusto, datafichatecnica,
                         respfichatenica, datainiciofichatecnica, respcusto, datacusto,
                         contabil, produto_novo, acompanhamento, responsavel_acompanha,
                         concluido_acompanha, obs_acompanhamento, importado, contabil_pldc,
                         narrativa, alx, inativo, qtd_etiqueta, fracao, dividir_qtd_volume,
                         conta_aplica_contabil, centro_custo_contabil, especial, foto,
                         tamanho_construcao, diverso, dificuldade, saldo_patrimonial_ano_anterior,
                         saldo_disponivel_ano_anterior, custo_despesa, link_foto, preco_shopping,
                         exportado_folhamatic, saldo_estoque)
                    VALUES
                        (@complementoadicional, @status, @estoque_inicial, @desc_process,
                         @estoque_inicial_processado, @altura, @largura, @profundidade, @vida_util,
                         @diametro, @peso, @unidade, @cadastradopor, @cadastradoem, @custo_real,
                         @prodcontrolado, @volume, @area, @precolocacao, @descricaofiscal,
                         @descricaoespanhol, @estoque_min, @v_unit, @v_unit_dolar, @ncm, @tipo,
                         @custoestimado, @indicecorrecao, @nf, @pesobruto, @coduniadicional,
                         @codfornecedor, @foralinhafornecedor, @origemcusto, @datafichatecnica,
                         @respfichatenica, @datainiciofichatecnica, @respcusto, @datacusto,
                         @contabil, @produto_novo, @acompanhamento, @responsavel_acompanha,
                         @concluido_acompanha, @obs_acompanhamento, @importado, @contabil_pldc,
                         @narrativa, @alx, @inativo, @qtd_etiqueta, @fracao, @dividir_qtd_volume,
                         @conta_aplica_contabil, @centro_custo_contabil, @especial, @foto,
                         @tamanho_construcao, @diverso, @dificuldade, @saldo_patrimonial_ano_anterior,
                         @saldo_disponivel_ano_anterior, @custo_despesa, @link_foto, @preco_shopping,
                         @exportado_folhamatic, @saldo_estoque)
                    RETURNING codcompladicional;
                    """,
                    complemento);
            }
            else
            {
                await conn.ExecuteAsync(
                    """
                    UPDATE producao.tblcomplementoadicional
                    SET complementoadicional = @complementoadicional,
                        status = @status,
                        estoque_inicial = @estoque_inicial,
                        desc_process = @desc_process,
                        estoque_inicial_processado = @estoque_inicial_processado,
                        altura = @altura,
                        largura = @largura,
                        profundidade = @profundidade,
                        vida_util = @vida_util,
                        diametro = @diametro,
                        peso = @peso,
                        unidade = @unidade,
                        alterado_por = @alterado_por,
                        alterado_em = @alterado_em,
                        custo_real = @custo_real,
                        prodcontrolado = @prodcontrolado,
                        volume = @volume,
                        area = @area,
                        precolocacao = @precolocacao,
                        descricaofiscal = @descricaofiscal,
                        descricaoespanhol = @descricaoespanhol,
                        estoque_min = @estoque_min,
                        v_unit = @v_unit,
                        v_unit_dolar = @v_unit_dolar,
                        ncm = @ncm,
                        tipo = @tipo,
                        custoestimado = @custoestimado,
                        indicecorrecao = @indicecorrecao,
                        nf = @nf,
                        pesobruto = @pesobruto,
                        coduniadicional = @coduniadicional,
                        codfornecedor = @codfornecedor,
                        foralinhafornecedor = @foralinhafornecedor,
                        origemcusto = @origemcusto,
                        datafichatecnica = @datafichatecnica,
                        respfichatenica = @respfichatenica,
                        datainiciofichatecnica = @datainiciofichatecnica,
                        respcusto = @respcusto,
                        datacusto = @datacusto,
                        contabil = @contabil,
                        produto_novo = @produto_novo,
                        acompanhamento = @acompanhamento,
                        responsavel_acompanha = @responsavel_acompanha,
                        concluido_acompanha = @concluido_acompanha,
                        obs_acompanhamento = @obs_acompanhamento,
                        importado = @importado,
                        contabil_pldc = @contabil_pldc,
                        narrativa = @narrativa,
                        alx = @alx,
                        inativo = @inativo,
                        qtd_etiqueta = @qtd_etiqueta,
                        fracao = @fracao,
                        dividir_qtd_volume = @dividir_qtd_volume,
                        conta_aplica_contabil = @conta_aplica_contabil,
                        centro_custo_contabil = @centro_custo_contabil,
                        especial = @especial,
                        foto = @foto,
                        tamanho_construcao = @tamanho_construcao,
                        diverso = @diverso,
                        dificuldade = @dificuldade,
                        saldo_patrimonial_ano_anterior = @saldo_patrimonial_ano_anterior,
                        saldo_disponivel_ano_anterior = @saldo_disponivel_ano_anterior,
                        custo_despesa = @custo_despesa,
                        link_foto = @link_foto,
                        preco_shopping = @preco_shopping,
                        exportado_folhamatic = @exportado_folhamatic,
                        saldo_estoque = @saldo_estoque
                    WHERE codcompladicional = @codcompladicional;
                    """,
                    complemento);
            }

            return complemento;
        }
    }
}
