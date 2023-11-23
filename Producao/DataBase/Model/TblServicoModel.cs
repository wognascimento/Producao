using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_servicos", Schema = "producao")]
    public class TblServicoModel
    {
        [Key]
        public long? num_os { get; set; }
        public string? tipo { get; set; }
        public long? codigo_setor { get; set; }
        public string? descricao_setor { get; set; }
        public string? descricao_servico { get; set; }
        public string? orientacao { get; set; }
        public double? quantidade { get; set; }
        public DateTime? data_emissao { get; set; }
        public string? emitido_por { get; set; }
        public string? planilha { get; set; }
        public string? cliente { get; set; }
        public DateTime? data_conclusao { get; set; }
        public string? cancelar { get; set; }
        public long? codigo_servico { get; set; }
        public DateTime? emitido_por_data { get; set; }
        public DateTime? data_cancelamento { get; set; }
        public DateTime? data_conclusao_efetiva { get; set; }
        public string? cancelado_por { get; set; }
        public string? sigla { get; set; }
    }
}
