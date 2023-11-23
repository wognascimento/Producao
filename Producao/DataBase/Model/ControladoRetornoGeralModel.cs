using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_controlados_retorno_geral", Schema = "expedicao")]
    public class ControladoRetornoGeralModel
    {
        public string? tipo_evento { get; set; }
        public string? nome { get; set; }
        public long? id_aprovado { get; set; }
        public string? sigla { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? unidade { get; set; }
        public double? expedido { get; set; }
        public double? solucao_manutencao { get; set; }
        public double? qrcode { get; set; }
        public double? recebida { get; set; }
        public double? devolvida { get; set; }
        public double? retorno { get; set; }
        public double? cobranca { get; set; }
        public double? custo { get; set; }
        public double? custo_total { get; set; }
        public string? cancelar_cobraca { get; set; }
        public string? justificativa { get; set; }
        public string? atualizado_por { get; set; }
        public DateTime? atualizado_em { get; set; }
    }
}
