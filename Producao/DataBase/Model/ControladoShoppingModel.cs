using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_controlado_shopping", Schema = "producao")]
    public class ControladoShoppingModel
    {
        [Key, Column(Order = 1)]
        public long? num_requisicao { get; set; }
        [Key, Column(Order = 2)]
        public string? barcode { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }
        public string? retorno { get; set; }
    }
}
