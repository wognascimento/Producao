using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("view_historico_setor", Schema = "modelos")]
    public class HistoricoSetorModel
    {
        public long? codcompladicional { get; set; }
        public long? codigo_setor { get; set; }
        public string? setor { get; set; }
        public bool? selesao { get; set; } = true;
        public string? observacao {  get; set; }
    }
}
