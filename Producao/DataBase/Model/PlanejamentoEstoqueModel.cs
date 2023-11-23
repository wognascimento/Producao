using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("pcp_planejamento_estoque", Schema = "producao")]
    public class PlanejamentoEstoqueModel
    {
        public string? alterado_por { get; set; }
        public DateTime? data { get; set; }
        public long? num_requisicao { get; set; }
        public long? num_os_servico { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public double? quantidade { get; set; }
        public double? qtd_baixa { get; set; }
        public string? descricao_setor { get; set; }
        public string? cliente_os { get; set; }
    }
}
