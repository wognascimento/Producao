using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_modelos", Schema = "modelos")]
    public class ModeloModel
    {
        [Key]
        public long? id_modelo { get; set; }
        public string? foto { get; set; }
        public string? tema { get; set; }
        public string? obs_modelo { get; set; }
        public string? aprovado { get; set; }
        public string? aprovado_por { get; set; }
        public DateTime? data_aprovacao { get; set; }
        public string? alterado { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string? liberado { get; set; }
        public string? liberado_por { get; set; }
        public DateTime? data_liberacao { get; set; }
        public long? codcompladicional { get; set; }
        public string? cadastrado_por { get; set; }
        public DateTime? data_cadastro { get; set; }
        public int? qtd_fiada_cascata {  get; set; }
    }
}
