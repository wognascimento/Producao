using Producao.Views.CentralModelos.Compat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Producao.Views.OrdemServico
{
    internal sealed class OrdemServicoModeloPrinter
    {
        private readonly DataBaseSettings baseSettings = DataBaseSettings.Instance;

        public async Task ImprimirAsync(
            IEnumerable<OsEmissaoProducaoImprimirModel> servicos,
            Func<long?, Task<IEnumerable<ProdutoServicoModel>>> buscarSetoresAsync)
        {
            var servicosOrdenados = servicos
                .Where(s => s is not null)
                .OrderBy(s => s.num_os_servico)
                .ToList();

            if (servicosOrdenados.Count == 0)
                return;

            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            for (var i = 0; i < servicosOrdenados.Count; i += 2)
            {
                var servicoSuperior = servicosOrdenados[i];
                var servicoInferior = i + 1 < servicosOrdenados.Count ? servicosOrdenados[i + 1] : null;

                using IWorkbook workbook = excelEngine.Excel.Workbooks.Open(baseSettings.ResolveModeloPath("ORDEM_SERVICO_MODELO.xlsx"));
                IWorksheet ordemServico = workbook.Worksheets[0];
                IWorksheet requisicao = workbook.Worksheets[1];

                var setoresSuperior = await buscarSetoresAsync(servicoSuperior.num_os_produto);
                PreencherBlocoSuperior(ordemServico, servicoSuperior, setoresSuperior);
                PreencherBlocoSuperior(requisicao, servicoSuperior, setoresSuperior);

                if (servicoInferior is not null)
                {
                    var setoresInferior = await buscarSetoresAsync(servicoInferior.num_os_produto);
                    ordemServico.ShowRange(ordemServico[27, 1, 53, 19], true);
                    requisicao.ShowRange(requisicao[27, 1, 53, 22], true);
                    PreencherBlocoInferior(ordemServico, servicoInferior, setoresInferior);
                    PreencherBlocoInferior(requisicao, servicoInferior, setoresInferior);
                }
                else
                {
                    ordemServico.ShowRange(ordemServico[27, 1, 53, 19], false);
                    requisicao.ShowRange(requisicao[27, 1, 53, 22], false);
                }

                ConfigurarImpressao(ordemServico, servicoInferior is null ? "A1:I26" : "A1:I54");
                ConfigurarImpressao(requisicao, servicoInferior is null ? "K1:V26" : "K1:V53");
                SelecionarAbasParaImpressao(workbook, ordemServico, requisicao);

                var sufixo = servicoInferior is null
                    ? servicoSuperior.num_os_servico.ToString()
                    : $"{servicoSuperior.num_os_servico}_{servicoInferior.num_os_servico}";

                var caminhoOs = baseSettings.ResolveImpressosPath($"ORDEM_SERVICO_{sufixo}.xlsx");
                workbook.SaveAs(caminhoOs);
                ImprimirArquivo(caminhoOs);

                ImprimirPermissaoTrabalhoSeNecessario(excelEngine, servicoSuperior);
                if (servicoInferior is not null)
                    ImprimirPermissaoTrabalhoSeNecessario(excelEngine, servicoInferior);
            }
        }

        private void ImprimirPermissaoTrabalhoSeNecessario(ExcelEngine excelEngine, OsEmissaoProducaoImprimirModel servico)
        {
            if (servico.pt != true)
                return;

            using IWorkbook workbook = excelEngine.Excel.Workbooks.Open(baseSettings.ResolveModeloPath("PERMISSAO_TRABALHO.xlsx"));
            IWorksheet worksheet = workbook.Worksheets[0];
            worksheet.Range["G1"].Number = Convert.ToDouble(servico.num_os_servico ?? 0);

            var caminhoPt = baseSettings.ResolveImpressosPath($"PERMISSAO_TRABALHO_{servico.num_os_servico}.xlsx");
            workbook.SaveAs(caminhoPt);
            ImprimirArquivo(caminhoPt);
        }

        private static void PreencherBlocoSuperior(
            IWorksheet worksheet,
            OsEmissaoProducaoImprimirModel servico,
            IEnumerable<ProdutoServicoModel> setores)
        {
            worksheet.Range["E2"].Text = servico.cliente;
            worksheet.Range["G2"].Text = servico.num_os_produto.ToString();
            worksheet.Range["I2"].Text = servico.num_os_servico.ToString();
            worksheet.Range["B4"].Text = servico.tipo;
            worksheet.Range["D4"].Text = $"{servico.data_inicio:dd/MM/yy}";
            worksheet.Range["B5"].Text = servico.setor_caminho;
            worksheet.Range["F5"].Text = servico.solicitado_por;
            worksheet.Range["G4"].Text = $"META HT: {servico.meta_peca_hora}";
            worksheet.Range["B6"].Text = servico.planilha;
            worksheet.Range["F6"].Text = Convert.ToString(servico.cod_compl_adicional);
            worksheet.Range["B7"].Text = servico.descricao_completa;
            worksheet.Range["G7"].Text = $"{servico.data_de_expedicao:dd/MM/yy}";
            worksheet.Range["B9"].Text = Convert.ToString(servico.quantidade);
            worksheet.Range["D9"].Text = Convert.ToString(servico.nivel);
            worksheet.Range["B10"].Text = servico.setor_caminho_proximo;
            worksheet.Range["B11"].Text = servico.tema;
            worksheet.Range["A13"].Text = servico.orientacao_caminho;
            worksheet.Range["A23"].Text = servico.laco;
            worksheet.Range["A25"].Text = servico.obs_iluminacao;

            PreencherSetores(worksheet, setores, 9, 17);
        }

        private static void PreencherBlocoInferior(
            IWorksheet worksheet,
            OsEmissaoProducaoImprimirModel servico,
            IEnumerable<ProdutoServicoModel> setores)
        {
            worksheet.Range["E29"].Text = servico.cliente;
            worksheet.Range["G29"].Text = servico.num_os_produto.ToString();
            worksheet.Range["I29"].Text = servico.num_os_servico.ToString();
            worksheet.Range["B31"].Text = servico.tipo;
            worksheet.Range["D31"].Text = $"{servico.data_inicio:dd/MM/yy}";
            worksheet.Range["B32"].Text = servico.setor_caminho;
            worksheet.Range["F32"].Text = servico.solicitado_por;
            worksheet.Range["G31"].Text = $"META HT: {servico.meta_peca_hora}";
            worksheet.Range["B33"].Text = servico.planilha;
            worksheet.Range["F33"].Text = Convert.ToString(servico.cod_compl_adicional);
            worksheet.Range["B34"].Text = servico.descricao_completa;
            worksheet.Range["G34"].Text = $"{servico.data_de_expedicao:dd/MM/yy}";
            worksheet.Range["B36"].Text = Convert.ToString(servico.quantidade);
            worksheet.Range["D36"].Text = Convert.ToString(servico.nivel);
            worksheet.Range["B37"].Text = servico.setor_caminho_proximo;
            worksheet.Range["B38"].Text = servico.tema;
            worksheet.Range["A40"].Text = servico.orientacao_caminho;
            worksheet.Range["A50"].Text = servico.laco;
            worksheet.Range["A52"].Text = servico.obs_iluminacao;

            PreencherSetores(worksheet, setores, 37, 45);
        }

        private static void PreencherSetores(IWorksheet worksheet, IEnumerable<ProdutoServicoModel> setores, int primeiraLinha, int linhaLimite)
        {
            for (var linhaLimpar = primeiraLinha; linhaLimpar < linhaLimite; linhaLimpar++)
                worksheet.Range[$"G{linhaLimpar}"].Text = string.Empty;

            var linha = primeiraLinha;
            foreach (var setor in setores)
            {
                worksheet.Range[$"G{linha}"].Text = setor.setor_caminho;
                linha++;
                if (linha == linhaLimite)
                    break;
            }
        }

        private static void SelecionarAbasParaImpressao(IWorkbook workbook, IWorksheet ordemServico, IWorksheet requisicao)
        {
            ordemServico.TabSelected = true;
            ordemServico.TabActive = true;
            requisicao.TabSelected = true;

            workbook.Worksheets[2].Hide();
        }

        private static void ConfigurarImpressao(IWorksheet worksheet, string areaImpressao)
        {
            worksheet.PageSetup.PrintArea = areaImpressao;
            worksheet.PageSetup.CenterHorizontally = true;
            worksheet.PageSetup.CenterVertically = false;
            worksheet.PageSetup.FitToPagesWide = 1;
            worksheet.PageSetup.FitToPagesTall = 1;
        }

        private static void ImprimirArquivo(string caminho)
        {
            Process.Start(new ProcessStartInfo(caminho)
            {
                Verb = "Print",
                UseShellExecute = true,
            });
        }
    }
}
