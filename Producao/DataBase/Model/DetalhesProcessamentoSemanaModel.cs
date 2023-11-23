using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_detalhes_processamento_semana", Schema = "producao")]
    public class DetalhesProcessamentoSemanaModel
    {
        public string? planilha { get; set; }
        //public string? descricao { get; set; }
        //public string? descricao_adicional { get; set; }
        //public string? complementoadicional { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public double? quantidade { get; set; }
        public int? semana { get; set; }
        //public string? galpao { get; set; }
        public long? codcompladicional { get; set; }
    }
}
