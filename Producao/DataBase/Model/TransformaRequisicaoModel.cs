using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_transforma_requisicao", Schema = "producao")]
    public class TransformaRequisicaoModel
    {
        public long? codcompladicional { get; set;}
        public long? quantidade { get; set;}
        public long? num_requisicao { get; set;}
        public string? planilha { get; set;}
        public string? descricao { get; set;}
        public string? descricao_adicional { get; set;}
        public string? complementoadicional { get; set; }
        public string? descricao_completa { get; set; }
    }
}
