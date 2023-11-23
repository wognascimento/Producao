using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_etiquetas_form", Schema = "producao")]
    public class ControladoEtiquetaModel
    {
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public string? unidade { get; set; }
        public string? inativo { get; set; }
        public int? etiquetas { get; set; }
        public int? impressas { get; set; }
        public int? total_etiquetas { get; set; }
        public string? desc_completa { get; set; }
        public double? saldo_estoque { get; set; }
    }
}
