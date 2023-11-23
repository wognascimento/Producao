using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qrybaseanaliseplan", Schema = "producao")]
    public class BaseAnalisePlanModel
    {
        public string? item_memorial { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public string? ok { get; set; }
        public string? confirmado { get; set; }
        public long? codcompl { get; set; }
        public long? coddetalhescompl { get; set; }
        public string? sigla { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public double? qtd_compl { get; set; }
        public string? descricao_adicional { get; set; }
        public string? local_shoppings { get; set; }
        public string? orient_montagem { get; set; }
        public string? obs { get; set; }
        public DateTime? fechamento_shopp { get; set; }
        public double? qtd_detalhe { get; set; }
        public string? complementoadicional { get; set; }
        public double? somadeqtd_expedida { get; set; }
        public string? local_producao { get; set; }
        public double? det { get; set; }
        public double? exp { get; set; }
        public DateTime? data_revisado_por { get; set; }
        public string? resp_prod { get; set; }
        public string? tema { get; set; }
        public DateTime? data_aprovado { get; set; }
        public string? resp_cenas { get; set; }
        public string? resp_trilha { get; set; }
    }
}
