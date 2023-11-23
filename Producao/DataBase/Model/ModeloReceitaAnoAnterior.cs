using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_receita_ano_anterior", Schema = "modelos")]
    public class ModeloReceitaAnoAnterior
    {
        [Key]
        public long? codigo_controle {get; set; }
        public long? id_modelo {get; set; }
        public string? obs_modelo {get; set; }
        public long? codcompladicional {get; set; }
        public string? planilha_ano_anterior {get; set; }
        public string? descricao_ano_anterior {get; set; }
        public string? desc_adicional_ano_anterior {get; set; }
        public string? complemento_adicional_ano_anterior {get; set; }
        public long? id_linha {get; set; }
        public long? itens_receita {get; set; }
        public string? planilha {get; set; }
        public string? descricao {get; set; }
        public string? desc_adicional {get; set; }
        public string? complemento_adicional {get; set; }
        public double? qtde_modelo {get; set; }
        public double? qtde_producao {get; set; }
        public int? ano {get; set; }
        public string? tema { get; set; }
        public string? descricao_completa_ano_anterior { get; set; }
        public string? descricao_completa_item_ano_anterior { get; set; }
    }
}
