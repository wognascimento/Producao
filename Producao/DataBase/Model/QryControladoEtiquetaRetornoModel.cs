using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_controlado_etiqueta_retorno", Schema = "producao")]
    public class QryControladoEtiquetaRetornoModel
    {
        public long codigo { get; set; }
        public string planilha { get; set;}
        public string descricao_completa { get; set;}
    }
}
