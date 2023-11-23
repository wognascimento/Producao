using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_obs_os", Schema = "producao")]
    public class ObsOsModel
    {
        [Key]
        public long? cod_obs { get; set; }
        public long? num_os_produto { get; set; }
        public long? cod_compl_adicional { get; set; }
        public long? num_caminho { get; set; }
        public long? codigo_setor { get; set; }
        public string? setor_caminho { get; set; }
        public string? orientacao_caminho { get; set; }
        public string? distribuir_os { get; set; }
        public string? cliente { get; set; }
        public string? solicitado_por { get; set; }
        public DateTime? solicitado_data { get; set; }
        public string? emitida { get; set; }
        public long? produtos_servico_num_os_servico {  get; set; }
        public bool? cancelar { get; set; }
        public string? cancelado_por { get; set; }
        public DateTime? cancelado_em { get; set; }
    }
}
