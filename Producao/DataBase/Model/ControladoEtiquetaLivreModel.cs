using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("qry_codigos_livres", Schema = "producao")]
    public class ControladoEtiquetaLivreModel
    {
        public string? barcode { get; set; }
        public long? codigo { get; set; }
    }
}
