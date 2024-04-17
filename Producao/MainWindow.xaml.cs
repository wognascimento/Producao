using Microsoft.EntityFrameworkCore;
using Producao.Views;
using Producao.Views.CadastroProduto;
using Producao.Views.CentralModelos;
using Producao.Views.CheckList;
using Producao.Views.Construcao;
using Producao.Views.Controlado;
using Producao.Views.Estoque;
using Producao.Views.kit;
using Producao.Views.kit.desmontagem;
using Producao.Views.kit.manutencao;
using Producao.Views.kit.solucao;
using Producao.Views.OrdemServico.Desbaiamento;
using Producao.Views.OrdemServico.Produto;
using Producao.Views.OrdemServico.Requisicao;
using Producao.Views.OrdemServico.Servicos;
using Producao.Views.Planilha;
using Squirrel;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using SizeMode = Syncfusion.SfSkinManager.SizeMode;
namespace Producao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		#region Fields
        private string currentVisualStyle;
		private string currentSizeMode;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current visual style.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string CurrentVisualStyle
        {
            get
            {
                return currentVisualStyle;
            }
            set
            {
                currentVisualStyle = value;
                OnVisualStyleChanged();
            }
        }
		
		/// <summary>
        /// Gets or sets the current Size mode.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string CurrentSizeMode
        {
            get
            {
                return currentSizeMode;
            }
            set
            {
                currentSizeMode = value;
                OnSizeModeChanged();
            }
        }

        #endregion

        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public MainWindow()
        {
            InitializeComponent();
			this.Loaded += OnLoaded;
            StyleManager.ApplicationTheme = new Windows11Theme();

            var appSettings = ConfigurationManager.GetSection("appSettings") as NameValueCollection;
            if(appSettings[0].Length > 0)
                BaseSettings.Username = appSettings[0];

            txtUsername.Text = BaseSettings.Username;
            txtDataBase.Text = BaseSettings.Database;
        }
        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CurrentVisualStyle = "Metro"; // "FluentLight";
	        CurrentSizeMode = "Default";
        }
		/// <summary>
        /// On Visual Style Changed.
        /// </summary>
        /// <remarks></remarks>
        private void OnVisualStyleChanged()
        {
            VisualStyles visualStyle = VisualStyles.Default;
            Enum.TryParse(CurrentVisualStyle, out visualStyle);            
            if (visualStyle != VisualStyles.Default)
            {
                SfSkinManager.ApplyStylesOnApplication = true;
                SfSkinManager.SetVisualStyle(this, visualStyle);
                SfSkinManager.ApplyStylesOnApplication = false;
            }
        }

        UpdateManager manager;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            try
            {

                manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/wognascimento/Producao");
                var updateInfo = await manager.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    RadWindow.Confirm(new DialogParameters()
                    {
                        Header = "Atualização",
                        Content = "Existe uma atualização para o sistema, deseja atualiza?",
                        Closed = async (object sender, WindowClosedEventArgs e) =>
                        {
                            var result = e.DialogResult;
                            if (result == true)
                            {
                                await manager.UpdateApp();
                                RadWindow.Alert("Sistema atualizado!\nFecha e abre o Sistema, para aplicar a atualização.");
                            }
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message);
            }
            */
        }

        /// <summary>
        /// On Size Mode Changed event.
        /// </summary>
        /// <remarks></remarks>
        private void OnSizeModeChanged()
        {
            SizeMode sizeMode = SizeMode.Default;
            Enum.TryParse(CurrentSizeMode, out sizeMode);
            if (sizeMode != SizeMode.Default)
            {
                SfSkinManager.ApplyStylesOnApplication = true;
                SfSkinManager.SetSizeMode(this, sizeMode);
                SfSkinManager.ApplyStylesOnApplication = false;
            }
        }

        public void adicionarFilho(object filho, string title, string name)
        {
            var doc = ExistDocumentInDocumentContainer(name);
            if (doc == null)
            {
                doc = (FrameworkElement?)filho;
                DocumentContainer.SetHeader(doc, title);
                doc.Name = name.ToLower();
                _mdi.Items.Add(doc);
            }
            else
            {
                //_mdi.RestoreDocument(doc as UIElement);
                _mdi.ActiveDocument = doc;
            }
        }

        private FrameworkElement ExistDocumentInDocumentContainer(string name_)
        {
            foreach (FrameworkElement element in _mdi.Items)
            {
                if (name_.ToLower() == element.Name)
                {
                    return element;
                }
            }
            return null;
        }

        private void MenuItemAdv_Click(object sender, RoutedEventArgs e)
        {
            /*
            ViewAprovado viewAprovado = new();
            DocumentContainer.SetHeader(viewAprovado, "PRODUÇÃO APROVADOS");
            DocumentContainer.SetSizetoContentInMDI(viewAprovado, true);
            DocumentContainer.SetMDIBounds(viewAprovado, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(viewAprovado, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(viewAprovado);
            */

            adicionarFilho(new ViewAprovado(), "PRODUÇÃO APROVADOS", "APROVADOS");
        }

        private void OnChecklist_Click(object sender, RoutedEventArgs e)
        {
            /*
            ViewCheckListNatal view = new();
            DocumentContainer.SetHeader(view, "CHECKLIST NATAL");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCheckListNatal(), "CHECKLIST NATAL", "CHECKLIST_NATAL");
        }

        private void OnRevisaoChecklistClick(object sender, RoutedEventArgs e)
        {
            /*
            ViewCheckListRevisao view = new();
            DocumentContainer.SetHeader(view, "REVISÃO DE CHECKLIST");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCheckListRevisao(), "REVISÃO DE CHECKLIST", "REVISAO_CHECKLIST");

        }

        private void OnEtiquetaChecklistClick(object sender, RoutedEventArgs e)
        {
            /*
            ViewEtiquetaCheckList view = new();
            DocumentContainer.SetHeader(view, "ETIQUETA CHECKLIST");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewEtiquetaCheckList(), "ETIQUETA CHECKLIST", "ETIQUETA_CHECKLIST");
        }

        private void OnEtiquetaChecklistEmitidaClick(object sender, RoutedEventArgs e)
        {
            /*
            ViewEtiquetaCheckListEmitida view = new();
            DocumentContainer.SetHeader(view, "ETIQUETA CHECKLIST EMITIDAS");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewEtiquetaCheckListEmitida(), "ETIQUETA CHECKLIST EMITIDAS", "ETIQUETA_CHECKLIST_EMITIDAS");
        }

        private void OnCentralCriarModelo(object sender, RoutedEventArgs e)
        {
            /*
            ViewCentralCriarModelo view = new();
            DocumentContainer.SetHeader(view, "CRIAR MODELO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCentralCriarModelo(), "CRIAR MODELO", "CRIAR_MODELO");
        }

        private void OnCentralTabelaPa(object sender, RoutedEventArgs e)
        {
            /*
            ViewCentralTabelaPA view = new();
            DocumentContainer.SetHeader(view, "TABELA ARVORE P.A");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCentralTabelaPA(), "TABELA ARVORE P.A", "TABELA_ARVORE_PA");
        }

        private void OnCentralFatorConversao(object sender, RoutedEventArgs e)
        {
            /*
            ViewCentralFatorConversao view = new();
            DocumentContainer.SetHeader(view, "TABELA FATOR CONVERSÃO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCentralFatorConversao(), "TABELA FATOR CONVERSÃO", "TABELA_FATOR_CONVERSAO");
        }

        private void OnCentralEmitirOs(object sender, RoutedEventArgs e)
        {
            /*
            ViewCentralEmitirOs view = new();
            DocumentContainer.SetHeader(view, "CONTROLE ORDEM DE SERVIÇO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1024.0) / 2.0, (this._mdi.ActualHeight - 800.0) / 2.0, 1024.0, 800.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCentralEmitirOs(), "CONTROLE ORDEM DE SERVIÇO", "CONTROLE_ORDEM_SERVICO");
        }

        private void OnCentralStatusCheckList(object sender, RoutedEventArgs e)
        {
            /*
            ViewCentralStatusCheckList view = new();
            DocumentContainer.SetHeader(view, "STATUS CHECK-LIST");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 800.0) / 2.0, 1000.0, 800.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCentralStatusCheckList(), "STATUS CHECK-LIST", "STATUS_CHECK_LIST");
        }


        private void OnCreateReceitaRequisicao(object sender, RoutedEventArgs e)
        {
            /*
            ViewReceitaRequisicao view = new();
            DocumentContainer.SetHeader(view, "RECEITA REQUISIÇÃO MATERIAL");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 800.0) / 2.0, 1000.0, 800.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewReceitaRequisicao(), "RECEITA REQUISIÇÃO MATERIAL", "RECEITA_REQUISICAO_MATERIAL");
        }

        private void OnCadastroProduto(object sender, RoutedEventArgs e)
        {
            /*
            ViewCadastroProduto view = new();
            DocumentContainer.SetHeader(view, "CADASTRO DE PRODUTOS");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 800.0) / 2.0, 1000.0, 800.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewCadastroProduto(), "CADASTRO DE PRODUTOS", "CADASTRO_PRODUTOS");
        }

        private void OnCadastroEspanhol(object sender, RoutedEventArgs e)
        {
            /*
            CadastroDescricaoEspanhol view = new();
            DocumentContainer.SetHeader(view, "CADASTRO DE DESCRIÇÃO ESPANHOL");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 800.0) / 2.0, 1000.0, 800.0));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new CadastroDescricaoEspanhol(), "CADASTRO DE DESCRIÇÃO ESPANHOL", "CADASTRO_DESCRICAO_ESPANHOL");
        }

        private void OnTodasDescricoes(object sender, RoutedEventArgs e)
        {
            /*
            TodasDescricoes view = new();
            DocumentContainer.SetHeader(view, "TODAS DESCRIÇÕES CADASTRADAS");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 800.0) / 2.0, 1000.0, 800.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new TodasDescricoes(), "TODAS DESCRIÇÕES CADASTRADAS", "TODAS_DESCRICOES_CADASTRADAS");
        }

        private void OnEmitirOSServicoClick(object sender, RoutedEventArgs e)
        {
            /*
            EmissaoServico view = new();
            DocumentContainer.SetHeader(view, "EMISSÃO DE O.S. DE SERVIÇO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 800) / 2.0, (this._mdi.ActualHeight - 400) / 2.0, 800.0, 400));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new EmissaoServico(), "EMISSÃO DE O.S. DE SERVIÇO", "EMISSAO_OS_SERVICO");
        }

        private void OnEmitidasOSServicoClick(object sender, RoutedEventArgs e)
        {
            /*
            EmissaoServicoEmitidas view = new();
            DocumentContainer.SetHeader(view, "O.S. DE SERVIÇO EMITIDAS");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000) / 2.0, (this._mdi.ActualHeight - 600) / 2.0, 1000, 600));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new EmissaoServicoEmitidas(), "O.S. DE SERVIÇO EMITIDAS", "OS_SERVICO_EMITIDAS");
        }

        private void OnAlterarRequisicoes(object sender, RoutedEventArgs e)
        {
            /*
            RequisicaoMaterialAlterar view = new();
            DocumentContainer.SetHeader(view, "ALTERAR REQUISIÇÕES DE MATERIAL");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000) / 2.0, (this._mdi.ActualHeight - 600) / 2.0, 1000, 600));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */

            adicionarFilho(new RequisicaoMaterialAlterar(), "ALTERAR REQUISIÇÕES DE MATERIAL", "ALTERAR_REQUISICOES_MATERIAL");
        }

        private async void OnRequisicoesEmitidas(object sender, RoutedEventArgs e)
        {
            /*
            RequisicaoMaterialEmitidas view = new();
            DocumentContainer.SetHeader(view, "REQUISIÇÕES DE MATERIAIS EMITIDAS");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000) / 2.0, (this._mdi.ActualHeight - 600) / 2.0, 1000, 600));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            //adicionarFilho(new RequisicaoMaterialEmitidas(), "REQUISIÇÕES DE MATERIAIS EMITIDAS", "REQUISICOES_MATERIAIS_EMITIDAS");

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.RequisicoesProducao.ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/REQUISICOES_MATERIAIS_EMITIDAS.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\REQUISICOES_MATERIAIS_EMITIDAS.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRequisicaClick(object sender, RoutedEventArgs e)
        {
            /*
            RequisicaoMaterialEmitir view = new();
            DocumentContainer.SetHeader(view, "EMITIR REQUISIÇÕES DE MATERIAL");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 250) / 2.0, (this._mdi.ActualHeight - 250.5) / 2.0, 250, 250.5));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new RequisicaoMaterialEmitir(), "EMITIR REQUISIÇÕES DE MATERIAL", "EMITIR_REQUISICOES_MATERIAL");
        }

        private void OnSolicitarOSProdutoClick(object sender, RoutedEventArgs e)
        {
            /*
            SolicitacaoOrdemServicoProduto view = new();
            DocumentContainer.SetHeader(view, "SOLICITAR ORDEM DE SERVIÇO DE PRODUTO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000) / 2.0, (this._mdi.ActualHeight - 600) / 2.0, 1000, 600));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new SolicitacaoOrdemServicoProduto(), "SOLICITAR ORDEM DE SERVIÇO DE PRODUTO", "SOLICITAR_ORDEM_SERVICO_PRODUTO");
        }

        private void OnSolicitarOSProdutoUnificadoClick(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new SolicitacaoOrdemServicoProdutoAgrupado(), "SOLICITAR ORDEM DE SERVIÇO DE PRODUTO UNIFICADO", "SOLICITAR_ORDEM_SERVICO_PRODUTO_UNIFICADO");
        }

        private void OnEmitirOSProdutoClick(object sender, RoutedEventArgs e)
        {
            /*
            EmitirOrdemServicoProduto view = new();
            DocumentContainer.SetHeader(view, "EMITIR ORDEM DE SERVIÇO DE PRODUTO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000) / 2.0, (this._mdi.ActualHeight - 768) / 2.0, 1000, 768));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new EmitirOrdemServicoProduto(), "EMITIR ORDEM DE SERVIÇO DE PRODUTO", "EMITIR_ORDEM_SERVICO_PRODUTO");
        }

        private void OnAlterarSolicitacaoOSProdutoClick(object sender, RoutedEventArgs e)
        {
            /*
            AlterarSolicitacaoOrdemServicoProduto view = new();
            DocumentContainer.SetHeader(view, "ALTERAR SOLICITAR ORDEM DE SERVIÇO DE PRODUTO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000) / 2.0, (this._mdi.ActualHeight - 600) / 2.0, 1000, 600));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new AlterarSolicitacaoOrdemServicoProduto(), "ALTERAR SOLICITAR ORDEM DE SERVIÇO DE PRODUTO", "ALTERAR_SOLICITAR_ORDEM_SERVICO_PRODUTO");
        }

        private async void OnPendenciaProducaoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.DetalhesPendenciaProducao.ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/PENDENCIA_PRODUCAO.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\PENDENCIA_PRODUCAO.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnPendenciaProducaoTreinamentoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.PendenciaProducaos.ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/PENDENCIA_PRODUCAO_TREINAMENTO.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\PENDENCIA_PRODUCAO_TREINAMENTO.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void ControleGrupoClick(object sender, RoutedEventArgs e)
        {
            /*
            ControleGrupo view = new();
            DocumentContainer.SetHeader(view, "CONTROLE POR GRUPO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 800.0) / 2.0, 1000.0, 800.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ControleGrupo(), "CONTROLE POR GRUPO", "CONTROLE_POR_GRUPO");
        }

        private void OnEntradaEstoqueClick(object sender, RoutedEventArgs e)
        {
            /*
            MovimentacaoEntrada view = new();
            DocumentContainer.SetHeader(view, "MOVIMENTAÇÃO ENTRADA ESTOQUE");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1024.00) / 2.0, (this._mdi.ActualHeight - 768.00) / 2.0, 1024.00, 768.00));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new MovimentacaoEntrada(), "MOVIMENTAÇÃO ENTRADA ESTOQUE", "MOVIMENTACAO_ENTRADA_ESTOQUE");
        }

        private void OnSaidaEstoqueClick(object sender, RoutedEventArgs e)
        {
            /*
            MovimentacaoSaida view = new();
            DocumentContainer.SetHeader(view, "MOVIMENTAÇÃO SAÍDA ESTOQUE");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1024.00) / 2.0, (this._mdi.ActualHeight - 768.00) / 2.0, 1024.00, 768.00));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new MovimentacaoSaida(), "MOVIMENTAÇÃO SAÍDA ESTOQUE", "MOVIMENTACAO_SAIDA_ESTOQUE");
        }

        private void OnBaixaRequisicaoClick(object sender, RoutedEventArgs e)
        {
            /*
            BaixaRequisicao view = new();
            DocumentContainer.SetHeader(view, "BAIXA DE REQUISIÇÃO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1024.00) / 2.0, (this._mdi.ActualHeight - 768.00) / 2.0, 1024.00, 768.00));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new BaixaRequisicao(), "BAIXA DE REQUISIÇÃO", "BAIXA_REQUISICAO");
        }

        private void OnSaldoEstoqueClick(object sender, RoutedEventArgs e)
        {
            /*
            SaldoEstoque view = new();
            DocumentContainer.SetHeader(view, "SALDO DE ESTOQUE");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 700) / 2.0, (this._mdi.ActualHeight - 250) / 2.0, 700, 250));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new SaldoEstoque(), "SALDO DE ESTOQUE", "SALDO_ESTOQUE");
        }

        private void OnRelatorioCCEClick(object sender, RoutedEventArgs e)
        {
            /*
            RelatorioCCE view = new();
            DocumentContainer.SetHeader(view, "RELATÓRIO C.C.E");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 400) / 2.0, (this._mdi.ActualHeight - 70) / 2.0, 400, 70));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new RelatorioCCE(), "RELATÓRIO C.C.E", "RELATORIO_CCE");
        }

        private void OnDigitarCCEClick(object sender, RoutedEventArgs e)
        {
            /*
            DigitacaoCCE view = new();
            DocumentContainer.SetHeader(view, "CONTROLE ESTOQUE PROCESSADO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1024.00) / 2.0, (this._mdi.ActualHeight - 768.00) / 2.0, 1024.00, 768.00));
            this._mdi.CanMDIMaximize = false;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new DigitacaoCCE(), "CONTROLE ESTOQUE PROCESSADO", "CONTROLE_ESTOQUE_PROCESSADO");
        }

        private void CompletarChecklistClick(object sender, RoutedEventArgs e)
        {
            /*
            ViewComplementoCheckListNatal view = new();
            DocumentContainer.SetHeader(view, "COMPLETAR CHECKLIST NATAL");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ViewComplementoCheckListNatal(), "COMPLETAR CHECKLIST NATAL", "COMPLETAR_CHECKLIST_NATAL");
        }

        private async void OnConsultaCCEClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                var data = await db.DetalhesProcessamentoSemanas.ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/CCE.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\CCE.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnBaixaServisoClick(object sender, RoutedEventArgs e)
        {
            /*
            ViewComplementoCheckListNatal view = new();
            DocumentContainer.SetHeader(view, "COMPLETAR CHECKLIST NATAL");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new BaixaOrdemServico(), "BAIXA ORDEM DE SERVIÇO", "BAIXA_ORDEM_SERVICO");
        }

        private void OnBaixaProdutoClick(object sender, RoutedEventArgs e)
        {
            /*
            BaixaOrdemServicoProduto view = new();
            DocumentContainer.SetHeader(view, "BAIXA ORDEM DE SERVIÇO PRODUTO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new BaixaOrdemServicoProduto(), "BAIXA ORDEM DE SERVIÇO PRODUTO", "BAIXA_ORDEM_SERVICO_PRODUTO");
        }

        private async void OnEmitidasClick(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    using DatabaseContext db = new();
                    var data = await db.OrdemServicoEmitidas.ToListAsync();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Xlsx;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    //worksheet.IsGridLinesVisible = false;
                    worksheet.ImportData(data, 1, 1, true);

                    workbook.SaveAs("Impressos/EMITIDAS.xlsx");
                    Process.Start(new ProcessStartInfo("Impressos\\EMITIDAS.xlsx")
                    {
                        UseShellExecute = true
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnConcluidasClick(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    using DatabaseContext db = new();
                    var data= await db.OrdemServicoEmitidas.Where(os => os.concluida_os_data != null).ToListAsync();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Xlsx;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    //worksheet.IsGridLinesVisible = false;
                    worksheet.ImportData(data, 1, 1, true);

                    workbook.SaveAs("Impressos/CONCLUIDAS.xlsx");
                    Process.Start(new ProcessStartInfo("Impressos\\CONCLUIDAS.xlsx")
                    {
                        UseShellExecute = true
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnNaoConcluidasClick(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    using DatabaseContext db = new();
                    var data = await db.OrdemServicoEmitidas.Where(os => os.concluida_os_data == null).ToListAsync();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Xlsx;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    //worksheet.IsGridLinesVisible = false;
                    worksheet.ImportData(data, 1, 1, true);

                    workbook.SaveAs("Impressos/NAO-CONCLUIDAS.xlsx");
                    Process.Start(new ProcessStartInfo("Impressos\\NAO-CONCLUIDAS.xlsx")
                    {
                        UseShellExecute = true
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnCanceladasClick(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    using DatabaseContext db = new();
                    var data = await db.OrdemServicoEmitidas.Where(os => os.cancelada_os == "-1").ToListAsync();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Xlsx;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    //worksheet.IsGridLinesVisible = false;
                    worksheet.ImportData(data, 1, 1, true);

                    workbook.SaveAs("Impressos/CANCELADAS.xlsx");
                    Process.Start(new ProcessStartInfo("Impressos\\CANCELADAS.xlsx")
                    {
                        UseShellExecute = true
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnProgramacaoProducaoClick(object sender, RoutedEventArgs e)
        {
            /*
            ProgramacaoProducao view = new();
            DocumentContainer.SetHeader(view, "PROGRAMAÇÃO DE PRODUÇÃO");
            DocumentContainer.SetSizetoContentInMDI(view, true);
            DocumentContainer.SetMDIBounds(view, new Rect((this._mdi.ActualWidth - 1000.0) / 2.0, (this._mdi.ActualHeight - 700.0) / 2.0, 1000.0, 700.0));
            DocumentContainer.SetMDIWindowState(view, MDIWindowState.Maximized);
            this._mdi.CanMDIMaximize = true;
            this._mdi.Items.Add(view);
            */
            adicionarFilho(new ProgramacaoProducao(), "PROGRAMAÇÃO DE PRODUÇÃO", "PROGRAMACAO_PRODUCAO");
        }

        private void _mdi_CloseButtonClick(object sender, CloseButtonEventArgs e)
        {
            var tab = (DocumentContainer)sender;
            _mdi.Items.Remove(tab.ActiveDocument);
        }

        private void _mdi_CloseAllTabs(object sender, CloseTabEventArgs e)
        {
            _mdi.Items.Clear();
        }

        private void OnImprimirEtiquetaControlado(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ImprimirEtiqueta(), "IMPRESSÃO ETIQUETA CONTROLADO", "IMPRESSAO_ETIQUETA_CONTROLADO");
        }

        private void OnVinculoRequisicao(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new VincularRequisicao(), "VINCULAR ETIQUETA A REQUISIÇÃO", "VINCULAR_ETIQUETA_REQUISICAO");
        }

        private void OnCadastroPecaConstrucao(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new CadastroPeca(), "CADASTRO PEÇAS CONSTRUÇÃO", "CADASTRO_PECAS_CONSTRUCAO");
        }

        private void OnCadastroConstrucao(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new EtiquetaConstrucao(), "CADASTRO DE CONSTRUÇÃO SHOPPING", "CADASTRO_CONSTRUCAO_SHOPPING");
        }

        private void OnLiberarProdutoBloqueadoClick(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new DesbloqueioAcertoEstoque(), "LIBERA PRODUTO BLOQUEADO", "LIBERA_PRODUTO_BLOQUEADO");
        }

        private void OnOpenMemorial(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ViewMemorial(), "MEMORIAL", "MEMORIAL");
        }

        private async void OnPlanejamentoEstoqueClick(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    using DatabaseContext db = new();
                    var data = await db.PlanejamentoEstoques.ToListAsync();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Xlsx;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    //worksheet.IsGridLinesVisible = false;
                    worksheet.ImportData(data, 1, 1, true);

                    workbook.SaveAs("Impressos/PLANEJAMENTO_ESTOQUE.xlsx");
                    Process.Start(new ProcessStartInfo("Impressos\\PLANEJAMENTO_ESTOQUE.xlsx")
                    {
                        UseShellExecute = true
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnAnaliseCliente(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.BaseAnaliseClientes.ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/ANALISE_CLIENTE.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\ANALISE_CLIENTE.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnAnalisePlanilha(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.BaseAnalisePlans.ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/ANALISE_PLANILHA.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\ANALISE_PLANILHA.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnKitSolucaoCheckList(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ViewKitSolucao(), "CHECKLIST KIT SOLUÇÃO", "CHECKLIST_KIT_SOLUCAO");
        }

        private async void OnQueryKitsolucaoGeral(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.KitSolicaoGeral.Where(c => c.local_shoppings == "KIT SOLUÇÃO").ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/KITSOLUCAO_GERAL.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\KITSOLUCAO_GERAL.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnControleGeral(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ViewControleGeralSolicitacao(), "CONTROLE GERAL DE SOLICITAÇÕES", "CONTROLE_GERAL_SOLICITACOES");
        }

        private void OnProdutosShopping(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ViewProdutoShopping(), "PRODUTO SHOPPING", "PRODUTO_SHOPPING");
        }

        private void OnKitManutencaoCheckList(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ViewKitManutencao(), "CHECKLIST KIT MANUTENÇÃO", "CHECKLIST_KIT_MANUTENCAO");
        }

        private async void OnQueryKitManutencaoGeral(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.KitSolicaoGeral.Where(c => c.local_shoppings == "KIT MANUTENÇÃO").ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/KITSOLUCAO_GERAL.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\KITSOLUCAO_GERAL.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //RadWindow.Prompt("ANO BANCO DE DADOS", this.OnClosed, BaseSettings.Database);
            //RadWindow.Prompt(new DialogParameters { Header = "ANO BANCO DE DADOS", Content = "INFORMA O ANO " }, this.OnClosed);

            RadWindow.Prompt(new DialogParameters()
            {
                Header = "Ano Sistema",
                Content = "Alterar o Ano do Sistema",
                Closed = (object sender, WindowClosedEventArgs e) => 
                {
                    if (e.PromptResult != null)
                    {
                        BaseSettings.Database = e.PromptResult;
                        txtDataBase.Text = BaseSettings.Database;
                        _mdi.Items.Clear();
                    }
                }
            });
        }

        private void OnClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.PromptResult != null)
            {
                BaseSettings.Database = e.PromptResult;
                txtDataBase.Text = BaseSettings.Database;
                _mdi.Items.Clear();
            }
                
            //var message = "Hello " + result + "!";
        }

        

        private void MenuItemAdv_Click_1(object sender, RoutedEventArgs e)
        {
            /*
            var alert = new RadDesktopAlert
            {
                Header = "INFORMAÇÃO SOBRE O SISTEMA",
                Content = $"VERSÃO: {string.Format("{0}.{1}.{2}.{3}", 1, 0, 0, 103)}",
                Height = 90,
                Width = 300,
                Background = Brushes.Red,

            };
            RadDesktopAlertManager manager = new();
            manager.ShowAlert(alert);
            */
        }

        private void OnRetornoControlado(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ControladoRecebimento(), "RECEBIMENTO DE CONTROLADO", "RECEBIMENTO_CONTROLADO");
        }

        private void OnRelatorioSigla(object sender, RoutedEventArgs e)
        {
            RadWindow.Prompt(
                new DialogParameters
                { 
                    Header = "RELATÓRIO POR CLIENTE", 
                    Content = "DIGITA A SIGLA DO CLIENTE", 
                    Closed = this.OnPrintReportControladoSigla 
                });
        }

        private async void OnPrintReportControladoSigla(object sender, WindowClosedEventArgs e)
        {
            var result = e.PromptResult.ToUpper();
            if (result != null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    using DatabaseContext db = new();
                    var data = await db.ProdutoControladoRecebimento.Where(c => c.sigla == result).ToListAsync();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Xlsx;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    worksheet.Range["A1"].Text = $"CONTROLADOS RETORNO SIGLA - {result}";
                    worksheet.Range["A1:F1"].Merge();
                    worksheet.Range["A1:F1"].CellStyle.Font.Bold = true;
                    worksheet.Range["A1:F1"].CellStyle.Font.Size = 20;
                    worksheet.Range["A1:F1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range["A1:F1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

                    IStyle headerStyle = workbook.Styles.Add("HeaderStyle");
                    headerStyle.BeginUpdate();
                    headerStyle.Font.Bold = true;
                    headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.EndUpdate();

                    IStyle bodyStyle = workbook.Styles.Add("bodyStyle");
                    bodyStyle.BeginUpdate();
                    bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.EndUpdate();

                    worksheet.Range["A3"].Text = "COD.";
                    worksheet.Range["B3"].Text = "PLANILHA";
                    worksheet.Range["C3"].Text = "DESCIÇÃO";
                    worksheet.Range["D3"].Text = "UNID.";
                    worksheet.Range["E3"].Text = "EXPEDIDO";
                    worksheet.Range["F3"].Text = "RETORNO";

                    worksheet.Rows[2].CellStyle = headerStyle;

                    int i = 3;
                    foreach (var row in data)
                    {
                        i++;
                        worksheet.Range[$"A{i}"].Number = Convert.ToDouble(row.codcompladicional);
                        worksheet.Range[$"A{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"B{i}"].Text = row.planilha;
                        worksheet.Range[$"B{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"C{i}"].Text = row.descricao;
                        worksheet.Range[$"C{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"D{i}"].Text = row.unidade;
                        worksheet.Range[$"D{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"E{i}"].Number = Convert.ToDouble(row.solucao_manutencao) + Convert.ToDouble(row.expedido);
                        worksheet.Range[$"E{i}"].CellStyle = bodyStyle;
                        worksheet.Range[$"E{i}"].NumberFormat = "0.00"; //"#,##0.00"; 

                        worksheet.Range[$"F{i}"].Text = "";
                        worksheet.Range[$"F{i}"].CellStyle = bodyStyle;

                    }

                    worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                    worksheet.PageSetup.FooterMargin = 0;
                    worksheet.PageSetup.HeaderMargin = 0;
                    worksheet.PageSetup.LeftMargin = 0;
                    worksheet.PageSetup.RightMargin = 0;
                    worksheet.PageSetup.TopMargin = 0;
                    worksheet.PageSetup.BottomMargin = 0;
                    worksheet.PageSetup.CenterHorizontally = true;
                    worksheet.PageSetup.CenterVertically = false;

                    worksheet.PageSetup.PrintTitleRows = "$1:$3";

                    worksheet.UsedRange.AutofitColumns();

                    workbook.SaveAs($"Impressos\\RELATORIO_CONTROLADO_{result}.xlsx");
                    Process.Start(new ProcessStartInfo($"Impressos\\RELATORIO_CONTROLADO_{result}.xlsx")
                    {
                        UseShellExecute = true
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnRelatorioPlanilha(object sender, RoutedEventArgs e)
        {
            RadWindow.Prompt(
                new DialogParameters
                {
                    Header = "RELATÓRIO POR PLANILHA",
                    Content = "DIGITA A PLANILHA",
                    Closed = this.OnPrintReportControladoPlanilha
                });
        }

        private async void OnPrintReportControladoPlanilha(object? sender, WindowClosedEventArgs e)
        {
            var result = e.PromptResult?.ToUpper();
            if (result != null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    using DatabaseContext db = new();
                    var data = await db.ProdutoControladoRecebimento.Where(c => c.planilha.Contains(result) && !c.sigla.Contains("SROOM")).ToListAsync();

                    using ExcelEngine excelEngine = new ExcelEngine();
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Xlsx;

                    //Create a workbook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    worksheet.Range["A1"].Text = $"CONTROLADOS RETORNO PLANILHA - {result}";
                    worksheet.Range["A1:F1"].Merge();
                    worksheet.Range["A1:F1"].CellStyle.Font.Bold = true;
                    worksheet.Range["A1:F1"].CellStyle.Font.Size = 20;
                    worksheet.Range["A1:F1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    worksheet.Range["A1:F1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

                    IStyle headerStyle = workbook.Styles.Add("HeaderStyle");
                    headerStyle.BeginUpdate();
                    headerStyle.Font.Bold = true;
                    headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    headerStyle.EndUpdate();

                    IStyle bodyStyle = workbook.Styles.Add("bodyStyle");
                    bodyStyle.BeginUpdate();
                    bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    bodyStyle.EndUpdate();

                    worksheet.Range["A3"].Text = "COD.";
                    worksheet.Range["B3"].Text = "SIGLA";
                    worksheet.Range["C3"].Text = "DESCIÇÃO";
                    worksheet.Range["D3"].Text = "UNID.";
                    worksheet.Range["E3"].Text = "EXPEDIDO";
                    worksheet.Range["F3"].Text = "RETORNO";

                    worksheet.Rows[2].CellStyle = headerStyle;

                    int i = 3;
                    foreach (var row in data)
                    {
                        i++;
                        worksheet.Range[$"A{i}"].Number = Convert.ToDouble(row.codcompladicional);
                        worksheet.Range[$"A{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"B{i}"].Text = row.sigla;
                        worksheet.Range[$"B{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"C{i}"].Text = row.descricao;
                        worksheet.Range[$"C{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"D{i}"].Text = row.unidade;
                        worksheet.Range[$"D{i}"].CellStyle = bodyStyle;

                        worksheet.Range[$"E{i}"].Number = Convert.ToDouble(row.solucao_manutencao) + Convert.ToDouble(row.expedido);
                        worksheet.Range[$"E{i}"].CellStyle = bodyStyle;
                        worksheet.Range[$"E{i}"].NumberFormat = "0.00"; //"#,##0.00"; 

                        worksheet.Range[$"F{i}"].Text = "";
                        worksheet.Range[$"F{i}"].CellStyle = bodyStyle;

                    }

                    worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                    worksheet.PageSetup.FooterMargin = 0;
                    worksheet.PageSetup.HeaderMargin = 0;
                    worksheet.PageSetup.LeftMargin = 0;
                    worksheet.PageSetup.RightMargin = 0;
                    worksheet.PageSetup.TopMargin = 0;
                    worksheet.PageSetup.BottomMargin = 0;
                    worksheet.PageSetup.CenterHorizontally = true;
                    worksheet.PageSetup.CenterVertically = false;

                    worksheet.PageSetup.PrintTitleRows = "$1:$3";

                    worksheet.UsedRange.AutofitColumns();

                    workbook.SaveAs($"Impressos\\RELATORIO_CONTROLADO_{result}.xlsx");
                    Process.Start(new ProcessStartInfo($"Impressos\\RELATORIO_CONTROLADO_{result}.xlsx")
                    {
                        UseShellExecute = true
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void OnHistoricoChecklistClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.HistoricoCheckList.Where(h => h.ano != Convert.ToInt16(BaseSettings.Database)).ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/HISTORICO-CHECK-LIST.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\HISTORICO-CHECK-LIST.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnAlterarUsuario(object sender, MouseButtonEventArgs e)
        {
            Login window = new();
            window.ShowDialog();

            try
            {
                var appSettings = ConfigurationManager.GetSection("appSettings") as NameValueCollection;
                BaseSettings.Username = appSettings[0];
                txtUsername.Text = BaseSettings.Username;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            /*
            RadWindow radWindow = new()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                HideMaximizeButton = true,
                HideMinimizeButton = true,
                Header = "Alterar Usuário",
                ResizeMode = ResizeMode.NoResize,
                CanMove = false,
                Content = new Login()
            };
            //radWindow.Content = grid;
            radWindow.ShowDialog();
            */
        }

        private void OnKitDesmontagemCheckList(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ViewKitDesmontagem(), "CHECKLIST KIT DESMONTAGEM", "CHECKLIST_KIT_DESMONTAGEM");
        }

        private async void OnQueryKitDesmontagemGeral(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                //var data = await db.PendenciaProducaos.ToListAsync();
                var data = await db.KitSolicaoGeral.Where(c => c.local_shoppings == "KIT DESMONTAGEM").ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/KITDESMONTAGEM_GERAL.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\KITDESMONTAGEM_GERAL.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRetornoEtiquetaManualClick(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new ControladoEtiquetaRetornoManual(), "CONTOLADO RETORNO MANUAL", "CONTOLADO_RETORNO_MANUAL");
        }

        private void OnEmitirOsDesbaiamento(object sender, RoutedEventArgs e)
        {
            adicionarFilho(new EmitirOSDesbaiamento(), "EMITIR ORDEM DE SERVIÇO DE DESBAIAMENTO", "EMITIR_ORDEM_SERVICO_DESBAIAMENTO");
        }

        private async void OnConsultaGeralClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                using DatabaseContext db = new();
                var data = await db.qryGeralRequisicaos.ToListAsync();

                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                //worksheet.IsGridLinesVisible = false;
                worksheet.ImportData(data, 1, 1, true);

                workbook.SaveAs("Impressos/CONTROLADO_GERAL.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\CONTROLADO_GERAL.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }
    }
}
