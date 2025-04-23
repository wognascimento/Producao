using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("qry_impressao", Schema = "producao")]
    public class QryImpressaoModel
    {
        public long? codcompladicional { get; set; }
        public string? planilha  { get; set; }
        public string? descricao_completa  { get; set; }
        public long? codigo  { get; set; }
        public string? barcode  { get; set; }
        public string? impresso  { get; set; }

    }
}
