using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_fecha_links", Schema = "comercial")]
    public class FechaLinkModel
    {
        [Key]
        public long? codlinkfecha { get; set; }
        public string? sigla { get; set; }
        public string? tema { get; set; }
        public string? links { get; set; }
        public DateTime? data_link { get; set; }
        public long? idtema { get; set; }
    }
}
