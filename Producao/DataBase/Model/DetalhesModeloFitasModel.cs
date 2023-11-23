using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qrydetalhesmodelo_fitas", Schema = "modelos")]
    public class DetalhesModeloFitasModel
    {
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public double? qtd_modelo { get; set; }
        public double? qtd_producao { get; set; }
        public long? id_modelo { get; set; }
        public double? qtd { get; set; }
        public long? codcompladicional { get; set; }
        public string? observacao { get; set; }
    }
}
