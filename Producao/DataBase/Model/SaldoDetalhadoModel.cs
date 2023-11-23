using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_saldo_detalhado_c", Schema = "producao")]
    public class SaldoDetalhadoModel
    {
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public long? codcompladicional { get; set; }
        public double? chks { get; set; }
        public double? saldo_patrimonial { get; set; }
        public string? unidade { get; set; }
        public double? saldo_patrimonial_ano_anterior { get; set; }
        public double? saldo_disponivel_ano_anterior { get; set; }
        public double? estoque_inicial_nao_processado { get; set; }
        public double? estoque_inicial_processado { get; set; }
        public double? cce { get; set; }
        public double? oss_peca_nova { get; set; }
        public double? oss_recuperacao { get; set; }
        public double? movimentacao_entrada_processada { get; set; }
        public double? movimentacao_de_entrada_nao_processada { get; set; }
        public double? total_entradas { get; set; }
        public double? requisicao_geral { get; set; }
        public double? movimentacao_saída { get; set; }
        public double? requisicoes_internas { get; set; }
        public double? movimentacao_de_saidas_gerais { get; set; }
        public double? movimentacao_de_saidas_processadas { get; set; }
        public double? descartes_gerais { get; set; }
        public double? total_de_saidas { get; set; }
        public double? total_produzido { get; set; }
        public double? saldo_de_estoque_produzido { get; set; }
        public double? saldo_disponível { get; set; }
        public string? inventariado { get; set; }
        public string? inativo { get; set; }
    }
}
