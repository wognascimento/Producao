using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_construcao_peca", Schema = "projetos")]
    public class ConstrucaoPecaModel
    {
        public long? id_detalhes { get; set; }
        public long? codcompladicional { get; set; }
        public long? id_construcao { get; set; }
        public int? item { get; set; }
        public string? descricao_peca { get; set; }
        public int? volume_etiqueta { get; set; }
        public int? volume_supermercado { get; set; }
        public string? volume_expedicao { get; set; }
        public string? cor { get; set; }
        public string? marca_tipo { get; set; }
        public int? ano { get; set; }
        public DateTime? data { get; set; }
        public string? responsavel { get; set; }
        public long? couniadicional { get; set; }
    }
}
