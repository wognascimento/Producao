using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("query_kitsolicao_geral", Schema = "kitsolucao")]
    public class KitSolicaoGeralModel
    {
        public long? coddetalhescompl { get; set; }
        public string? shopping { get; set; }
        public DateTime? data_solicitacao { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public string? solicitante { get; set; }
        public double? qtd { get; set; }
        public string? unidade { get; set; }
        public string? local_shoppings { get; set; }
        public string? obs { get; set; }
        public double? kp { get; set; }
        public string? sigla { get; set; }
        public string? orient_montagem { get; set; }
        public double? custo_unitatio { get; set; }
        public long? codigo { get; set; }
        public DateTime? data_saida { get; set; }
        public TimeSpan? hora_saida { get; set; }
        public string? placa_veiculo { get; set; }
        public long? codigo_veiculo { get; set; }
        public string? nome_motorista { get; set; }
        public long? codigo_motorista { get; set; }
        public string? solicitado_por { get; set; }
        public string? autorizado_por { get; set; }
        public string? destino_1 { get; set; }
        public string? destino_2 { get; set; }
        public string? observacao { get; set; }
        public long? km_inicial { get; set; }
        public string? atendido { get; set; }
        public long? km_final { get; set; }
        public string? cancela_solicitacao { get; set; }
        public string? motivo_cancelamento { get; set; }
        public string? alterado_por { get; set; }
        public string? cancelado_por { get; set; }
        public DateTime? alterado_em { get; set; }
        public DateTime? cancelado_em { get; set; }
        public string? portaria { get; set; }
        public string? tipo_saida { get; set; }
        public string? nome_solicitante { get; set; }
        public string? departamento { get; set; }
        public TimeSpan? previsao_retorno { get; set; }
        public string? galpao_saida { get; set; }
        public string? portaria_entrada { get; set; }
        public DateTime? data_saida_portaria { get; set; }
        public TimeSpan? hora_saida_portaria { get; set; }
        public DateTime? data_entrada_portaria { get; set; }
        public TimeSpan? hora_entrada_portaria { get; set; }
        public string? origem_solicitacao { get; set; }
        public double? volume_carga { get; set; }
        public string? class_solucao { get; set; }
        public string? motivos { get; set; }
    }
}
