using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_planilhas_construcao", Schema = "projetos")]
    public class PlanilhaConstrucaoModel
    {
        [Key]
        public string planilha { get; set; }
    }
}
