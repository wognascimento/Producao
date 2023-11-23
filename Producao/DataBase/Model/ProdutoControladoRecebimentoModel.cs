using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("view_produtos_controlas_recebimento", Schema = "expedicao")]
    public class ProdutoControladoRecebimentoModel
    {
        public string? nome { get; set; }
        public long? id_aprovado { get; set; }
        public string? sigla { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? unidade { get; set; }
        public double? expedido { get; set; }
        public double? solucao_manutencao { get; set; }
        public double? recebida { get; set; }
        public double? devolvida { get; set; }
        public double? retorno { get; set; }
        public double? cobranca { get; set; }
        public double? custo { get; set; }
        public double? custo_total { get; set; }
        public string? atualizado_por { get; set; }
        public DateTime? atualizado_em { get; set; }
        public string? cancelar_cobraca { get; set; }
        public string? justificativa { get; set; }
        public string? entrada_estoque { get; set; }
        public string? entrada_estoque_por { get; set; }
        public DateTime? entrada_estoque_em { get; set; }
        public double? quantidade { get; set; }
        public double? cobranca_eventos { get; set; }
        public double? custo_total_eventos { get; set; }
    }
}
