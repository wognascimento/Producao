using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_controle_baia_enderecamento", Schema = "expedicao")]
    public class ControleBaiaEnderecamentoModel
    {
        [Key]
        public long? id_controle { get; set; }
        public string? sigla_serv { get; set; }
        public string? baia_caminhao { get; set; }
        public string? endereco { get; set; }
        public string? item_memorial { get; set; }
        public long? id_aprovado { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }
        public string? alterado_por { get; set; }
        public DateTime? alterado_em { get; set; }
    }
}
