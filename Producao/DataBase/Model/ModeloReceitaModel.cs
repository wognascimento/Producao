using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_receita_detalhe", Schema = "modelos")]
    public class ModeloReceitaModel
    {
        [Key]
        public long? id_linha { get; set; }
        public long? id_modelo { get; set; }
        public long? codcompladicional { get; set; }
        public double? qtd_modelo { get; set; }
        public double? qtd_producao { get; set; }
        public string? cadastrado_por { get; set; }
        public DateTime? data_cadastro { get; set; }
        public string? alterado_por { get; set; }
        public DateTime? data_alterado { get; set; }
        public bool? cadastrado { get; set; }
        public int? mod1 { get; set; }
        public int? mod2 { get; set; }
        public int? mod3 { get; set; }
        public int? mod4 { get; set; }
        public int? mod5 { get; set; }
        public string? observacao { get; set; }
        public string? local { get; set; }
        public int? mod6 { get; set; }
        public int? mod7 { get; set; }
        public int? mod8 { get; set; }
        public int? mod9 { get; set; }
        public int? mod10 {  get; set; }
    }
}
