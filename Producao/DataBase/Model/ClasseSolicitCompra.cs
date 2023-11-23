using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_classe_solicit_compra", Schema = "producao")]
    public class ClasseSolicitCompra
    {
        [Key]
        public string classe_solicit_compra { get; set; }
    }
}
