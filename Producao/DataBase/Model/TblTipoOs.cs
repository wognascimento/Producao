using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_tipo_os", Schema = "producao")]
    public class TblTipoOs
    {
        [Key]
        public long? codigo_tipo { get; set; }
        public string? tipo_servico { get; set; }
        public string? descricao_servico { get; set; }
        public string? classificacao { get; set; }
    }
}
