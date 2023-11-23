using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_classificacao_solucao", Schema = "producao")]
    public class ClassificacaoSolucaoModel
    {
        [Key]
        public int? id { get; set; }
        public string? classificacao { get; set; }
        public string? motivo { get; set; }
    }
}
