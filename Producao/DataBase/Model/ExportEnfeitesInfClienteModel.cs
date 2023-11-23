using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao
{
    [Keyless]
    [Table("qry_exportEnfeites_inf_cliente", Schema = "modelos")]
    public class ExportEnfeitesInfClienteModel
    {
        public string? sigla_serv { get; set; }
        public string? tema { get; set; }
        public int? nivel { get; set; }
        public long? id_modelo { get; set; }
        public int? qtd_fiada_cascata { get; set; }
        public string? local_shoppings { get; set; }
        public long? coddetalhescompl { get; set; }
        public double? qtd { get; set; }
        public string? descricao_adicional { get; set; }
        public long? num_os_servico { get; set; }
    }
}
