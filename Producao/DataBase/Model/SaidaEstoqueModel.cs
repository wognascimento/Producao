using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("t_saida", Schema = "producao")]
    public class SaidaEstoqueModel
    {
        [Key]
        public long? codigo_saida { get; set; }
        public double? quantidade { get; set; }
        public string? destino { get; set; }
        public DateTime? saida_data { get; set; }
        public string? saida_por { get; set; }
        public string? observacao { get; set; }
        public long? codcompladicional { get; set; }
        public string? local_galpao { get; set; }
        public long? num_requisicao { get; set; }
        public string? caminho { get; set; }
        public string? endereco { get; set; }
        public double? quantidade_fisica { get; set; }
        public string? processado { get; set; }
    }
}
