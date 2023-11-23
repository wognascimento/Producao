using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("unidades", Schema = "producao")]
    public class UnidadeModel
    {
        [Key]
        public string unidade { get; set; }
    }
}
