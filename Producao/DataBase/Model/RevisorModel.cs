using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_revisores", Schema = "producao")]
    public class RevisorModel
    {
        [Key]
        public string? revisores { get; set; }
    }
}
