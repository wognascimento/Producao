using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_rpt_inflamabilidade_responsavel", Schema = "projetos")]
    public class InflamabilidadeResponsavelModel
    {
        [Key]
        public string nome { get; set; }
        public string rg { get; set; }
        public string cau_sp { get; set; }
    }
}
