using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("view_produto_shopping", Schema = "producao")]
    public class ProdutoShoppingModel
    {

        [Key]
        public long? codcompladicional { get; set; }
        public DateTime? primeirodedata_de_expedicao { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public string? descricaofiscal { get; set; }
        public string? unidade { get; set; }
        public string? ncm { get; set; }
        public double? peso { get; set; }
        public double? custo { get; set; }
        public string? exportado_folhamatic { get; set; }
        public string? alx { get; set; }
    }
}
