using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_rpt_inflamabilidade_detalhes", Schema = "projetos")]
    public class InflamabilidadeDetalheModel
    {
        [Key, Column(Order = 0)]
        public string? sigla { get; set; }
        [Key, Column(Order = 1)]
        public string? tipo { get; set; }
        public string? classificacao { get; set; }
    }
}
