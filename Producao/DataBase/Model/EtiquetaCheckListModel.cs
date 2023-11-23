using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qryetiquetachkgeral", Schema = "producao")]
    public class EtiquetaCheckListModel
    {
        public string? sigla {get; set;}
        public string? item_memorial {get; set;}
        public string? local_shoppings {get; set;}
        public string? planilha {get; set;}
        public string? descricao {get; set;}
        public string? descricao_adicional {get; set;}
        public string? complementoadicional {get; set;}
        public string? descricao_completa {get; set;}
        public long? coddetalhescompl {get; set;}
        public double? qtd_detalhe {get; set;}
        public double? qtd_nao_expedida {get; set;}
        public DateTime? data_de_expedicao {get; set;}
        public bool? etiqueta_emitida { get; set; }
    }
}
