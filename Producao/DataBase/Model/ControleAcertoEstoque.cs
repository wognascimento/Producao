using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_controle_acerto_estoque", Schema = "producao")]
    public class ControleAcertoEstoque
    {
        [Key]
        public long? codigo { get; set; }
        public long? cod_movimentacao { get; set; }
        public string? processado { get; set; }
        public long? codcompladicional { get; set; }
        public double? quantidade { get; set; }
        public DateTime? data { get; set; }
        public TimeSpan? hora { get; set; }
        public string? operacao { get; set; }
        public string? processo { get; set; }
        public string? local { get; set; }
        public string? incluido_por { get; set; }
        public DateTime? incluido_data { get; set; }
        public string? bloqueado { get; set; }
        public string? liberado_por { get; set; }
        public DateTime? liberado_em { get; set; }
    }
}
