using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_baixa_os_producao", Schema = "ht")]
    public class BaixaOsProducaoModel
    {
        public long? num_os_produto { get; set; }
        public long? num_os_servico { get; set; }
        public DateTime? recebido_setor_data { get; set; }
        public DateTime? concluida_os_data { get; set; }
        public string? cancelada_os { get; set; }
        //public string? alterado_por { get; set; }
        //public DateTime? alterado_data { get; set; }
        //public string? aprovado { get; set; }
        //public string? aprovado_por { get; set; }
        //public DateTime? aprovado_em { get; set; }
        public string? retrabalho { get; set; }
        public string? situacao { get; set; }
    }
}
