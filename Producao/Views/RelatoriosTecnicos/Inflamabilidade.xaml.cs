using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Producao.DataBase.Model;
using Syncfusion.Pdf.Graphics;
using System.Collections.Generic;
using Syncfusion.Pdf;
using System.Drawing;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using System.Globalization;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using System.Diagnostics;

namespace Producao.Views.RelatoriosTecnicos
{
    /// <summary>
    /// Interação lógica para Inflamabilidade.xam
    /// </summary>
    public partial class Inflamabilidade : UserControl
    {
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
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                vm.Responsaveis = await Task.Run(vm.GetResponsaveisAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                vm.Inflamabilidade = await Task.Run(() => vm.GetvInflamabilidadeAsync(vm.Sigla));
                if (vm.Inflamabilidade == null)
                    await Task.Run(() => vm.SaveInflamabilidadeAsync(vm.Sigla));

                vm.Responsavel = vm.Responsaveis.FirstOrDefault(r => r.nome == vm.Inflamabilidade?.responsavel);
                vm.Detalhes = await Task.Run(() => vm.GetDetalhesAsync(vm.Sigla));
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnResponsavelSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                vm.Inflamabilidade.responsavel = vm.Responsavel.nome;
                await Task.Run(() => vm.SaveInflamabilidadeAsync(vm.Inflamabilidade));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void DetalhesRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            try
            {
                var dado = e.RowData as InflamabilidadeDetalhe;
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(() => vm.SaveDetalhesAsync(new InflamabilidadeDetalheModel { sigla = dado.sigla, tipo = dado.tipo, classificacao = dado.classificacao}));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void SfTextBoxExt_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //Inflamabilidade.rrt
                //OriginalSource = {Syncfusion.Windows.Controls.Input.SfTextBoxExt: 123}
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                await Task.Run(() => vm.SaveInflamabilidadeAsync(vm.Inflamabilidade));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }


        public async void CreateDynamicPdfReport()
        {
            try
            {

                InflamabilidadeViewModel vm = (InflamabilidadeViewModel)DataContext;
                if (vm.Sigla == null)
                    return;

                PdfDocument document = new PdfDocument();
                document.PageSettings.Orientation = PdfPageOrientation.Portrait;
                document.PageSettings.Margins.All = 20;
                PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
                PdfFont fontBold = new PdfStandardFont(PdfFontFamily.TimesRoman, 15, PdfFontStyle.Bold);
                PdfBrush brush = PdfBrushes.Black;

                DateTime dataAtual = DateTime.Now;
                string cidade = "São Paulo";


                vm.Cliente = await Task.Run(() => vm.GetClienteAsync(vm.Sigla));

                // Formatar a data conforme desejado
                string dataFormatada = $"{cidade}, {dataAtual:dd MMMM} de {dataAtual:yyyy}";

                PdfPage page = document.Pages.Add();

                PdfGraphics graphics = page.Graphics;

                PdfImage image = PdfImage.FromFile("LOCACAO_NOVO.png");
                RectangleF bounds = new RectangleF(220, 0, 100, 40);
                page.Graphics.DrawImage(image, bounds);

                graphics.DrawString(dataFormatada, font, brush, new PointF(0, 60));
                graphics.DrawString("Ao", font, brush, new PointF(0, 80));
                graphics.DrawString($"{vm.Cliente.nome}", font, brush, new PointF(0, 100));
                graphics.DrawString($"{vm.Cliente.cidade} - {vm.Cliente.est}", font, brush, new PointF(0, 120));
                graphics.DrawString("Ref. Relatório de Inflamabilidade", fontBold, brush, new PointF(0, 170));
                graphics.DrawString("Este relatório foi elaborado para a perfeita aplicação dos agentes extintores aos\r\nnossos produtos, classificando-os segundo a natureza dos corpos em\r\ncombustão.\r\nSendo a decoração composta de vários tipos de materiais, apresentamos a\r\nseguir a lista dos agentes combustíveis mais significativos, seguida de suas\r\nqualidades individuais.\r\nConcluindo, o procedimento de combate propriamente dito.\r\nFicaremos a disposição para quaisquer esclarecimentos que se fizerem\r\nnecessários", font, brush, new PointF(0, 210));


                graphics.DrawString("Atenciosamente,", font, brush, new PointF(0, 500));
                graphics.DrawString($"{vm.Responsavel.nome}", font, brush, new PointF(0, 560));
                graphics.DrawString($"RG. {vm.Responsavel.rg}", font, brush, new PointF(0, 570));
                graphics.DrawString($"CAU/SP. {vm.Responsavel.cau_sp}", font, brush, new PointF(0, 580));
                graphics.DrawString($"RRT. {vm.Inflamabilidade.rrt}", font, brush, new PointF(0, 590));


                PdfPage page2 = document.Pages.Add();
                page2.Graphics.DrawString("Informações dos Materiais Predominantes na Decoração", fontBold, brush, new PointF(0, 0));

                string[] inflamabilidadesDesejadas = ["1", "2", "3"];
                var itens = vm.Detalhes.Where(m => inflamabilidadesDesejadas.Contains(m.classificacao)).ToArray();

                float yPositionTipo = 60;
                float yPositionDescritivo = 90;

                float yPositionExtraTipo = 0;
                float yPositionExtraDescritivo = 30;

                PdfPage pageExtra = new();

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
                            pageExtra = document.Pages.Add();

                        pageExtra.Graphics.DrawString($"{item.tipo}", fontBold, brush, new PointF(0, yPositionExtraTipo));
                        pageExtra.Graphics.DrawString($"{item.descritivo}", font, brush, new PointF(0, yPositionExtraDescritivo));
                        pageExtra.Graphics.DrawString($"{classificacao.FirstOrDefault()}", font, brush, new PointF(185, yPositionExtraDescritivo + 14));

                        yPositionExtraTipo += 120;
                        yPositionExtraDescritivo += 120;
                    }
                    else 
                    {
                        page2.Graphics.DrawString($"{item.tipo}", fontBold, brush, new PointF(0, yPositionTipo));
                        page2.Graphics.DrawString($"{item.descritivo}", font, brush, new PointF(0, yPositionDescritivo));
                        page2.Graphics.DrawString($"{classificacao.FirstOrDefault()}", font, brush, new PointF(185, yPositionDescritivo + 14));

                        //page.Graphics.DrawString(materialInfo, font, brush, new RectangleF(0, yPosition, page.GetClientSize().Width, page.GetClientSize().Height), format);
                        yPositionTipo += 120;
                        yPositionDescritivo += 120;
                    }

                    det++;

                }


                PdfPage page3 = document.Pages.Add();
                page3.Graphics.DrawString("Classes de Incêndio Quanto à Forma de Combate", fontBold, brush, new PointF(0, 0));
                page3.Graphics.DrawString("CLASSE A", fontBold, brush, new PointF(0, 60));
                page3.Graphics.DrawString("Constituído por materiais que tem a propriedade de queimar em sua superfície e\r\nprofundidade e deixam resíduos. Para Extinção, necessita-se o resfriamento e\r\npenetração do agente extintor. Exemplo: madeira, couro, papel, algodão,m cereais,\r\netc.", font, brush, new PointF(0, 90));
                page3.Graphics.DrawString("CLASSE C", fontBold, brush, new PointF(0, 160));
                page3.Graphics.DrawString("Constituído por equipamentos elétricos com energia, caracterizando-se por\r\noferecer riscos a quem irá combate-lo. Para a sua extinção é necessário usar um\r\nagente extintor não condutor de eletricidade. Quando a rede elétrica é desligada,\r\nnão havendo mais energia, o incêndio torna-se Classe A.", font, brush, new PointF(0, 190));
                page3.Graphics.DrawString("CONCLUSÃO", fontBold, brush, new PointF(0, 260));
                page3.Graphics.DrawString("Lembrando que a nossa decoração é provida de micro-lâmpadas, lâmpadas,\r\nmotores e equipamentos elétricos em carga, o procedimento de combate a um\r\npossível incêndio deve ser executado em duas fases.\r\nCombate inicial classe C, não em face do material que queima, mas sim pelo risco\r\nque oferece ao operador na sua extinção. Para este tipo de incêndio é necessário\r\nusar um agente extintor não condutor de eletricidade (Extintores de gás\r\ncarbônico ou pó químico seco) e desligar a rede elétrica tão rápido quanto\r\npossível.\r\nQuando a rede elétrica for desligada o incêndio torna-se de Classe A, podendo\r\nser tratado com hidrante e extintores de água ou espuma.", font, brush, new PointF(0, 290));
                
                
                page3.Graphics.DrawString("Atenciosamente,", font, brush, new PointF(0, 500));
                page3.Graphics.DrawString($"{vm.Responsavel.nome}", font, brush, new PointF(0, 560));
                page3.Graphics.DrawString($"RG. {vm.Responsavel.rg}", font, brush, new PointF(0, 570));
                page3.Graphics.DrawString($"CAU/SP. {vm.Responsavel.cau_sp}", font, brush, new PointF(0, 580));
                page3.Graphics.DrawString($"RRT. {vm.Inflamabilidade.rrt}", font, brush, new PointF(0, 590));

                string[] footerLines =
                [
                    "CIPOLATTI CIPOLATTI LOCAÇÃO E COMÉRCIO LTDA.",
                    "Av. Doutor Luis Migliano, 1110/104 – Jardim Caboré, São Paulo - SP - Cep 05711-001",
                    "FONE: (5511) 4788-4166",
                    "Home page: www.cipolatti.com.br",
                    "E-mail: cipolatti@cipolatti.com.br"
                ];

                float yPosition = page.GetClientSize().Height - 60; // Posição inicial do rodapé
                float lineSpacing = 12; // Espaçamento entre as linhas

                foreach (string line in footerLines)
                {
                    // Calcula a largura do texto
                    SizeF textSize = font.MeasureString(line);

                    // Calcula a posição do texto para centralizá-lo
                    float xPosition = (page.GetClientSize().Width - textSize.Width) / 2;

                    // Adiciona o texto do rodapé
                    page.Graphics.DrawString(line, font, brush, new PointF(xPosition, yPosition));
                    page2.Graphics.DrawString(line, font, brush, new PointF(xPosition, yPosition));
                    page3.Graphics.DrawString(line, font, brush, new PointF(xPosition, yPosition));
                    if (det > 5)
                    {
                        pageExtra.Graphics.DrawString(line, font, brush, new PointF(xPosition, yPosition));
                    }


                    // Atualiza a posição y para a próxima linha
                    yPosition += lineSpacing;
                }


                document.Save("RelatorioInflamabilidade.pdf");
                //Close the document.
                document.Close(true);

                
                Process.Start(new ProcessStartInfo("RelatorioInflamabilidade.pdf")
                {
                    UseShellExecute = true
                });
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void OnConcluirClick(object sender, RoutedEventArgs e)
        {
           
            CreateDynamicPdfReport();

            //MessageBox.Show("Relatório PDF dinâmico criado com sucesso!");
        }
    }

    public class InflamabilidadeViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName) { this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); }

        private ObservableCollection<string> _siglas;
        public ObservableCollection<string> Siglas { get { return _siglas; } set { _siglas = value; RaisePropertyChanged("Siglas"); } }

        private string _sigla;
        public string Sigla { get { return _sigla; } set { _sigla = value; RaisePropertyChanged("Sigla"); } }

        private ObservableCollection<InflamabilidadeResponsavelModel> _responsaveis;
        public ObservableCollection<InflamabilidadeResponsavelModel> Responsaveis { get { return _responsaveis; } set { _responsaveis = value; RaisePropertyChanged("Responsaveis"); } }

        private InflamabilidadeResponsavelModel _responsavel;
        public InflamabilidadeResponsavelModel Responsavel { get { return _responsavel; } set { _responsavel = value; RaisePropertyChanged("Responsavel"); } }

        private InflamabilidadeModel _inflamabilidade;
        public InflamabilidadeModel Inflamabilidade { get { return _inflamabilidade; } set { _inflamabilidade = value; RaisePropertyChanged("Inflamabilidade"); } }

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
                using DatabaseContext db = new();
                var query = await db.Clientes.FindAsync(sigla);

                return query;
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
                using DatabaseContext db = new();
                var query = from p in db.Siglas
                                   group p by p.sigla
                                   into g
                                   select g.Key ;

                return new ObservableCollection<string>(await query.ToListAsync());
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
                using DatabaseContext db = new();
                var query = await db.InflamabilidadeResponsaveis.ToListAsync();

                return new ObservableCollection<InflamabilidadeResponsavelModel>(query);
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
                using DatabaseContext db = new();
                var query = await db.Inflamabilidades.FindAsync(sigla);

                return query;
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
                using DatabaseContext db = new();
                //var query = await db.InflamabilidadeResponsaveis.ToListAsync();
                var resultado = from detalhes in db.InflamabilidadeDetalhes
                                join materiais in db.MaterialPredominanteDecoracoes
                                on detalhes.tipo equals materiais.tipo
                                where detalhes.sigla == sigla
                                select new InflamabilidadeDetalhe
                                {
                                    sigla = detalhes.sigla,
                                    tipo = detalhes.tipo,
                                    classificacao = detalhes.classificacao,
                                    descritivo = materiais.descritivo
                                };

                var listaResultado = await resultado.ToListAsync();

                return new ObservableCollection<InflamabilidadeDetalhe>(listaResultado);
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
                using DatabaseContext db = new();
                db.Inflamabilidades.Update(inflamabilidade);
                await db.SaveChangesAsync();
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
                using DatabaseContext db = new();
                db.InflamabilidadeDetalhes.Update(detalheModel);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveInflamabilidadeAsync(string sigla)
        {

            using DatabaseContext db = new();

            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await db.Inflamabilidades.AddAsync(new InflamabilidadeModel { sigla = sigla });
                        await db.SaveChangesAsync();
                        var materiais = await db.MaterialPredominanteDecoracoes.ToListAsync();
                        foreach (var item in materiais)
                        {
                            await db.InflamabilidadeDetalhes.AddAsync(new InflamabilidadeDetalheModel { sigla = sigla, tipo = item.tipo });
                        }
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
    }
}
