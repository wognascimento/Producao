using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("t_entrada_estoque", Schema = "producao")]
    public class EntradaEstoqueModel
    {
        [Key]
        public long? codigo_entrada { get; set; }
        public double? quantidade { get; set; }
        public string? procedencia { get; set; }
        public DateTime? entrada_data { get; set; }
        public string? entrada_por { get; set; }
        public long? codcompladicional { get; set; }
        public string? local_galpao { get; set; }
        public string? endereco { get; set; }
        public double? quantidade_fisica { get; set; }
        public string? processado { get; set; }
    }
}
