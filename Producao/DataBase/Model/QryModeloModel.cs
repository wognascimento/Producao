using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qrymodelos", Schema = "modelos")]
    public class QryModeloModel
    {
        public long? id_modelo {get; set; }
        public string? tema {get; set; }
        public string? planilha {get; set; }
        public string? descricao {get; set; }
        public string? descricao_adicional {get; set; }
        public string? complementoadicional {get; set; }
        public string? descricao_completa {get; set; }
        public string? unidade {get; set; }
        public long? codcompladicional {get; set; }
        public string? cadastrado_por {get; set; }
        public DateTime? data_cadastro {get; set; }
        public double? multiplica { get;set; }
        public string? obs_modelo { get; set;}
        public int? qtd_fiada_cascata { get; set; }
    }
}
