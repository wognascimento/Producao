using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_receita_detalhes_criado", Schema = "modelos")]
    public class QryReceitaDetalheCriadoModel
    {
        public long? codcompladicional_produto {get; set; }
        public long? id_linha {get; set; }
        public string? planilha {get; set; }
        public string? descricao {get; set; }
        public string? descricao_adicional {get; set; }
        public string? complementoadicional {get; set; }
        public string? unidade {get; set; }
        public long? id_modelo {get; set; }
        public long? codcompladicional {get; set; }
        public double? qtd_modelo {get; set; }
        public double? qtd_producao {get; set; }
        public string? cadastrado_por {get; set; }
        public DateTime? data_cadastro {get; set; }
        public string? alterado_por {get; set; }
        public DateTime? data_alterado {get; set; }
        public bool? cadastrado {get; set; }
        public int? mod1 {get; set; }
        public int? mod2 {get; set; }
        public int? mod3 {get; set; }
        public int? mod4 {get; set; }
        public int? mod5 {get; set; }
        public int? mod6 {get; set; }
        public int? mod7 {get; set; }
        public int? mod8 {get; set; }
        public int? mod9 {get; set; }
        public int? mod10 {get; set; }
        public double? custo {get; set; }
        public double? custo_total {get; set; }
        public double? saldoestoque {get; set; }
        public double? saldoproduzido {get; set; }
        public double? saldo_patrimonial {get; set; }
        public string? observacao {get; set; }
        public string? local {get; set; }
        public string? descricao_completa { get; set; }
        public string? inativo { get; set; }
    }
}
