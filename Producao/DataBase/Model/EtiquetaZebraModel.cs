using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_etiqueta_zebra", Schema = "producao")]
    public class EtiquetaZebraModel
    {
        [Key]
        public long? codigo { get; set; }
        public long? codcompladicional { get; set; }
        public long? etiqueta { get; set; }
    }
}
