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
    [Table("etiqueta_emitida", Schema = "producao")]
    public class EtiquetaEmitidaModel
    {
        public DateTime? data_de_expedicao {get; set;}
        public string? sigla {get; set;}
        public string? item_memorial {get; set;}
        public string? local_shoppings {get; set;}
        public string? planilha {get; set;}
        public string? descricao_completa {get; set;}
        public long? coddetalhescompl {get; set;}
        public long? codvol {get; set;}
        public long? volumes {get; set;}
        public long? volumes_total {get; set;}
        public double? qtd {get; set;}
        public string? criado_por {get; set;}
        public DateTime? criado_em { get; set; }
    }
}
