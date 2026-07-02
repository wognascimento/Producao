using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("qry_codigo_impresso", Schema = "producao")]
    public class ControladoEtiquetaImpressaModel
    {
        public long? codigo { get; set; }
        public string? impresso { get; set; }
        public int? impressa { get; set; }
        public long? codcompladicional { get; set; }
        public bool? vinculado { get; set; }
    }
}
