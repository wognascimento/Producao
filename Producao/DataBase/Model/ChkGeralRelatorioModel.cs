using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qrychkgeral_relatorio", Schema = "producao")]
    public class ChkGeralRelatorioModel
    {
        public string? sigla { get; set; }
        public string? nome { get; set; }
        public string? tema { get; set; }
        public string? ordem { get; set; }
        public string? item_memorial { get; set; }
        public string? local_shoppings { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? unidade { get; set; }
        public double? qtd { get; set; }
        public string? orient_montagem { get; set; }
        public long? coddetalhescompl { get; set; }
        public long? codcompladicional { get; set; }
        public long? bloco_revisao { get; set; }
        public string? resp_revisao { get; set; }
        public string? ok_revisao_alterada { get; set; }
        public string? bloco { get; set; }
        public string? sigla_sem_acento { get; set; }
        public long? id { get; set; }
        public long? coduniadicional { get; set; }
        public string? descricao_dd { get; set; }
        public long? id_aprovado { get; set; }
        public DateTime? data_aprovado { get; set; }
    }
}
