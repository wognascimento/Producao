using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_checklist_prduto_construcao", Schema = "projetos")]
    public class ChecklistPrdutoConstrucaoModel
    {
        public string? sigla { get; set; }
        public long? coddetalhescompl { get; set; }
        public string? local_shoppings { get; set; }
        public long? codcompladicional { get; set; }
    }
}
