using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_os_emitidas", Schema = "producao")]
    public class OrdemServicoEmitidaModel
    {
        public long? num_os_produto { get; set; }
        public long? num_os_servico { get; set; }
        public string? emitida_por { get; set; }
        public string? solicitante { get; set; }
        public string? setor_caminho { get; set; }
        public string? tipo { get; set; }
        public string? cliente { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public DateTime? emitida_data { get; set; }
        public double? quantidade { get; set; }
        public DateTime? meta_data { get; set; }
        public double? ht { get; set; }
        public string? cancelada_os { get; set; }
        public DateTime? concluida_os_data { get; set; }
        public string? alterado_por { get; set; }
        public DateTime? alterado_data { get; set; }
        public string? orientacao_caminho { get; set; }
        public double? ht_peca { get; set; }
        public double? m3 { get; set; }
        public double? ref_cub_total { get; set; }
        public int? caminho { get; set; }
    }
}
