using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("t_conta_process_semana", Schema = "producao")]

    public class ContaProcessSemanaModel
    {
        [Key]
        public long? cod_movimento { get; set; }
        public long? cod_compladicional { get; set; }
        public string? barcode { get; set; }
        public double? quantidade { get; set; }
        public int? semana { get; set; }
        public string? digitado_por { get; set; }
        public DateTime? digitado_data { get; set; }
        public string? galpao { get; set; }
    }
}
