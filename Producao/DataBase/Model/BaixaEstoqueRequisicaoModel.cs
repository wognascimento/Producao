using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_baixa_estoque_requisicao", Schema = "producao")]
    public class BaixaEstoqueRequisicaoModel
    {
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public long? num_requisicao { get; set; }
        public double? qtd_req { get; set; }
        public double? qtd_baixa { get; set; }
        public string? destino { get; set; }
        public DateTime? saida_data { get; set; }
        public string? saida_por { get; set; }
    }
}
