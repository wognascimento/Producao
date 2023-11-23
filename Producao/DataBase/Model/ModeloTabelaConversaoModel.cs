using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_conversao", Schema = "modelos")]
    public class ModeloTabelaConversaoModel
    {
        [Key]
        public long? codcompladicional { get; set; }
        public double? multiplica { get; set; }
        public double? soma {  get; set; }
    }
}
