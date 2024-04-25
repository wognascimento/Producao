using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_etiqueta_zebra", Schema = "producao")]
    public class ControladoZebraModel
    {
        [Key]
        public long? codigo { get; set; }
        public long? codcompladicional { get; set; }
        public int? etiqueta { get; set; } = 1;
    }
}
