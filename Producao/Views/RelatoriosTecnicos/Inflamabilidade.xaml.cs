using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Fonts;

namespace Producao.Views.RelatoriosTecnicos
{
    /// <summary>
    /// Interação lógica para Inflamabilidade.xam
    /// </summary>
    public partial class Inflamabilidade : UserControl
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public Inflamabilidade()
        {
            InitializeComponent();
            DataContext = new InflamabilidadeViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                vm.Responsaveis = await vm.GetResponsaveisAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                if (sender is RadComboBox combo)
                    vm.Sigla = combo.SelectedItem as string;

                if (vm.Sigla == null)
                {
                    vm.Responsavel = null;
                    vm.Detalhes = [];
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Inflamabilidade = await vm.GetvInflamabilidadeAsync(vm.Sigla);
                if (vm.Inflamabilidade == null)
                {
                    await vm.SaveInflamabilidadeAsync(vm.Sigla);
                    vm.Inflamabilidade = await vm.GetvInflamabilidadeAsync(vm.Sigla);
                }

                vm.Responsavel = vm.Responsaveis.FirstOrDefault(r => r.nome == vm.Inflamabilidade?.responsavel);
                vm.Detalhes = await vm.GetDetalhesAsync(vm.Sigla);
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnResponsavelSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                if (sender is RadComboBox combo)
                    vm.Responsavel = combo.SelectedItem as InflamabilidadeResponsavelModel;

                if (vm.Responsavel == null)
                {
                    vm.Inflamabilidade = null;

                    return;
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Inflamabilidade.responsavel = vm.Responsavel.nome;
                await vm.SaveInflamabilidadeAsync(vm.Inflamabilidade);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void DetalhesCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            try
            {
                if (e.Cell.Column.UniqueName != "classificacao")
                    return;

                var grid = sender as RadGridView;
                var dado = grid?.Items.CurrentEditItem as InflamabilidadeDetalhe ?? e.Cell.DataContext as InflamabilidadeDetalhe;

                if (dado == null)
                    return;

                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await vm.SaveDetalhesAsync(new InflamabilidadeDetalheModel { sigla = dado.sigla, tipo = dado.tipo, classificacao = dado.classificacao});
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void Rrt_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                await vm.SaveInflamabilidadeAsync(vm.Inflamabilidade);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }


        public async void CreateDynamicPdfReport()
        {
            try
            {

                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                if (vm.Sigla == null)
                    return;

                DateTime dataAtual = DateTime.Now;
                string cidade = "São Paulo";


                vm.Cliente = await vm.GetClienteAsync(vm.Sigla);

                // Formatar a data conforme desejado
                string dataFormatada = $"{cidade}, {dataAtual:dd MMMM} de {dataAtual:yyyy}";

                RadFixedDocument document = new();
                RadFixedPage page = AddPdfPage(document);

                DrawImage(page, "LOCACAO_NOVO.png", 220, 0, 100, 40);
                DrawText(page, dataFormatada, 0, 60);
                DrawText(page, "Ao", 0, 80);
                DrawText(page, $"{vm.Cliente.nome}", 0, 100);
                DrawText(page, $"{vm.Cliente.cidade} - {vm.Cliente.est}", 0, 120);
                DrawText(page, "Ref. Relatório de Inflamabilidade", 0, 170, bold: true, fontSize: 15);
                DrawMultilineText(page, "Este relatório foi elaborado para a perfeita aplicação dos agentes extintores aos\r\nnossos produtos, classificando-os segundo a natureza dos corpos em\r\ncombustão.\r\nSendo a decoração composta de vários tipos de materiais, apresentamos a\r\nseguir a lista dos agentes combustíveis mais significativos, seguida de suas\r\nqualidades individuais.\r\nConcluindo, o procedimento de combate propriamente dito.\r\nFicaremos a disposição para quaisquer esclarecimentos que se fizerem\r\nnecessários", 0, 210);


                DrawText(page, "Atenciosamente,", 0, 500);
                DrawText(page, $"{vm.Responsavel?.nome}", 0, 560);
                DrawText(page, $"RG. {vm.Responsavel?.rg}", 0, 570);
                DrawText(page, $"CAU/SP. {vm.Responsavel?.cau_sp}", 0, 580);
                DrawText(page, $"RRT. {vm.Inflamabilidade?.rrt}", 0, 590);


                RadFixedPage page2 = AddPdfPage(document);
                DrawText(page2, "Informações dos Materiais Predominantes na Decoração", 0, 0, bold: true, fontSize: 15);

                string[] inflamabilidadesDesejadas = ["1", "2", "3"];
                var itens = vm.Detalhes.Where(m => inflamabilidadesDesejadas.Contains(m.classificacao)).ToArray();

                double yPositionTipo = 60;
                double yPositionDescritivo = 90;

                double yPositionExtraTipo = 0;
                double yPositionExtraDescritivo = 30;

                RadFixedPage? pageExtra = null;

                int det = 1;
                foreach (var item in itens)
                {
                    //var classificacao = vm.Classificacoes.Where(c => c.id == item.classificacao).Select()

                    var classificacao = from p in vm.Classificacoes
                                        where p.id == item.classificacao
                                        select p.classificacao;

                    if (det > 5)
                    {
                        if(det == 6)
                            pageExtra = AddPdfPage(document);

                        DrawText(pageExtra!, $"{item.tipo}", 0, yPositionExtraTipo, bold: true, fontSize: 15);
                        DrawText(pageExtra!, $"{item.descritivo}", 0, yPositionExtraDescritivo);
                        DrawText(pageExtra!, $"{classificacao.FirstOrDefault()}", 185, yPositionExtraDescritivo + 14);

                        yPositionExtraTipo += 120;
                        yPositionExtraDescritivo += 120;
                    }
                    else 
                    {
                        DrawText(page2, $"{item.tipo}", 0, yPositionTipo, bold: true, fontSize: 15);
                        DrawText(page2, $"{item.descritivo}", 0, yPositionDescritivo);
                        DrawText(page2, $"{classificacao.FirstOrDefault()}", 185, yPositionDescritivo + 14);

                        //page.Graphics.DrawString(materialInfo, font, brush, new RectangleF(0, yPosition, page.GetClientSize().Width, page.GetClientSize().Height), format);
                        yPositionTipo += 120;
                        yPositionDescritivo += 120;
                    }

                    det++;

                }


                RadFixedPage page3 = AddPdfPage(document);
                DrawText(page3, "Classes de Incêndio Quanto à Forma de Combate", 0, 0, bold: true, fontSize: 15);
                DrawText(page3, "CLASSE A", 0, 60, bold: true, fontSize: 15);
                DrawMultilineText(page3, "Constituído por materiais que tem a propriedade de queimar em sua superfície e\r\nprofundidade e deixam resíduos. Para Extinção, necessita-se o resfriamento e\r\npenetração do agente extintor. Exemplo: madeira, couro, papel, algodão,m cereais,\r\netc.", 0, 90);
                DrawText(page3, "CLASSE C", 0, 160, bold: true, fontSize: 15);
                DrawMultilineText(page3, "Constituído por equipamentos elétricos com energia, caracterizando-se por\r\noferecer riscos a quem irá combate-lo. Para a sua extinção é necessário usar um\r\nagente extintor não condutor de eletricidade. Quando a rede elétrica é desligada,\r\nnão havendo mais energia, o incêndio torna-se Classe A.", 0, 190);
                DrawText(page3, "CONCLUSÃO", 0, 260, bold: true, fontSize: 15);
                DrawMultilineText(page3, "Lembrando que a nossa decoração é provida de micro-lâmpadas, lâmpadas,\r\nmotores e equipamentos elétricos em carga, o procedimento de combate a um\r\npossível incêndio deve ser executado em duas fases.\r\nCombate inicial classe C, não em face do material que queima, mas sim pelo risco\r\nque oferece ao operador na sua extinção. Para este tipo de incêndio é necessário\r\nusar um agente extintor não condutor de eletricidade (Extintores de gás\r\ncarbônico ou pó químico seco) e desligar a rede elétrica tão rápido quanto\r\npossível.\r\nQuando a rede elétrica for desligada o incêndio torna-se de Classe A, podendo\r\nser tratado com hidrante e extintores de água ou espuma.", 0, 290);
                
                
                DrawText(page3, "Atenciosamente,", 0, 500);
                DrawText(page3, $"{vm.Responsavel?.nome}", 0, 560);
                DrawText(page3, $"RG. {vm.Responsavel?.rg}", 0, 570);
                DrawText(page3, $"CAU/SP. {vm.Responsavel?.cau_sp}", 0, 580);
                DrawText(page3, $"RRT. {vm.Inflamabilidade?.rrt}", 0, 590);

                string[] footerLines =
                [
                    "CIPOLATTI CIPOLATTI LOCAÇÃO E COMÉRCIO LTDA.",
                    "Av. Doutor Luis Migliano, 1110/104 – Jardim Caboré, São Paulo - SP - Cep 05711-001",
                    "FONE: (5511) 4788-4166",
                    "Home page: www.cipolatti.com.br",
                    "E-mail: cipolatti@cipolatti.com.br"
                ];

                double yPosition = PageHeight - PageMargin - 60; // Posição inicial do rodapé
                double lineSpacing = 12; // Espaçamento entre as linhas

                foreach (string line in footerLines)
                {
                    double textWidth = EstimateTextWidth(line, 12);
                    double xPosition = ((PageWidth - (PageMargin * 2)) - textWidth) / 2;

                    // Adiciona o texto do rodapé
                    DrawText(page, line, xPosition, yPosition);
                    DrawText(page2, line, xPosition, yPosition);
                    DrawText(page3, line, xPosition, yPosition);


                    // Atualiza a posição y para a próxima linha
                    yPosition += lineSpacing;
                }


                string reportPath = Path.Combine(BaseSettings.CaminhoSistema, "RelatorioInflamabilidade.pdf");
                PdfFormatProvider provider = new();
                using (Stream output = File.Open(reportPath, FileMode.Create))
                    provider.Export(document, output);

                
                Process.Start(new ProcessStartInfo(reportPath)
                {
                    UseShellExecute = true
                });
                

            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            
        }

        private const double PageWidth = 595;
        private const double PageHeight = 842;
        private const double PageMargin = 20;

        private static RadFixedPage AddPdfPage(RadFixedDocument document)
        {
            RadFixedPage page = document.Pages.AddPage();
            page.Size = new Size(PageWidth, PageHeight);
            return page;
        }

        private static void DrawImage(RadFixedPage page, string imagePath, double x, double y, double width, double height)
        {
            string path = Path.Combine(AppContext.BaseDirectory, imagePath);
            if (!File.Exists(path))
                path = imagePath;

            if (!File.Exists(path))
                return;

            FixedContentEditor editor = new(page);
            editor.Position.Translate(PageMargin + x, PageMargin + y);
            using Stream imageStream = File.OpenRead(path);
            editor.DrawImage(imageStream, width, height);
        }

        private static void DrawText(RadFixedPage page, string? text, double x, double y, bool bold = false, double fontSize = 12)
        {
            FixedContentEditor editor = new(page);
            editor.Position.Translate(PageMargin + x, PageMargin + y);
            editor.TextProperties.Font = bold ? FontsRepository.TimesBold : FontsRepository.TimesRoman;
            editor.TextProperties.FontSize = fontSize;
            editor.DrawText(text ?? string.Empty);
        }

        private static void DrawMultilineText(RadFixedPage page, string text, double x, double y, double fontSize = 12, double lineSpacing = 14)
        {
            foreach (string line in text.Replace("\r\n", "\n").Split('\n'))
            {
                DrawText(page, line, x, y, fontSize: fontSize);
                y += lineSpacing;
            }
        }

        private static double EstimateTextWidth(string text, double fontSize)
        {
            return text.Length * fontSize * 0.45;
        }

        private async void OnConcluirClick(object sender, RoutedEventArgs e)
        {

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                vm.Inflamabilidade.concluido_por = Environment.UserName;
                vm.Inflamabilidade.data_conclusao = DateTime.Now;
                await vm.SaveInflamabilidadeAsync(vm.Inflamabilidade);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void OnImprimirClick(object sender, RoutedEventArgs e)
        {
            CreateDynamicPdfReport();
        }
    }

    public class InflamabilidadeViewModel : INotifyPropertyChanged
    {
        private static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName) { this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); }

        private ObservableCollection<string> _siglas;
        public ObservableCollection<string> Siglas { get { return _siglas; } set { _siglas = value; RaisePropertyChanged("Siglas"); } }

        private string _sigla;
        public string Sigla { get { return _sigla; } set { _sigla = value; RaisePropertyChanged("Sigla"); } }

        private ObservableCollection<InflamabilidadeResponsavelModel> _responsaveis;
        public ObservableCollection<InflamabilidadeResponsavelModel> Responsaveis { get { return _responsaveis; } set { _responsaveis = value; RaisePropertyChanged("Responsaveis"); } }

        private InflamabilidadeResponsavelModel _responsavel;
        public InflamabilidadeResponsavelModel? Responsavel { get { return _responsavel; } set { _responsavel = value; RaisePropertyChanged("Responsavel"); } }

        private InflamabilidadeModel _inflamabilidade;
        public InflamabilidadeModel? Inflamabilidade { get { return _inflamabilidade; } set { _inflamabilidade = value; RaisePropertyChanged("Inflamabilidade"); } }

        private ObservableCollection<InflamabilidadeDetalhe> _detalhes;
        public ObservableCollection<InflamabilidadeDetalhe> Detalhes { get { return _detalhes; } set { _detalhes = value; RaisePropertyChanged("Detalhes"); } }

        private InflamabilidadeDetalhe _detalhe;
        public InflamabilidadeDetalhe Detalhe { get { return _detalhe; } set { _detalhe = value; RaisePropertyChanged("Detalhe"); } }

        private ObservableCollection<InflamabilidadeCassificacao> _classificacoes =
        [
            new InflamabilidadeCassificacao { id = "1", classificacao = "Pequeno"},
            new InflamabilidadeCassificacao { id = "2", classificacao = "Médio"},
            new InflamabilidadeCassificacao { id = "3", classificacao = "Grande"},
            new InflamabilidadeCassificacao { id = "4", classificacao = "Não tem"}
        ];
        public ObservableCollection<InflamabilidadeCassificacao> Classificacoes { get { return _classificacoes; } set { _classificacoes = value; RaisePropertyChanged("Classificacoes"); } }

        private ClienteModel _cliente;
        public ClienteModel Cliente { get { return _cliente; } set { _cliente = value; RaisePropertyChanged("Cliente"); } }


        public async Task<ClienteModel> GetClienteAsync(string sigla)
        {
            try
            {
                using var conn = CreateConnection();
                return await conn.QueryFirstOrDefaultAsync<ClienteModel>(
                    @"SELECT sigla, nome, endereco, cidade, bairro, est
                      FROM comercial.clientes
                      WHERE sigla = @sigla;",
                    new { sigla });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<string>> GetSiglasAsync()
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<string>(
                    @"SELECT sigla
                      FROM producao.view_sigla_chkgeral
                      GROUP BY sigla
                      ORDER BY sigla;");
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<InflamabilidadeResponsavelModel>> GetResponsaveisAsync()
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<InflamabilidadeResponsavelModel>(
                    @"SELECT nome, rg, cau_sp
                      FROM projetos.tbl_rpt_inflamabilidade_responsavel
                      ORDER BY nome;");
                return new ObservableCollection<InflamabilidadeResponsavelModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<InflamabilidadeModel> GetvInflamabilidadeAsync(string sigla)
        {
            try
            {
                using var conn = CreateConnection();
                return await conn.QueryFirstOrDefaultAsync<InflamabilidadeModel>(
                    @"SELECT sigla, rrt, responsavel, data_conclusao, concluido_por
                      FROM projetos.tbl_rpt_inflamabilidade
                      WHERE sigla = @sigla;",
                    new { sigla });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<InflamabilidadeDetalhe>> GetDetalhesAsync(string sigla)
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<InflamabilidadeDetalhe>(
                    @"SELECT d.sigla,
                             d.tipo,
                             d.classificacao,
                             m.descritivo
                      FROM projetos.tbl_rpt_inflamabilidade_detalhes d
                      JOIN projetos.tbl_materiais_predominantes_decoracao m
                        ON d.tipo = m.tipo
                      WHERE d.sigla = @sigla
                      ORDER BY d.tipo;",
                    new { sigla });
                return new ObservableCollection<InflamabilidadeDetalhe>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveInflamabilidadeAsync(InflamabilidadeModel inflamabilidade)
        {
            try
            {
                if (inflamabilidade?.sigla == null)
                    return;

                using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    @"INSERT INTO projetos.tbl_rpt_inflamabilidade
                        (sigla, rrt, responsavel, data_conclusao, concluido_por)
                      VALUES
                        (@sigla, @rrt, @responsavel, @data_conclusao, @concluido_por)
                      ON CONFLICT (sigla) DO UPDATE SET
                        rrt = EXCLUDED.rrt,
                        responsavel = EXCLUDED.responsavel,
                        data_conclusao = EXCLUDED.data_conclusao,
                        concluido_por = EXCLUDED.concluido_por;",
                    inflamabilidade);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveDetalhesAsync(InflamabilidadeDetalheModel detalheModel)
        {
            try
            {
                if (detalheModel.sigla == null || detalheModel.tipo == null)
                    return;

                using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    @"INSERT INTO projetos.tbl_rpt_inflamabilidade_detalhes
                        (sigla, tipo, classificacao)
                      VALUES
                        (@sigla, @tipo, @classificacao)
                      ON CONFLICT (sigla, tipo) DO UPDATE SET
                        classificacao = EXCLUDED.classificacao;",
                    detalheModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveInflamabilidadeAsync(string sigla)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                await conn.ExecuteAsync(
                    @"INSERT INTO projetos.tbl_rpt_inflamabilidade (sigla)
                      VALUES (@sigla)
                      ON CONFLICT (sigla) DO NOTHING;",
                    new { sigla },
                    transaction);

                await conn.ExecuteAsync(
                    @"INSERT INTO projetos.tbl_rpt_inflamabilidade_detalhes (sigla, tipo)
                      SELECT @sigla, tipo
                      FROM projetos.tbl_materiais_predominantes_decoracao
                      ON CONFLICT (sigla, tipo) DO NOTHING;",
                    new { sigla },
                    transaction);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
