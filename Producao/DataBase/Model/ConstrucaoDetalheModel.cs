using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_construcao_detalhes", Schema = "projetos")]
    public class ConstrucaoDetalheModel
    {
        [Key]
        public long? id_contrucao_detalhes { get; set; }
        public long? codcompladicional { get; set; }
        public string? nome_fantasia { get; set; }
        public int? item { get; set; }
        public string? descricao_peca { get; set; }
        public int? volume { get; set; }
    }
}
