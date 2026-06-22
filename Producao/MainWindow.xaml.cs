using BibliotecasSIG;
using ClosedXML.Excel;
using Dapper;
using Npgsql;
using Producao.DataBase.Model.Dto;
using Producao.Views;
using Producao.Views.CadastroProduto;
using Producao.Views.CentralModelos;
using Producao.Views.CheckList;
using Producao.Views.Construcao;
using Producao.Views.Controlado;
using Producao.Views.Controle;
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
using Producao.Views.RelatoriosTecnicos;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
namespace Producao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;
        private readonly string CURRENT_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public MainWindow()
        {
            InitializeComponent();
            StyleManager.ApplicationTheme = new Windows11Theme();

            txtUsername.Text = BaseSettings.Username;
            txtDataBase.Text = BaseSettings.Database;
        }

        private string ExportToExcelAndOpen<T>(IEnumerable<T> data, string fileName)
        {
            var filePath = BaseSettings.ResolveImpressosPath(fileName);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Dados");
            worksheet.Cell(1, 1).InsertTable(data, "Dados", true);
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filePath);

            OpenFile(filePath);
            return filePath;
        }

        private string ExportDataTableToExcelAndOpen(DataTable dataTable, string fileName)
        {
            var filePath = BaseSettings.ResolveImpressosPath(fileName);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Dados");
            worksheet.Cell(1, 1).InsertTable(dataTable, "Dados", true);
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filePath);

            OpenFile(filePath);
            return filePath;
        }

        private static void OpenFile(string filePath)
        {
            Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            });
        }

        private async Task<DataTable> QueryDataTableAsync(string sql, object? parameters = null)
        {
            await using var conn = new NpgsqlConnection(BaseSettings.connectionString);
            await conn.OpenAsync();

            using var reader = await conn.ExecuteReaderAsync(
                new CommandDefinition(sql, parameters, commandTimeout: 300));

            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        private static object ConvertDataTableValue(object? value, Type targetType)
        {
            if (value is null)
                return DBNull.Value;

            if (value is DateOnly dateOnly && targetType == typeof(DateTime))
                return dateOnly.ToDateTime(TimeOnly.MinValue);

            if (value is TimeOnly timeOnly && targetType == typeof(TimeSpan))
                return timeOnly.ToTimeSpan();

            return value;
        }

        private async Task<List<T>> QueryListAsync<T>(string sql, object? parameters = null)
        {
            await using var conn = new NpgsqlConnection(BaseSettings.connectionString);
            var data = await conn.QueryAsync<T>(
                new CommandDefinition(sql, parameters, commandTimeout: 300));

            return data.AsList();
        }

        private string ExportControladoRetornoToExcelAndOpen(
            IEnumerable<ProdutoControladoRecebimentoModel> data,
            string title,
            string secondColumnHeader,
            Func<ProdutoControladoRecebimentoModel, object?> secondColumnValue,
            string fileName)
        {
            var filePath = BaseSettings.ResolveImpressosPath(fileName);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Dados");

            worksheet.Cell("A1").Value = title;
            worksheet.Range("A1:F1").Merge();
            worksheet.Range("A1:F1").Style.Font.Bold = true;
            worksheet.Range("A1:F1").Style.Font.FontSize = 20;
            worksheet.Range("A1:F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Range("A1:F1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            var headers = new[] { "COD.", secondColumnHeader, "DESCRIÇÃO", "UNID.", "EXPEDIDO", "RETORNO" };
            for (var column = 0; column < headers.Length; column++)
                worksheet.Cell(3, column + 1).Value = headers[column];

            var headerRange = worksheet.Range(3, 1, 3, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            var rowIndex = 4;
            foreach (var row in data)
            {
                worksheet.Cell(rowIndex, 1).Value = row.codcompladicional ?? 0;
                worksheet.Cell(rowIndex, 2).Value = XLCellValue.FromObject(secondColumnValue(row) ?? string.Empty);
                worksheet.Cell(rowIndex, 3).Value = row.descricao ?? string.Empty;
                worksheet.Cell(rowIndex, 4).Value = row.unidade ?? string.Empty;
                worksheet.Cell(rowIndex, 5).Value = (row.solucao_manutencao ?? 0) + (row.expedido ?? 0);
                worksheet.Cell(rowIndex, 6).Value = string.Empty;
                rowIndex++;
            }

            if (rowIndex > 4)
            {
                var bodyRange = worksheet.Range(4, 1, rowIndex - 1, headers.Length);
                bodyRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                bodyRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(4, 5, rowIndex - 1, 5).Style.NumberFormat.Format = "0.00";
            }

            worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            worksheet.PageSetup.Margins.Footer = 0;
            worksheet.PageSetup.Margins.Header = 0;
            worksheet.PageSetup.Margins.Left = 0;
            worksheet.PageSetup.Margins.Right = 0;
            worksheet.PageSetup.Margins.Top = 0;
            worksheet.PageSetup.Margins.Bottom = 0;
            worksheet.PageSetup.CenterHorizontally = true;
            worksheet.PageSetup.CenterVertically = false;
            worksheet.PageSetup.SetRowsToRepeatAtTop(1, 3);
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
            OpenFile(filePath);
            return filePath;
        }

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

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BaseSettings.UpdateInfoUrl))
                {
                    MessageBox.Show(
                        "O endereco de atualizacao nao esta configurado.",
                        "Atualizacao do sistema",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var updateChecker = new UpdateChecker(BaseSettings.UpdateInfoUrl, CURRENT_VERSION);
                var updateInfo = await updateChecker.CheckForUpdatesAsync();

                if (updateInfo == null)
                {
                    MessageBox.Show(
                        $"O sistema ja esta atualizado.\n\nVersao instalada: {CURRENT_VERSION}",
                        "Atualizacao do sistema",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Nova versao disponivel!\n\n" +
                    $"Versao atual: {CURRENT_VERSION}\n" +
                    $"Nova versao: {updateInfo.updateVersion}\n\n" +
                    "Alteracoes:\n" +
                    string.Join("\n", updateInfo.changelog) +
                    "\n\nDeseja atualizar o sistema agora?",
                    "Atualizacao disponivel",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result != MessageBoxResult.Yes)
                    return;

                string updateExecutable = Path.Combine(AppContext.BaseDirectory, "Update.exe");
                if (!File.Exists(updateExecutable))
                    throw new FileNotFoundException("O atualizador Update.exe nao foi encontrado.", updateExecutable);

                string jsonData = JsonSerializer.Serialize(updateInfo);
                var startInfo = new ProcessStartInfo(updateExecutable)
                {
                    UseShellExecute = true,
                    WorkingDirectory = AppContext.BaseDirectory
                };
                startInfo.ArgumentList.Add(jsonData);
                startInfo.ArgumentList.Add("Producao.exe");

                Process.Start(startInfo);
                Application.Current.Shutdown();
            }
            catch (HttpRequestException ex)
            {
                // Log do erro ou tratamento de exceção
                MessageBox.Show(
                    $"Erro ao verificar atualizações: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao verificar atualizações: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public void adicionarFilho(object filho, string title, string name)
        {
            var paneGroup = radDocking.FindChildByType<RadPaneGroup>();
            if (paneGroup == null)
            {
                return;
            }

            var pane = ExistDocumentInDocumentContainer(paneGroup, name, title);
            if (pane != null)
            {
                paneGroup.SelectedItem = pane;
                pane.IsActive = true;
                return;
            }

            var doc = filho as UserControl ?? filho as FrameworkElement;
            if (doc == null)
            {
                return;
            }

            doc.Name = name.ToLower();
            pane = new RadPane
            {
                Header = title,
                Content = doc,
                Tag = name.ToUpperInvariant(),
                CanUserClose = true,
                CanFloat = false
            };

            paneGroup.Items.Add(pane);
            paneGroup.SelectedItem = pane;
            pane.IsActive = true;
        }

        private static RadPane? ExistDocumentInDocumentContainer(RadPaneGroup paneGroup, string name_, string title)
        {
            var normalizedName = name_.ToUpperInvariant();
            return paneGroup.Items.OfType<RadPane>()
                .FirstOrDefault(p =>
                    string.Equals(p.Tag as string, normalizedName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(p.Header?.ToString(), title, StringComparison.OrdinalIgnoreCase));
        }

        private void MenuItemAdv_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnChecklist_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnRevisaoChecklistClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnEtiquetaChecklistClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnEtiquetaChecklistEmitidaClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnCentralCriarModelo(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnCentralTabelaPa(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnCentralFatorConversao(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnCentralEmitirOs(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnCentralStatusCheckList(object sender, Telerik.Windows.RadRoutedEventArgs e)
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


        private void OnCreateReceitaRequisicao(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnCadastroProduto(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnCadastroEspanhol(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnTodasDescricoes(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnEmitirOSServicoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnEmitidasOSServicoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnAlterarRequisicoes(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private async void OnRequisicoesEmitidas(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

                var data = await QueryDataTableAsync("SELECT * FROM producao.qry_requisicao_producao;");
                ExportDataTableToExcelAndOpen(data, "REQUISICOES_MATERIAIS_EMITIDAS.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRequisicaClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnSolicitarOSProdutoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnSolicitarOSProdutoUnificadoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new SolicitacaoOrdemServicoProdutoAgrupado(), "SOLICITAR ORDEM DE SERVIÇO DE PRODUTO UNIFICADO", "SOLICITAR_ORDEM_SERVICO_PRODUTO_UNIFICADO");
        }

        private void OnEmitirOSProdutoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnAlterarSolicitacaoOSProdutoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private async void OnPendenciaProducaoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync("SELECT * FROM pcp.detalhes_pendencia_producao;");
                ExportDataTableToExcelAndOpen(data, "PENDENCIA_PRODUCAO.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnPendenciaProducaoTreinamentoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync("SELECT * FROM producao.qry_pendencia_producao;");
                ExportDataTableToExcelAndOpen(data, "PENDENCIA_PRODUCAO_TREINAMENTO.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void ControleGrupoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnEntradaEstoqueClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnSaidaEstoqueClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnBaixaRequisicaoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnSaldoEstoqueClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnRelatorioCCEClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnDigitarCCEClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void CompletarChecklistClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private async void OnConsultaCCEClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync("SELECT * FROM producao.qry_detalhes_processamento_semana;");
                ExportDataTableToExcelAndOpen(data, "CCE.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnBaixaServisoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnBaixaProdutoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private async void OnEmitidasClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    var data = await QueryDataTableAsync("SELECT * FROM producao.qry_os_emitidas;");
                    ExportDataTableToExcelAndOpen(data, "EMITIDAS.xlsx");

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

        private async void OnConcluidasClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    var data = await QueryDataTableAsync("SELECT * FROM producao.qry_os_emitidas WHERE concluida_os_data IS NOT NULL;");
                    ExportDataTableToExcelAndOpen(data, "CONCLUIDAS.xlsx");

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

        private async void OnNaoConcluidasClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    var data = await QueryDataTableAsync("SELECT * FROM producao.qry_os_emitidas WHERE concluida_os_data IS NULL;");
                    ExportDataTableToExcelAndOpen(data, "NAO-CONCLUIDAS.xlsx");

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

        private async void OnCanceladasClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    var data = await QueryDataTableAsync("SELECT * FROM producao.qry_os_emitidas WHERE cancelada_os = @cancelada;", new { cancelada = "-1" });
                    ExportDataTableToExcelAndOpen(data, "CANCELADAS.xlsx");

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

        private void OnProgramacaoProducaoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnImprimirEtiquetaControlado(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ImprimirEtiqueta(), "IMPRESSÃO ETIQUETA CONTROLADO", "IMPRESSAO_ETIQUETA_CONTROLADO");
        }

        private void OnVinculoRequisicao(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new VincularRequisicao(), "VINCULAR ETIQUETA A REQUISIÇÃO", "VINCULAR_ETIQUETA_REQUISICAO");
        }

        private void OnCadastroPecaConstrucao(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new CadastroPeca(), "CADASTRO PEÇAS CONSTRUÇÃO", "CADASTRO_PECAS_CONSTRUCAO");
        }

        private void OnCadastroConstrucao(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new EtiquetaConstrucao(), "CADASTRO DE CONSTRUÇÃO SHOPPING", "CADASTRO_CONSTRUCAO_SHOPPING");
        }

        private void OnLiberarProdutoBloqueadoClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new DesbloqueioAcertoEstoque(), "LIBERA PRODUTO BLOQUEADO", "LIBERA_PRODUTO_BLOQUEADO");
        }

        private void OnOpenMemorial(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ViewMemorial(), "MEMORIAL", "MEMORIAL");
        }

        private async void OnPlanejamentoEstoqueClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    var data = await QueryDataTableAsync("SELECT * FROM producao.pcp_planejamento_estoque;");
                    ExportDataTableToExcelAndOpen(data, "PLANEJAMENTO_ESTOQUE.xlsx");

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

        private async void OnAnaliseCliente(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync("SELECT * FROM producao.qrybaseanalisecliente1b;");
                ExportDataTableToExcelAndOpen(data, "ANALISE_CLIENTE.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnAnalisePlanilha(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync("SELECT * FROM producao.qrybaseanaliseplan;");
                ExportDataTableToExcelAndOpen(data, "ANALISE_PLANILHA.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnKitSolucaoCheckList(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ViewKitSolucao(), "CHECKLIST KIT SOLUÇÃO", "CHECKLIST_KIT_SOLUCAO");
        }

        private async void OnQueryKitsolucaoGeral(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync(
                    "SELECT * FROM kitsolucao.query_kitsolicao_geral WHERE local_shoppings = @local;",
                    new { local = "KIT SOLUÇÃO" });
                ExportDataTableToExcelAndOpen(data, "KITSOLUCAO_GERAL.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnControleGeral(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ViewControleGeralSolicitacao(), "CONTROLE GERAL DE SOLICITAÇÕES", "CONTROLE_GERAL_SOLICITACOES");
        }

        private void OnProdutosShopping(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ViewProdutoShopping(), "PRODUTO SHOPPING", "PRODUTO_SHOPPING");
        }

        private void OnKitManutencaoCheckList(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ViewKitManutencao(), "CHECKLIST KIT MANUTENÇÃO", "CHECKLIST_KIT_MANUTENCAO");
        }

        private async void OnQueryKitManutencaoGeral(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync(
                    "SELECT * FROM kitsolucao.query_kitsolicao_geral WHERE local_shoppings = @local;",
                    new { local = "KIT MANUTENÇÃO" });
                ExportDataTableToExcelAndOpen(data, "KITMANUTENCAO_GERAL.xlsx");

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
                        BaseSettings.connectionString = $"Host={BaseSettings.Host};Database={BaseSettings.Database};Username={BaseSettings.Username};Password={BaseSettings.Password}";
                        documentGroup.Items.Clear();
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
                documentGroup.Items.Clear();
            }
                
            //var message = "Hello " + result + "!";
        }

        

        private void MenuItemAdv_Click_1(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

        private void OnRetornoControlado(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ControladoRecebimento(), "RECEBIMENTO DE CONTROLADO", "RECEBIMENTO_CONTROLADO");
        }

        private void OnRelatorioSigla(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

                    var data = await QueryListAsync<ProdutoControladoRecebimentoModel>(
                        "SELECT * FROM expedicao.view_produtos_controlas_recebimento WHERE sigla = @sigla;",
                        new { sigla = result });

                    ExportControladoRetornoToExcelAndOpen(
                        data,
                        $"CONTROLADOS RETORNO SIGLA - {result}",
                        "PLANILHA",
                        row => row.planilha,
                        $"RELATORIO_CONTROLADO_{result}.xlsx");

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnRelatorioPlanilha(object sender, Telerik.Windows.RadRoutedEventArgs e)
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

                    var data = await QueryListAsync<ProdutoControladoRecebimentoModel>(
                        @"SELECT *
                          FROM expedicao.view_produtos_controlas_recebimento
                          WHERE planilha LIKE @planilha
                            AND (sigla IS NULL OR sigla NOT LIKE @sigla);",
                        new { planilha = $"%{result}%", sigla = "%SROOM%" });

                    ExportControladoRetornoToExcelAndOpen(
                        data,
                        $"CONTROLADOS RETORNO PLANILHA - {result}",
                        "SIGLA",
                        row => row.sigla,
                        $"RELATORIO_CONTROLADO_{result}.xlsx");

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void OnHistoricoChecklistClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                const string sql = @"
                    SELECT ano, sigla, tema, ordem, item_memorial, local_shoppings, obs, orient_montagem, orient_desmont, qry3descricoes.planilha, qtd_chk, qry3descricoes.codcompladicional, qry3descricoes.descricao_completa, qtd_comple, m3 AS m3_media_produto_unitario, m3 * qtd_comple::numeric(12,2) AS m3_media_produto_total
                    FROM producao.view_checklist_completo_historico
                    LEFT JOIN producao.qry3descricoes ON view_checklist_completo_historico.codcompladicional = qry3descricoes.codcompladicional
                    ORDER BY ano, sigla, tema, ordem, item_memorial;
                ";

                await using var conn = new NpgsqlConnection(BaseSettings.connectionString);
                await conn.OpenAsync();
                var data = await conn.QueryAsync<HistoricoCheckListExcelDTO>(sql);
                ExportToExcelAndOpen(data.ToList(), "HISTORICO-CHECK-LIST.xlsx");

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

        private void OnKitDesmontagemCheckList(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ViewKitDesmontagem(), "CHECKLIST KIT DESMONTAGEM", "CHECKLIST_KIT_DESMONTAGEM");
        }

        private async void OnQueryKitDesmontagemGeral(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync(
                    "SELECT * FROM kitsolucao.query_kitsolicao_geral WHERE local_shoppings = @local;",
                    new { local = "KIT DESMONTAGEM" });
                ExportDataTableToExcelAndOpen(data, "KITDESMONTAGEM_GERAL.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private static void ExportKitToXlsx<T>(IEnumerable<T> data, string caminhoArquivo)
        {
            var workbook = new Telerik.Windows.Documents.Spreadsheet.Model.Workbook();
            var worksheet = workbook.Worksheets.Add();
            worksheet.Name = "Consulta";

            var properties = typeof(T).GetProperties();

            for (var column = 0; column < properties.Length; column++)
            {
                worksheet.Cells[0, column].SetValueAsText(properties[column].Name);
            }

            var row = 1;
            foreach (var item in data)
            {
                for (var column = 0; column < properties.Length; column++)
                {
                    var value = properties[column].GetValue(item);
                    var cell = worksheet.Cells[row, column];

                    switch (value)
                    {
                        case null:
                            cell.SetValueAsText(string.Empty);
                            break;
                        case DateTime dateTime:
                            cell.SetValueAsText(dateTime.ToString("dd/MM/yyyy HH:mm:ss"));
                            break;
                        case DateOnly dateOnly:
                            cell.SetValueAsText(dateOnly.ToString("dd/MM/yyyy"));
                            break;
                        case byte byteValue:
                            cell.SetValue(byteValue);
                            break;
                        case short shortValue:
                            cell.SetValue(shortValue);
                            break;
                        case int intValue:
                            cell.SetValue(intValue);
                            break;
                        case long longValue:
                            cell.SetValue((double)longValue);
                            break;
                        case decimal decimalValue:
                            cell.SetValue((double)decimalValue);
                            break;
                        case float floatValue:
                            cell.SetValue((double)floatValue);
                            break;
                        case double doubleValue:
                            cell.SetValue(doubleValue);
                            break;
                        case bool boolValue:
                            cell.SetValueAsText(boolValue ? "SIM" : "NÃO");
                            break;
                        default:
                            cell.SetValueAsText(value.ToString() ?? string.Empty);
                            break;
                    }
                }

                row++;
            }

            worksheet.Cells[0, 0, 0, Math.Max(properties.Length - 1, 0)].SetIsBold(true);

            using var output = File.Open(caminhoArquivo, FileMode.Create);
            new Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx.XlsxFormatProvider()
                .Export(workbook, output, TimeSpan.FromSeconds(30));
        }

        private void OnRetornoEtiquetaManualClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ControladoEtiquetaRetornoManual(), "CONTOLADO RETORNO MANUAL", "CONTOLADO_RETORNO_MANUAL");
        }

        private void OnEmitirOsDesbaiamento(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new EmitirOSDesbaiamento(), "EMITIR ORDEM DE SERVIÇO DE DESBAIAMENTO", "EMITIR_ORDEM_SERVICO_DESBAIAMENTO");
        }

        private async void OnConsultaGeralClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var data = await QueryDataTableAsync("SELECT * FROM producao.qry_geral_requisicao;");
                ExportDataTableToExcelAndOpen(data, "CONTROLADO_GERAL.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnConsultaReceitaRequisicao(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                string sql = @"
                    SELECT 
                        codcompladicional_produto AS CodComplAdicionalProduto,
                        p.planilha AS PlanilhaProduto,
                        p.descricao_completa AS DescricaoProduto,
                        p.unidade AS UnidadeProduto,
                        codcompladicional_receita AS CodComplAdicionalReceita,
                        r.planilha AS PlanilhaReceita,
                        r.descricao_completa AS DescricaoReceita,
                        r.unidade AS UnidadeReceita,
                        quantidade AS Quantidade,
                        inserido_por AS InseridoPor,
                        inserido_em AS InseridoEm
                    FROM producao.tbl_requisicao_receita
                    JOIN producao.qry3descricoes AS p ON tbl_requisicao_receita.codcompladicional_produto = p.codcompladicional
                    JOIN producao.qry3descricoes AS r ON tbl_requisicao_receita.codcompladicional_receita = r.codcompladicional
                ";

                using var connection = new NpgsqlConnection(BaseSettings.connectionString);
                connection.Open();

                var resultado = connection.Query<RequisicaoReceitaDTO>(sql).ToList();
                ExportToExcelAndOpen(resultado, "CONSULTA_RECEITA_GERAL.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnCargaEletrica(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new CargaEletrica(), "CARGA ELÉTRICA APROXIMADA", "CARGA_ELETRICA");
        }

        private void OnEstabilidade(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new Estabilidade(), "RELATÓRIO DE ESTABILIDADE", "ESTABILIDADE");
        }

        private void OnInflamabilidade(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new Inflamabilidade(), "RELATÓRIO DE INFLAMABILIDADE", "INFLAMABILIDADE");
        }

        private void OnOpenRelPlanClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new Relplan(), "RELPLAN", "RELPLAN");
        }

        private void OnAreasTemas(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new AreaTema(), "ÁREAS TEMAS", "AREAS_TEMAS");
        }

        private async void OnConsultaEntradaEstoqueClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                string sql = @"
                    SELECT 
                        t_entrada_estoque.codcompladicional, 
                        planilha,
                        descricao_completa,
                        unidade,
                        quantidade, 
                        procedencia, 
                        entrada_data, 
                        entrada_por, 
                        codigo_entrada, 
                        local_galpao, 
                        endereco, 
                        quantidade_fisica, 
                        processado
                    FROM producao.t_entrada_estoque
                    JOIN producao.qry3descricoes ON producao.t_entrada_estoque.codcompladicional = producao.qry3descricoes.codcompladicional
                    ORDER BY planilha, descricao_completa;
                ";

                await using var connection = new NpgsqlConnection(BaseSettings.connectionString);
                await connection.OpenAsync();
                var resultado = (await connection.QueryAsync(sql)).ToList();
                DataTable table = new();

                // Define colunas com tipos específicos
                table.Columns.Add("codcompladicional", typeof(int));
                table.Columns.Add("planilha", typeof(string));
                table.Columns.Add("descricao_completa", typeof(string));
                table.Columns.Add("unidade", typeof(string));
                table.Columns.Add("quantidade", typeof(decimal));
                table.Columns.Add("procedencia", typeof(string));
                table.Columns.Add("entrada_data", typeof(DateTime));
                table.Columns.Add("entrada_por", typeof(string));
                table.Columns.Add("codigo_entrada", typeof(int));
                table.Columns.Add("local_galpao", typeof(string));
                table.Columns.Add("endereco", typeof(string));
                table.Columns.Add("quantidade_fisica", typeof(decimal));
                table.Columns.Add("processado", typeof(string));

                // Adiciona os dados linha a linha
                foreach (var row in resultado)
                {
                    var dict = (IDictionary<string, object>)row;
                    var novaLinha = table.NewRow();

                    foreach (DataColumn col in table.Columns)
                    {
                        // Verifica se a chave existe no dicionário antes de atribuir
                        dict.TryGetValue(col.ColumnName, out var valor);
                        novaLinha[col.ColumnName] = ConvertDataTableValue(valor, col.DataType);
                    }

                    table.Rows.Add(novaLinha);
                }
                ExportDataTableToExcelAndOpen(table, "CONSULTA_MOVEMENTACAO_ENTRADA.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnConsultaSaidaEstoqueClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                string sql = @"
                    SELECT
                        t_saida.codcompladicional,
                        planilha,
                        descricao_completa,
                        unidade,
                        quantidade, 
                        destino, 
                        saida_data, 
                        saida_por, 
                        codigo_saida, 
                        observacao, 
                        local_galpao, 
                        num_requisicao, 
                        caminho, 
                        endereco, 
                        quantidade_fisica, 
                        processado
                    FROM producao.t_saida
                    JOIN producao.qry3descricoes ON producao.t_saida.codcompladicional = producao.qry3descricoes.codcompladicional
                    ORDER BY planilha, descricao_completa;
                ";

                await using var connection = new NpgsqlConnection(BaseSettings.connectionString);
                await connection.OpenAsync();
                var resultado = (await connection.QueryAsync(sql)).ToList();
                DataTable table = new();

                // Define colunas com tipos específicos
                table.Columns.Add("codcompladicional", typeof(int));
                table.Columns.Add("planilha", typeof(string));
                table.Columns.Add("descricao_completa", typeof(string));
                table.Columns.Add("unidade", typeof(string));
                table.Columns.Add("quantidade", typeof(decimal));
                table.Columns.Add("destino", typeof(string));
                table.Columns.Add("saida_data", typeof(DateTime));
                table.Columns.Add("saida_por", typeof(string));
                table.Columns.Add("codigo_saida", typeof(int));
                table.Columns.Add("observacao", typeof(string));
                table.Columns.Add("local_galpao", typeof(string));
                table.Columns.Add("num_requisicao", typeof(int));
                table.Columns.Add("caminho", typeof(string));
                table.Columns.Add("endereco", typeof(string));
                table.Columns.Add("quantidade_fisica", typeof(decimal));
                table.Columns.Add("processado", typeof(string));

                // Adiciona os dados linha a linha
                foreach (var row in resultado)
                {
                    var dict = (IDictionary<string, object>)row;
                    var novaLinha = table.NewRow();

                    foreach (DataColumn col in table.Columns)
                    {
                        // Verifica se a chave existe no dicionário antes de atribuir
                        dict.TryGetValue(col.ColumnName, out var valor);
                        novaLinha[col.ColumnName] = ConvertDataTableValue(valor, col.DataType);
                    }

                    table.Rows.Add(novaLinha);
                }
                ExportDataTableToExcelAndOpen(table, "CONSULTA_MOVEMENTACAO_SAIDA.xlsx");

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void OnProdutosCusto(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new CustoProduto(), "CUSTO PRODUTO", "CUSTO_PRODUTO");
        }

        private void OnOpenControlePlantaClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new Planta(), "CONTROLE PLANTAS", "CONTROLE_PLANTA");
        }

        private async void OnAtualizarSistemaClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var menuItem = sender as RadMenuItem;
            if (menuItem != null)
                menuItem.IsEnabled = false;

            try
            {
                await CheckForUpdatesAsync();
            }
            finally
            {
                if (menuItem != null)
                    menuItem.IsEnabled = true;
            }
        }

        private void OnSobreSistemaClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            MessageBox.Show(
                $"Sistema Integrado de Gerenciamento - Produção\n\nVersão atual: {CURRENT_VERSION}",
                "Sobre o sistema",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

    }
}

