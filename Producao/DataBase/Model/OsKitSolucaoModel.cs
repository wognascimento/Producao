using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("t_os_kitsolucao", Schema = "kitsolucao")]
    public class OsKitSolucaoModel
    {
        public long? t_os_mont { get; set; }
        public string? shopping { get; set; }
        public DateTime? data_emissao { get; set; }
        [Key]
        public long? os { get; set; }
        public DateTime? data_solicitacao { get; set; }
        public DateTime? hora_solicitacao { get; set; }
        public string? solicitante { get; set; }
        public DateTime? concluir_ate { get; set; }
        public DateTime? hora_concluir { get; set; }
        public string? forma_de_envio { get; set; }
        public string? responsavel { get; set; }
        public string? obs_de_envio { get; set; }
        public double? valor_estimado { get; set; }
        public string? noite_montagem { get; set; }
        public double? volumes { get; set; }
        public string? atendente { get; set; }
        public long? cod_solicita_transporte { get; set; }
        public string? tipo_manutencao { get; set; }
        public string? status { get; set; }
        public string? status_por { get; set; }
        public DateTime? status_data { get; set; }
        public long? id_manutencao { get; set; }
    }
}
