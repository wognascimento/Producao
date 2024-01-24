using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_controlado_shopping_retorno", Schema = "producao")]
    public class ControladoShoppingRetornoModel
    {
        [Key, Column(Order = 1)]
        public string barcode { get; set; }
        public string inserido_por { get; set; }
        [Key, Column(Order = 2)]
        public DateTime inserido_em { get; set; }
    }
}
