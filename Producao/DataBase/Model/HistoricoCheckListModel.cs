using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("view_historico_checklist", Schema = "producao")]
    public class HistoricoCheckListModel
    {
        public string? sigla { get; set; } 
        public string? sigla_serv { get; set; } 
        public string? tema { get; set; } 
        public long? codcompladicional { get; set; } 
        public string? planilha { get; set; } 
        public string? descricao_completa { get; set; } 
        public string? unidade { get; set; } 
        public double? qtd_chk { get; set; } 
        public double? qtd_completada { get; set; } 
        public int? ano { get; set; } 
    }
}
