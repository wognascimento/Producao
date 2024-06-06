using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_materiais_predominantes_decoracao", Schema = "projetos")]
    public class MaterialPredominanteDecoracaoModel
    {
        [Key]
        public string? tipo { get; set; }
        public string? descritivo { get; set; }
    }
}
