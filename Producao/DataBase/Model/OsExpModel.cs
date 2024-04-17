using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_os_exp", Schema = "expedicao")]
    public class OsExpModel
    {
        [Key]
        public long? n_os_desbaiamento { get; set; }
        public int? antigo { get; set; }
        public string? codvol { get; set; }
        public DateTime? data { get; set; }
        public string? resp { get; set; }
        public string? setor { get; set; }
        public long? quantidade { get; set; }
        public string? obs { get; set; }
        public long? coddetalhescompl { get; set; }
        public string? solicitante { get; set; }
        public string? local_shopp { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }
    }
}
