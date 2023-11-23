using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao
{
    [Table("tbl_etiqueta_producao", Schema = "producao")]
    public class EtiquetaProducaoModel
    {
        [Key]
        public long? codvol {get; set;}
        public long? coddetalhescompl {get; set;}
        public int? volumes {get; set;}
        public int? volumes_total { get; set; }
        public double? qtd {get; set;}
        public double? largura {get; set;}
        public double? altura {get; set;}
        public double? profundidade {get; set;}
        public double? peso_bruto {get; set;}
        public double? peso_liquido {get; set;}
        public bool? impresso {get; set;}
        public string? impresso_por {get; set;}
        public DateTime? impresso_em {get; set;}
        public string? criado_por {get; set;}
        public DateTime? criado_em {get; set;}
    }
}
