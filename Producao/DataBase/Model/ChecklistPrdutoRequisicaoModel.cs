using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_checklist_prduto_requisicao", Schema = "producao")]
    public class ChecklistPrdutoRequisicaoModel
    {
        public string? sigla { get; set; }
        public long? coddetalhescompl { get; set; }
        public string? local_shoppings { get; set; }
        public string? descricao_completa { get; set; }
        public long? num_requisicao { get; set; }
    }
}
