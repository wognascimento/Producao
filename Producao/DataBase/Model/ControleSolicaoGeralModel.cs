using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("query_controle_solicao_geral", Schema = "kitsolucao")]
    public class ControleSolicaoGeralModel
    {
        public DateTime? data_solicitacao { get; set; }
        public string? shopping { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public double? qtd { get; set; }
        public double? kp { get; set; }
        public DateTime? data_envio { get; set; }
        public string? class_solucao { get; set; }
        public string? motivos { get; set; }
        public string? local_galpao { get; set; }
        public string? status { get; set; }
        public string? placa { get; set; }
        public string? motorista { get; set; }
        public TimeSpan? horario_saida { get; set; }
        public long? ordem { get; set; }
        public string? obs { get; set; }
        public string? noite_montagem { get; set; }
        public long? coddetalhescompl { get; set; }
    }
}
