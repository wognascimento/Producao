using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_controle_envio", Schema = "kitsolucao")]
    public class ControleEnvioModel
    {
        [Key]
        public long? coddetalhescompl { get; set; }
        public DateTime? data_envio { get; set; }
        public string? local_galpao { get; set; }
        public string? status { get; set; }
        public string? placa { get; set; }
        public string? motorista { get; set; }
        public TimeSpan? horario_saida { get; set; }
        public long? ordem { get; set; }
    }
}
