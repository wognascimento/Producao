using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Producao.Views.kit
{
    internal static class KitRequisicaoExporter
    {
        private const string SenhaProtecao = "1@3mudar";

        public static void Exportar(string titulo, IEnumerable<KitChkGeralModel> dados, string caminhoArquivo)
        {
            var itens = dados.ToList();
            var cabecalho = itens.FirstOrDefault();

            ExcelPackage.License.SetNonCommercialOrganization("SIG");

            var caminhoModelo = DataBaseSettings.Instance.ResolveModeloPath("REQUISICAO_KIT_MODELO.xlsx");
            using var package = new ExcelPackage(new FileInfo(caminhoModelo));

            var requisicao = package.Workbook.Worksheets[0];
            var etiquetas = package.Workbook.Worksheets.Count > 1
                ? package.Workbook.Worksheets[1]
                : package.Workbook.Worksheets.Add("Etiquetas");
            Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(requisicao);
            Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(etiquetas);

            PreencherCabecalho(requisicao, titulo, cabecalho);
            PreencherItens(requisicao, itens);
            PreencherEtiquetas(etiquetas, itens);
            Proteger(package);

            package.SaveAs(new FileInfo(caminhoArquivo));

            Process.Start(new ProcessStartInfo(caminhoArquivo)
            {
                UseShellExecute = true
            });
        }

        private static void PreencherCabecalho(ExcelWorksheet worksheet, string titulo, KitChkGeralModel? cabecalho)
        {
            SetText(worksheet, "A1", titulo);

            if (cabecalho == null)
                return;

            SetText(worksheet, "B2", cabecalho.sigla);
            SetText(worksheet, "H2", cabecalho.cidade);
            SetText(worksheet, "M2", cabecalho.est);
            SetNumber(worksheet, "I3", cabecalho.distancia);
            SetNumber(worksheet, "L3", cabecalho.t_os_mont);
            SetNumber(worksheet, "O3", cabecalho.os);
            SetText(worksheet, "C4", cabecalho.atendente);
            SetText(worksheet, "C5", cabecalho.solicitante);
            SetText(worksheet, "J5", cabecalho.data_solicitacao?.ToString("dd/MM/yyyy HH:mm:ss"));
            SetText(worksheet, "C7", cabecalho.noite_montagem);
        }

        private static void PreencherItens(ExcelWorksheet worksheet, IReadOnlyList<KitChkGeralModel> itens)
        {
            for (var index = 0; index < itens.Count; index++)
            {
                var item = itens[index];
                var row = index + 10;

                SetNumber(worksheet, $"A{row}", item.qtd);
                SetNumber(worksheet, $"B{row}", item.codcompladicional);
                SetText(worksheet, $"C{row}", item.planilha);
                SetText(worksheet, $"E{row}", item.descricao_completa);
                SetText(worksheet, $"L{row}", item.unidade);
                SetText(worksheet, $"M{row}", item.obs);
                SetNumber(worksheet, $"O{row}", item.coddetalhescompl);
                SetText(worksheet, $"P{row}", string.Empty);
                SetNumber(worksheet, $"Q{row}", item.custo);
                SetNumber(worksheet, $"R{row}", item.peso);
                SetText(worksheet, $"S{row}", item.inserido_por);
            }
        }

        private static void PreencherEtiquetas(ExcelWorksheet worksheet, IReadOnlyList<KitChkGeralModel> itens)
        {
            worksheet.Cells.Clear();

            for (var column = 1; column <= 10; column++)
                worksheet.Column(column).Width = 12;

            var row1 = 1;
            var row2 = 2;

            foreach (var item in itens)
            {
                MergeText(worksheet, $"A{row1}:J{row2}", item.nome, 24, true, 26.25);
                row1 += 2;
                row2 += 2;

                MergeText(worksheet, $"A{row1}:G{row2}", item.shopping, 24, true, 26.25);
                MergeText(worksheet, $"H{row1}:J{row1}", "CONTROLE", 24, true, 26.25);
                MergeText(worksheet, $"H{row2}:J{row2}", item.kp?.ToString(), 24, true, 26.25);
                row1 += 2;
                row2 += 1;

                MergeText(worksheet, $"A{row2}:E{row2}", item.cidade, 20, false, 26.25);
                MergeText(worksheet, $"F{row2}:J{row2}", item.est, 20, false, 26.25);
                row1 += 1;
                row2 += 1;

                MergeText(worksheet, $"A{row2}:E{row2}", "CODDETCOMPL", 22, true, 26.25);
                MergeText(worksheet, $"F{row2}:J{row2}", "PLANILHA", 22, true, 26.25);
                row1 += 1;
                row2 += 1;

                MergeText(worksheet, $"A{row2}:E{row2}", item.coddetalhescompl?.ToString(), 20, false, 26.25);
                MergeText(worksheet, $"F{row2}:J{row2}", item.planilha, 20, false, 26.25);
                row1 += 1;
                row2 += 1;

                MergeText(worksheet, $"A{row2}:G{row2}", "DESCRIÇÃO", 22, true, 26.25);
                MergeText(worksheet, $"H{row2}:J{row2}", "QTD", 22, true, 26.25);
                row1 += 1;
                row2 += 3;

                MergeText(worksheet, $"A{row1}:G{row2}", item.descricao_completa, 20, false, 26.25);
                MergeText(worksheet, $"H{row1}:J{row2}", item.qtd.ToString(), 40, false, 26.25);
                row1 += 3;
                row2 += 1;

                MergeText(worksheet, $"A{row1}:J{row2}", "OBSERVAÇÃO", 20, true, 26.25);
                row1 += 1;
                row2 += 1;

                MergeText(worksheet, $"A{row1}:J{row2}", item.obs, 20, false, 57);
                row1 += 1;
                row2 += 1;

                MergeText(worksheet, $"A{row2}:G{row2}", "SOLICITANTE", 22, true, 26.25);
                MergeText(worksheet, $"H{row2}:J{row2}", "ATENDENTE", 22, true, 26.25);
                row1 += 1;
                row2 += 1;

                MergeText(worksheet, $"A{row2}:G{row2}", item.solicitante, 22, false, 26.25);
                MergeText(worksheet, $"H{row2}:J{row2}", item.atendente, 22, false, 26.25);
                row1 += 1;
                row2 += 2;

                row1 += 1;
                row2 += 1;
            }

            worksheet.PrinterSettings.LeftMargin = Producao.Utils.PrintPageSetupHelper.LeftMargin;
            worksheet.PrinterSettings.RightMargin = Producao.Utils.PrintPageSetupHelper.RightMargin;
            worksheet.PrinterSettings.HorizontalCentered = true;
            worksheet.PrinterSettings.VerticalCentered = true;
            worksheet.PrinterSettings.Scale = 90;
        }

        private static void MergeText(ExcelWorksheet worksheet, string address, string? value, double fontSize, bool bold, double rowHeight)
        {
            var range = worksheet.Cells[address];
            range.Merge = true;
            range.Value = value ?? string.Empty;

            range.Style.Font.Size = (float)fontSize;
            range.Style.Font.Bold = bold;
            range.Style.WrapText = true;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            for (var row = range.Start.Row; row <= range.End.Row; row++)
                worksheet.Row(row).Height = rowHeight;
        }

        private static void SetText(ExcelWorksheet worksheet, string address, string? value)
        {
            worksheet.Cells[address].Value = value ?? string.Empty;
        }

        private static void SetNumber(ExcelWorksheet worksheet, string address, double? value)
        {
            worksheet.Cells[address].Value = value;
        }

        private static void SetNumber(ExcelWorksheet worksheet, string address, long? value)
        {
            worksheet.Cells[address].Value = value;
        }

        private static void Proteger(ExcelPackage package)
        {
            package.Workbook.Protection.LockStructure = true;
            package.Workbook.Protection.SetPassword(SenhaProtecao);

            foreach (var worksheet in package.Workbook.Worksheets)
            {
                worksheet.Protection.IsProtected = true;
                worksheet.Protection.SetPassword(SenhaProtecao);
            }
        }
    }
}
