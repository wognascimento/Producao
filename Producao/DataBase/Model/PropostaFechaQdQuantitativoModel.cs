using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_fecha_qd_quantitativo", Schema = "comercial")]
    public class PropostaFechaQdQuantitativoModel
    {
        [Key]
        public long? cod_linha_qdfecha { get; set; }
        public string? sigla { get; set; }
        public long? coddimensao { get; set; }
        public string? local { get; set; }
        public string? detalhe_local { get; set; }
    }
}
