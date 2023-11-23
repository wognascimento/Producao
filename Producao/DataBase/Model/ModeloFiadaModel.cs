using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_modelo_fiada", Schema = "modelos")]
    public class ModeloFiadaModel
    {
        [Key]
        public long? id { get; set; }
        public long? id_modelo { get; set; }
        public string? modelofiada { get; set; }
        public int? qtdmodelofiada {  get; set; }
    }
}
