using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_rpt_inflamabilidade", Schema = "projetos")]
    public class InflamabilidadeModel
    {
        [Key]
        public string? sigla { get; set; }
        public string? rrt { get; set; }
        public string? responsavel { get; set; }
        public DateTime? data_conclusao { get; set; }
        public string? concluido_por { get; set; }
    }
}
