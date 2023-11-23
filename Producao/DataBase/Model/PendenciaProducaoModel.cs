using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_pendencia_producao", Schema = "producao")]
    public class PendenciaProducaoModel
    {
        public DateTime? fechamento_shopp { get; set; }
        public string? sigla { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public string? unidade { get; set; }
        public double? quantidade_chk { get; set; }
        public double? qtd_completada { get; set; }
        public long? coddetalhescompl { get; set; }
        public long? codcompl { get; set; }
        public string? confirmados { get; set; }
        public string? orient_producao { get; set; }
        public string? orient_montagem { get; set; }
        public double? det { get; set; }
        public double? qtd_exped { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public string? resp_prod { get; set; }
        public int? nivel { get; set; }
        public string? id { get; set; }
        public string? item_memorial { get; set; }
        public string? local_shoppings { get; set; }
        public string? revisado { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string? alterado_por { get; set; }
        public string? tema { get; set; }
        public double? distancia { get; set; }
        public string? caminhao { get; set; }
    }
}
