using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Producao.Views.CadastroProduto
{
    /// <summary>
    /// Interação lógica para CadastroCompmento.xam
    /// </summary>
    public partial class CadastroCompmento : Window
    {
        TabelaDescAdicionalModel produtoAdicional;

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
                CadastroCompmentoViewModel vm = (CadastroCompmentoViewModel)DataContext;
                vm.Unidades = await Task.Run(vm.GetUnidadesAsync);
                vm.ComplementoAdicionais = await Task.Run(() => vm.GetComplementoAdicionaisAsync(produtoAdicional.coduniadicional));
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnAddNewRowInitiating(object sender, AddNewRowInitiatingEventArgs e)
        {
            CadastroCompmentoViewModel vm = (CadastroCompmentoViewModel)DataContext;
            ((TblComplementoAdicionalModel)e.NewObject).coduniadicional = produtoAdicional.coduniadicional;
        }

        private void OnCurrentCellDropDownSelectionChanged(object sender, CurrentCellDropDownSelectionChangedEventArgs e)
        {

        }

        private void OnCurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

        }

        private async void OnRowValidated(object sender, RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            CadastroCompmentoViewModel vm = (CadastroCompmentoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                TblComplementoAdicionalModel data = (TblComplementoAdicionalModel)e.RowData;
                //data.codigoproduto
                //data.descricao_adicional
                //data.cadastradopor = data.codigoproduto == null ? Environment.UserName : data.cadastradopor;
                //data.cadastradoem = data.codigoproduto == null ? DateTime.Now : data.cadastradoem;
                //data.alteradopor = data.codigoproduto == null ? null : Environment.UserName;
                //data.alteradoem = data.codigoproduto == null ? null : DateTime.Now;
                //data.revisao
                //data.obsproducaoobrigatoria
                //data.obsmontagem
                //data.unidade = produto
                //data.inativo = data.inativo == null ? "0" : "-1";

                var comple = vm.ComplementoAdicionais.Where(x => x.coduniadicional == produtoAdicional.coduniadicional).LastOrDefault();


                //codcompladicional
                //coduniadicional = 
                //complementoadicional
                //status
                data.estoque_inicial = 0;
                //desc_process
                data.estoque_inicial_processado = 0;
                //altura
                //largura
                //profundidade
                data.vida_util = comple == null ? 0 : comple.vida_util;
                //diametro
                data.peso = comple == null ? 0 : comple.peso;
                //unidade
                data.cadastradopor = data.codcompladicional == null ? Environment.UserName : comple.cadastradopor;
                data.cadastradoem = data.codcompladicional == null ? DateTime.Now : comple.cadastradoem;
                data.alterado_por = data.codcompladicional == null ? null : Environment.UserName;
                data.alterado_em = data.codcompladicional == null ? null : DateTime.Now;
                //custo_real
                data.prodcontrolado = comple == null ? "0" : comple.prodcontrolado;
                //volume
                //area
                //precolocacao
                //descricaofiscal = ultimo ou null
                //descricaoespanhol
                //estoque_min
                //v_unit
                //v_unit_dolar
                //ncm
                //tipo
                //custoestimado
                //indicecorrecao
                //nf
                data.pesobruto = comple == null ? 0 : comple.pesobruto;
                //codfornecedor
                //foralinhafornecedor
                data.origemcusto = comple == null ? null : comple.origemcusto;
                //datafichatecnica
                //respfichatenica
                //datainiciofichatecnica
                //respcusto
                //datacusto
                data.contabil = comple == null ? null : comple.contabil;
                data.produto_novo = DateTime.Now.Year.ToString();
                //acompanhamento
                //responsavel_acompanha
                //concluido_acompanha
                //obs_acompanhamento
                //importado
                data.contabil_pldc = comple == null ? null : comple.contabil_pldc;
                //narrativa
                //alx
                //data.inativo = data.inativo == null ? "0" : "-1";
                data.inativo = data.inativo == null ? "0" : data.inativo;
                //qtd_etiqueta
                //fracao
                //dividir_qtd_volume
                //conta_aplica_contabil
                //centro_custo_contabil
                //especial
                //foto
                //custodescadicional_custo
                //custodescadicional_codcompladicional
                //tamanho_construcao
                //diverso
                //dificuldade
                //saldo_patrimonial_ano_anterior
                //saldo_disponivel_ano_anterior
                //custo_despesa
                //link_foto
                data.preco_shopping = comple == null ? null : comple.preco_shopping;
                //exportado_folhamatic
                data.saldo_estoque = 0;
    


                data = await Task.Run(() => vm.SaveAsync(data));
                var record = sfdatagrid.View.CurrentAddItem as ProdutoModel;
                sfdatagrid.View.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                var toRemove = vm.ComplementoAdicionais.Where(x => x.codcompladicional == null).ToList();
                foreach (var item in toRemove)
                    vm.ComplementoAdicionais.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            TblComplementoAdicionalModel rowData = (TblComplementoAdicionalModel)e.RowData;
            if (rowData.coduniadicional == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("codcompladicional", "descrição adicional não selecionado");
                e.ErrorMessages.Add("complementoadicional","Informe o COMPLEMENTO ADICIONAL");
                e.ErrorMessages.Add("descricaofiscal", "Informe a DESCRIÇÃO FISCAL");
                e.ErrorMessages.Add("unidade","Informe a UNIDADE");
            }
            else if (rowData.complementoadicional == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("complementoadicional", "Informe o COMPLEMENTO ADICIONAL");
            }
            else if (rowData.descricaofiscal == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descricaofiscal", "Informe a DESCRIÇÃO FISCAL");
            }
            else if (rowData.unidade == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("unidade", "Informe a UNIDADE");
            }
        }
    }

    public class CadastroCompmentoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private TblComplementoAdicionalModel _complementoAdicional;
        public TblComplementoAdicionalModel ComplementoAdicional
        {
            get { return _complementoAdicional; }
            set { _complementoAdicional = value; RaisePropertyChanged("ComplementoAdicional"); }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _complementoAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> ComplementoAdicionais
        {
            get { return _complementoAdicionais; }
            set { _complementoAdicionais = value; RaisePropertyChanged("ComplementoAdicionais"); }
        }

        private UnidadeModel _unidade;
        public UnidadeModel Unidade
        {
            get { return _unidade; }
            set { _unidade = value; RaisePropertyChanged("Unidade"); }
        }

        private ObservableCollection<UnidadeModel> _unidades;
        public ObservableCollection<UnidadeModel> Unidades
        {
            get { return _unidades; }
            set { _unidades = value; RaisePropertyChanged("Unidades"); }
        }

        public async Task<ObservableCollection<TblComplementoAdicionalModel>> GetComplementoAdicionaisAsync(long? coduniadicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ComplementoAdicionais
                    .OrderBy(c => c.acompanhamento)
                    .Where(c => c.coduniadicional == coduniadicional)
                    .ToListAsync();
                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<UnidadeModel>> GetUnidadesAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Unidades
                    .OrderBy(c => c.unidade)
                    .ToListAsync();
                return new ObservableCollection<UnidadeModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TblComplementoAdicionalModel> SaveAsync(TblComplementoAdicionalModel complemento)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ComplementoAdicionais.SingleMergeAsync(complemento);
                await db.SaveChangesAsync();
                return complemento;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
