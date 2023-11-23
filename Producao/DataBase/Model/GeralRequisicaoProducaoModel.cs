using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_geral_requisicao_producao", Schema = "producao")]
    public class GeralRequisicaoProducaoModel
    {
        public long? num_os_servico { get; set; }
        public long? num_requisicao { get; set; }
        public string? cliente { get; set; }
        public DateTime? data { get; set; }
        public string? alterado_por { get; set; }
        public long? id_modelo { get; set; }
        public string? planilha { get; set; }
        //public string? descricao { get; set; }
        //public string? descricao_adicional { get; set; }
        //public string? complementoadicional { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public int? vida_util { get; set; }
        public double? quantidade { get; set; }
        public string? ok { get; set; }
        public DateTime? data_ok { get; set; }
        public string? ok_expedido { get; set; }
        public string? observacao { get; set; }
        public string? voltagem { get; set; }
        public string? local_shop { get; set; }
        public long? complemento_chk { get; set; }
        public double? custo { get; set; }
        public double? c_total { get; set; }
    }
}
