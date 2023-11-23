using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_requisicao_receita", Schema = "producao")]
    public class RequisicaoReceitaModel
    {
        [Key]
        public long? id { get; set; }
        public long? codcompladicional_produto { get; set; }
        public long? codcompladicional_receita { get; set; }
        public double? quantidade { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }
        public string? alterado_por { get; set; }
        public DateTime? alterado_em {  get; set; }
    }
}
