using Npgsql;
using Producao.Views.CadastroProduto;
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

namespace Producao.Views.kit.solucao
{
    /// <summary>
    /// Interação lógica para ViewDetalhesKitSolucao.xam
    /// </summary>
    public partial class ViewDetalhesKitSolucao : UserControl
    {
        private OsKitSolucaoModel OsKit;
        private bool carregandoSelecaoGrid;
        private bool inicializado;
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public ViewDetalhesKitSolucao(OsKitSolucaoModel osKit)
        {
            InitializeComponent();
            this.OsKit = osKit;
            cbSiglaShopping.Text = OsKit.shopping[..^2];
            tbId.Text = "0";
            tbItem.Text = "0";
            tbLocalShopping.Text = "KIT SOLUÇÃO";
            DataContext = new DetalhesKitSolucaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (inicializado)
                return;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
                vm.Planilhas = await vm.GetPlanilhasAsync();
                vm.CheckListGerais = await vm.GetCheckListGeralAsync(OsKit.os);
                vm.Classificacoes = await vm.GetClassificacoesAsync();

                vm.Sigla = await vm.GetSiglaAsync(OsKit.shopping[..^2]);

                inicializado = true;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnPlanilhaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (carregandoSelecaoGrid)
                return;

            try
            {

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;

                vm.ComplementoCheckList.codproduto = null;
                vm.Produtos = new ObservableCollection<ProdutoModel>();
                cbDescricao.SelectedItem = null;
                cbDescricao.Text = string.Empty;

                vm.ComplementoCheckList.coduniadicional = null;
                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                cbDescricaoAdicional.SelectedItem = null;
                cbDescricaoAdicional.Text = string.Empty;

                vm.Compledicional = null;
                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();

                vm.Produtos = await vm.GetProdutosAsync(vm.Planilha?.planilha);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnDescricaoSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (carregandoSelecaoGrid)
                return;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;

                vm.ComplementoCheckList.coduniadicional = null;
                vm.DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                cbDescricaoAdicional.SelectedItem = null;
                cbDescricaoAdicional.Text = string.Empty;

                vm.Compledicional = null;
                vm.CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();

                vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm.Produto?.codigo);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnLimparClick(object sender, RoutedEventArgs e)
        {
            Limpar();
        }

        private async void OnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.ComplementoCheckList.codcompl = null;
                vm.ComplementoCheckList.sigla = cbSiglaShopping.Text;
                vm.ComplementoCheckList.id_aprovado = vm.Sigla.id_aprovado;
                vm.ComplementoCheckList.kp = this.OsKit.os;
                vm.ComplementoCheckList.ordem = tbId.Text;
                vm.ComplementoCheckList.item_memorial = tbItem.Text;
                vm.ComplementoCheckList.local_shoppings = tbLocalShopping.Text;
                vm.ComplementoCheckList.class_solucao = cmbClassificacoes.SelectedItem.ToString();
                vm.ComplementoCheckList.motivos = cmbMotivos.SelectedItem.ToString();
                vm.ComplementoCheckList.inserido_por = Environment.UserName;
                vm.ComplementoCheckList.inserido_em = DateTime.Now;

                ComplementoCheckListModel compl = await vm.AddComplementoCheckListAsync(vm.ComplementoCheckList);

                vm.CheckListGerais = await vm.GetCheckListGeralAsync(this.OsKit.os);

                SelectCheckListGeral(compl.codcompl);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro ao inserir");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;

                if (!PrepararComplementoParaEdicao(vm))
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.ComplementoCheckList.class_solucao = cmbClassificacoes!.SelectedItem!.ToString();
                vm.ComplementoCheckList.motivos = cmbMotivos!.SelectedItem!.ToString();
                vm.ComplementoCheckList.alterado_por = Environment.UserName;
                vm.ComplementoCheckList.alterado_em = DateTime.Now;
                ComplementoCheckListModel compl = await vm.AddComplementoCheckListAsync(vm.ComplementoCheckList);

                vm.CheckListGerais = await vm.GetCheckListGeralAsync(this.OsKit.os);
                
                SelectCheckListGeral(compl.codcompl);
                dgCheckListGeral.Rebind();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro ao inserir");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private bool PrepararComplementoParaEdicao(DetalhesKitSolucaoViewModel vm)
        {
            if (vm.ComplementoCheckList?.codcompl is > 0)
                return true;

            if (vm.CheckListGeral?.codcompl is > 0)
            {
                vm.ComplementoCheckList ??= new ComplementoCheckListModel();
                vm.ComplementoCheckList.codcompl = vm.CheckListGeral.codcompl;
                return true;
            }

            MessageBox.Show("Selecione uma linha do checklist antes de editar.", "Editar kit solução", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;

                //QryRequisicaoDetalheModel requi = (from r in vm.QryRequisicaoDetalhes select r).FirstOrDefault();

                vm.ChkGerais = await vm.GetKitCheckListGeralAsync(this.OsKit.os);
                vm.ChkGeral = (from r in vm.ChkGerais select r).FirstOrDefault();

                if (vm.ChkGeral == null)
                {
                    MessageBox.Show("Nenhum item encontrado para imprimir.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                var caminhoArquivo = BaseSettings.ResolveImpressosPath($"REQUISICAO_KIT_{vm.ChkGeral.os}.xlsx");
                KitRequisicaoExporter.Exportar("KIT SOLUÇÃO", vm.ChkGerais, caminhoArquivo);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void dgCheckListGeral_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            try
            {
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                var dado = e.Row.Item as QryCheckListGeralModel;
                ComplementoCheckListModel CompleChkList = new()
                {
                    codcompl = dado?.codcompl,
                    obs = dado?.obs,
                    orient_montagem = dado?.orient_montagem,
                    orient_desmont = dado?.orient_desmont,
                    qtd = dado?.qtd,
                    alterado_por = Environment.UserName,
                    alterado_em = DateTime.Now
                //ordem = dado?.id
            };
                await vm.EditComplementoCheckListAsync(CompleChkList);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro ao inserir");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
                var record = vm.CheckListGeral;

                
                if (record == null)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    return;
                }

                carregandoSelecaoGrid = true;
                try
                {
                    vm.ComplementoCheckList = new ComplementoCheckListModel
                    {
                        ordem = vm?.CheckListGeral?.id,
                        sigla = vm?.CheckListGeral?.sigla,
                        local_shoppings = vm?.CheckListGeral?.local_shoppings,
                        codproduto = vm?.CheckListGeral?.codigo,
                        obs = vm?.CheckListGeral?.obs,
                        dataalteracaodesc = vm?.CheckListGeral?.dataalteracaodesc,
                        alteradopor = vm?.CheckListGeral?.alteradopor,
                        orient_montagem = vm?.CheckListGeral?.orient_montagem,
                        item_memorial = vm?.CheckListGeral?.item_memorial,
                        incluidopordesc = vm?.CheckListGeral?.incluidopordesc,
                        kp = vm?.CheckListGeral?.kp,
                        orient_desmont = vm?.CheckListGeral?.orient_desmont,
                        qtd = vm?.CheckListGeral.qtd,
                        coduniadicional = vm?.CheckListGeral?.coduniadicional,
                        codcompl = vm?.CheckListGeral?.codcompl,
                        nivel = vm?.CheckListGeral?.nivel,
                        carga = vm?.CheckListGeral?.carga,
                        class_solucao = vm?.CheckListGeral?.class_solucao,
                        motivos = vm?.CheckListGeral?.motivos,
                        id_aprovado = vm?.CheckListGeral?.id_aprovado,
                        historico = vm?.CheckListGeral?.historico,
                        agrupar = vm?.CheckListGeral?.agrupar,
                        inserido_por = vm?.CheckListGeral?.inserido_por,
                        inserido_em = vm?.CheckListGeral?.inserido_em,
                    };

                    vm.Planilha = (from p in vm.Planilhas where p.planilha == record?.planilha select p).FirstOrDefault();
                    vm.Produtos = await vm.GetProdutosAsync(vm?.Planilha?.planilha);
                    vm.Produto = (from p in vm.Produtos where p.codigo == record?.codigo select p).FirstOrDefault();
                    vm.DescAdicionais = await vm.GetDescAdicionaisAsync(vm?.Produto?.codigo);
                    vm.DescAdicional = (from d in vm.DescAdicionais where d.coduniadicional == record?.coduniadicional select d).FirstOrDefault();

                    vm.CompleAdicionais = await vm.GetCompleAdicionaisAsync(vm?.CheckListGeral?.coduniadicional);
                    vm.CheckListGeralComplementos = await vm.GetCheckListGeralComplementoAsync(vm?.CheckListGeral?.codcompl);

                    cmbClassificacoes.SelectedItem = record?.class_solucao;
                    cmbMotivos.SelectedItem = record?.motivos;
                }
                finally
                {
                    carregandoSelecaoGrid = false;
                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                carregandoSelecaoGrid = false;
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void dgComplemento_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            try
            {
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
                e.NewObject = new QryCheckListGeralComplementoModel
                {
                    codcompl = vm.CheckListGeral?.codcompl,
                    qtd = null
                };
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void OnComplementoAdicionalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is not RadComboBox combo ||
                    combo.DataContext is not QryCheckListGeralComplementoModel record ||
                    combo.SelectedItem is not TblComplementoAdicionalModel complemento)
                {
                    return;
                }

                record.unidade = complemento.unidade;
                record.saldoestoque = complemento.saldo_estoque;
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            
        }

        private void dgComplemento_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {

        }

        private async void dgComplemento_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
            try
            {
                if (e.Row.Item is not QryCheckListGeralComplementoModel data)
                    return;

                if (!ComplementoTemDadosParaSalvar(data))
                {
                    CancelarLinhaComplementoVazia(vm.CheckListGeralComplementos, data, dgComplemento);
                    return;
                }

                if (!ComplementoEstaValido(data))
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.DetCompl.coddetalhescompl = data.coddetalhescompl;
                vm.DetCompl.codcompl = data.codcompl;
                vm.DetCompl.codcompladicional = data.codcompladicional;
                vm.DetCompl.qtd = data.qtd.GetValueOrDefault();
                vm.DetCompl.confirmado = data.confirmado;
                vm.DetCompl.confirmado_data = data.confirmado == "-1" ? DateTime.Now : null;
                vm.DetCompl.confirmado_por = data.confirmado == "-1" ? Environment.UserName : null;
                vm.DetCompl.desabilitado_confirmado_data = data.confirmado == "-1" ? DateTime.Now : null;
                vm.DetCompl.desabilitado_confirmado_por = data.confirmado == "-1" ? Environment.UserName : null;
                vm.DetCompl.local_producao = "JACAREÍ";
                vm.DetCompl.os = data.os;

                vm.DetCompl = await vm.AddDetalhesComplementoCheckListAsync(vm.DetCompl);
                data.coddetalhescompl = vm.DetCompl.coddetalhescompl;
                dgComplemento.Rebind();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                var toRemove = vm.CheckListGeralComplementos.Where(x => x.coddetalhescompl == null).ToList();
                foreach (var item in toRemove)
                    vm.CheckListGeralComplementos.Remove(item);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void dgComplemento_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row.Item is not QryCheckListGeralComplementoModel rowData)
                return;

            if (!ComplementoTemDadosParaSalvar(rowData))
                return;

            if (!rowData.codcompl.HasValue)
            {
                AddValidation(e, "codcompladicional", "Erro ao selecionar a linha.");
                AddValidation(e, "qtd", "Erro ao selecionar a linha.");
            }
            else if (!rowData.codcompladicional.HasValue)
            {
                AddValidation(e, "codcompladicional", "Seleciona o COMPLEMENTO ADICIONAL.");
            }
            else if (!QuantidadeComplementoInformada(rowData))
            {
                AddValidation(e, "qtd", "Informa a QTDE.");
            }
        }

        private static bool ComplementoTemDadosParaSalvar(QryCheckListGeralComplementoModel rowData)
        {
            return rowData.codcompladicional.HasValue || QuantidadeComplementoInformada(rowData);
        }

        private static bool ComplementoEstaValido(QryCheckListGeralComplementoModel rowData)
        {
            return rowData.codcompl.HasValue &&
                   rowData.codcompladicional.HasValue &&
                   QuantidadeComplementoInformada(rowData);
        }

        private static bool QuantidadeComplementoInformada(QryCheckListGeralComplementoModel rowData)
        {
            return rowData.qtd.HasValue;
        }

        private static void CancelarLinhaComplementoVazia(ObservableCollection<QryCheckListGeralComplementoModel>? itens, QryCheckListGeralComplementoModel rowData, RadGridView grid)
        {
            if (rowData.coddetalhescompl != null || itens is null)
                return;

            grid.Dispatcher.BeginInvoke(() =>
            {
                if (itens.Contains(rowData))
                    itens.Remove(rowData);

                grid.CancelEdit();
                grid.Rebind();
            });
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = propertyName,
                ErrorMessage = message
            });
        }

        private async void OnClassificacaoSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                string classificacao = cmbClassificacoes.SelectedItem?.ToString(); //e.AddedItems[0].ToString(); 
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
                vm.Motivos = await vm.GetMotivosAsync(classificacao);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        void Limpar()
        {
            DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;

            //tbId.Text = string.Empty;
            //tbItem.Text = string.Empty;
            //tbLocalShopping.Text = string.Empty;
            cbPlanilha.SelectedItem = null;
            cbPlanilha.Text = string.Empty;
            cbDescricao.SelectedItem = null;
            cbDescricao.Text = string.Empty;
            cbDescricaoAdicional.SelectedItem = null;
            cbDescricaoAdicional.Text = string.Empty;
            tbQtde.Value = null;
            tbOrientacaoProducao.Text = string.Empty;
            tbOrientacaoMontagem.Text = string.Empty;
            tbOrientacaoDesmontagem.Text = string.Empty;

            dgCheckListGeral.SelectedItem = null;
            dgComplemento.SelectedItem = null;


            //cmbClassificacoes.SelectedItem = null;
            cmbClassificacoes.SelectedIndex = -1;
            vm.Motivos = null;
            cmbMotivos.SelectedItem = null;
            //btnAddicionar.Visibility = Visibility.Collapsed;

            vm.ComplementoCheckList = new ComplementoCheckListModel();

            tbId.Focus();
        }

        private void SelectCheckListGeral(long? codcompl)
        {
            DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;
            var item = vm.CheckListGerais?.FirstOrDefault(c => c.codcompl == codcompl);
            if (item == null)
                return;

            dgCheckListGeral.SelectedItem = item;
            dgCheckListGeral.ScrollIntoViewAsync(item, null);
        }

        private async void OnProdutos(object sender, RoutedEventArgs e)
        {
            try
            {
                DetalhesKitSolucaoViewModel vm = (DetalhesKitSolucaoViewModel)DataContext;

                var window = new Window
                {
                    Title = "BUSCAR PRODUTO",
                    Height = 600,
                    Width = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    WindowStyle = WindowStyle.ToolWindow,
                    ResizeMode = ResizeMode.NoResize,
                    Content = new LocalizaProduto(/*this.DataContext*/),
                    Owner = Window.GetWindow(dgCheckListGeral.Parent), //GetTopParent();
                    ShowInTaskbar = false
                };
                window.ShowDialog();

                var descricao = ((LocalizaProdutoViewModel)((LocalizaProduto)window.Content).DataContext).Descricao;

                if (descricao == null)
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                carregandoSelecaoGrid = true;
                try
                {
                    cbPlanilha.SelectedItem = (from p in vm.Planilhas where p.planilha == descricao.planilha select p).FirstOrDefault();
                    vm.Produtos = await vm.GetProdutosAsync(descricao.planilha);
                    cbDescricao.SelectedItem = (from p in vm.Produtos where p.codigo == descricao.codigo select p).FirstOrDefault(); 
                    vm.DescAdicionais = await vm.GetDescAdicionaisAsync(descricao.codigo);
                    cbDescricaoAdicional.SelectedItem = (from d in vm.DescAdicionais where d.coduniadicional == descricao.coduniadicional select d).FirstOrDefault();
                }
                finally
                {
                    carregandoSelecaoGrid = false;
                }
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                tbQtde.Focus();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }

    public class DetalhesKitSolucaoViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private SiglaChkListModel _sigla;
        public SiglaChkListModel Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged("Sigla"); }
        }
        private ObservableCollection<RelplanModel> _planilhas;
        public ObservableCollection<RelplanModel> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
        }
        private RelplanModel _planilha;
        public RelplanModel Planilha
        {
            get { return _planilha; }
            set { _planilha = value; RaisePropertyChanged("Planilha"); }
        }
        private ObservableCollection<ProdutoModel> _produtos;
        public ObservableCollection<ProdutoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }
        private ProdutoModel _produto;
        public ProdutoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _descAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
        {
            get { return _descAdicionais; }
            set { _descAdicionais = value; RaisePropertyChanged("DescAdicionais"); }
        }
        private TabelaDescAdicionalModel _descAdicional;
        public TabelaDescAdicionalModel DescAdicional
        {
            get { return _descAdicional; }
            set { _descAdicional = value; RaisePropertyChanged("DescAdicional"); }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _compleAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
        {
            get { return _compleAdicionais; }
            set { _compleAdicionais = value; RaisePropertyChanged("CompleAdicionais"); }
        }
        private TblComplementoAdicionalModel _compledicional;
        public TblComplementoAdicionalModel Compledicional
        {
            get { return _compledicional; }
            set { _compledicional = value; RaisePropertyChanged("Compledicional"); }
        }

        private QryDescricao descricao;
        public QryDescricao Descricao
        {
            get { return descricao; }
            set { descricao = value; RaisePropertyChanged("Descricao"); }
        }

        private ObservableCollection<ComplementoCheckListModel> _complementoCheckLists;
        public ObservableCollection<ComplementoCheckListModel> ComplementoCheckLists
        {
            get { return _complementoCheckLists; }
            set { _complementoCheckLists = value; RaisePropertyChanged("ComplementoCheckLists"); }
        }

        private ComplementoCheckListModel complementoCheckList;
        public ComplementoCheckListModel ComplementoCheckList
        {
            get { return complementoCheckList; }
            set { complementoCheckList = value; RaisePropertyChanged("ComplementoCheckList"); }
        }

        private QryCheckListGeralModel _checkListGeral;
        public QryCheckListGeralModel CheckListGeral
        {
            get { return _checkListGeral; }
            set { _checkListGeral = value; RaisePropertyChanged("CheckListGeral"); }
        }
        private ObservableCollection<QryCheckListGeralModel> _checkListGerais;
        public ObservableCollection<QryCheckListGeralModel> CheckListGerais
        {
            get { return _checkListGerais; }
            set { _checkListGerais = value; RaisePropertyChanged("CheckListGerais"); }
        }

        private ObservableCollection<string> _classificacoes;
        public ObservableCollection<string> Classificacoes
        {
            get { return _classificacoes; }
            set { _classificacoes = value; RaisePropertyChanged("Classificacoes"); }
        }

        private ObservableCollection<string> _motivos;
        public ObservableCollection<string> Motivos
        {
            get { return _motivos; }
            set { _motivos = value; RaisePropertyChanged("Motivos"); }
        }

        private QryCheckListGeralComplementoModel _checkListGeralComplemento;
        public QryCheckListGeralComplementoModel CheckListGeralComplemento
        {
            get { return _checkListGeralComplemento; }
            set { _checkListGeralComplemento = value; RaisePropertyChanged("CheckListGeralComplemento"); }
        }
        private ObservableCollection<QryCheckListGeralComplementoModel> _checkListGeralComplementos;
        public ObservableCollection<QryCheckListGeralComplementoModel> CheckListGeralComplementos
        {
            get { return _checkListGeralComplementos; }
            set { _checkListGeralComplementos = value; RaisePropertyChanged("CheckListGeralComplementos"); }
        }

        private DetalhesComplemento _detCompl;
        public DetalhesComplemento DetCompl
        {
            get { return _detCompl; }
            set { _detCompl = value; RaisePropertyChanged("DetCompl"); }
        }
        private ObservableCollection<DetalhesComplemento> _detCompls;
        public ObservableCollection<DetalhesComplemento> DetCompls
        {
            get { return _detCompls; }
            set { _detCompls = value; RaisePropertyChanged("DetCompls"); }
        }
        
        private KitChkGeralModel _chkGeral;
        public KitChkGeralModel ChkGeral
        {
            get { return _chkGeral; }
            set { _chkGeral = value; RaisePropertyChanged("ChkGeral"); }
        }
        private ObservableCollection<KitChkGeralModel> _chkGerais;
        public ObservableCollection<KitChkGeralModel> ChkGerais
        {
            get { return _chkGerais; }
            set { _chkGerais = value; RaisePropertyChanged("ChkGerais"); }
        }

        public DetalhesKitSolucaoViewModel()
        {
            DetCompl = new DetalhesComplemento();
            ComplementoCheckList = new ComplementoCheckListModel();
            Planilhas = new ObservableCollection<RelplanModel>();
        }

        public async Task<SiglaChkListModel> GetSiglaAsync(string sigla)
        {
            try
            {
                var data = await KitDetalhesRepository.GetSiglaAsync(sigla);
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                var data = await KitDetalhesRepository.GetPlanilhasAsync();
                return new ObservableCollection<RelplanModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryCheckListGeralModel>> GetCheckListGeralAsync(long? kp)
        {
            try
            {
                var data = await KitDetalhesRepository.GetCheckListGeralAsync(kp);
                return new ObservableCollection<QryCheckListGeralModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<string>> GetClassificacoesAsync()
        {
            try
            {
                var data = await KitDetalhesRepository.GetClassificacoesAsync();
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<string>> GetMotivosAsync(string classificacao)
        {
            try
            {
                var data = await KitDetalhesRepository.GetMotivosAsync(classificacao);
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProdutoModel>> GetProdutosAsync(string? planilha)
        {
            try
            {
                var data = await KitDetalhesRepository.GetProdutosAsync(planilha);
                return new ObservableCollection<ProdutoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TabelaDescAdicionalModel>> GetDescAdicionaisAsync(long? codigo)
        {
            try
            {
                var data = await KitDetalhesRepository.GetDescAdicionaisAsync(codigo);

                return new ObservableCollection<TabelaDescAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task EditComplementoCheckListAsync(ComplementoCheckListModel compChkList)
        {
            try
            {
                await KitDetalhesRepository.EditComplementoCheckListAsync(compChkList);
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task<ComplementoCheckListModel> AddComplementoCheckListAsync(ComplementoCheckListModel ComplementoCheckList)
        {
            try
            {
                return await KitDetalhesRepository.AddComplementoCheckListAsync(ComplementoCheckList);
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TblComplementoAdicionalModel>> GetCompleAdicionaisAsync(long? coduniadicional)
        {
            try
            {
                CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                var data = await KitDetalhesRepository.GetCompleAdicionaisAsync(coduniadicional);

                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryCheckListGeralComplementoModel>> GetCheckListGeralComplementoAsync(long? codcompl)
        {
            try
            {
                var data = await KitDetalhesRepository.GetCheckListGeralComplementoAsync(codcompl);

                return new ObservableCollection<QryCheckListGeralComplementoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DetalhesComplemento> AddDetalhesComplementoCheckListAsync(DetalhesComplemento detCompl)
        {
            try
            {
                return await KitDetalhesRepository.AddDetalhesComplementoCheckListAsync(detCompl);
            }
            catch (NpgsqlException)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<KitChkGeralModel>> GetKitCheckListGeralAsync(long? os)
        {
            try
            {
                var data = await KitDetalhesRepository.GetKitCheckListGeralAsync(os);

                return new ObservableCollection<KitChkGeralModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
    }
}

