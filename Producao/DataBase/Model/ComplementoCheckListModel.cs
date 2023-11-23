using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("t_complemento_chk", Schema = "producao")]
    public class ComplementoCheckListModel
    {
        [Key]
        public long? codcompl { get; set; }
        public string? ordem { get; set; }
        public string? sigla { get; set; }
        public string? local_shoppings { get; set; }
        public long? codproduto { get; set; }
        public string? obs { get; set; }
        public DateTime? dataalteracaodesc { get; set; }
        public string? alteradopor { get; set; }
        public string? orient_montagem { get; set; }
        public string? item_memorial { get; set; }
        public DateTime? datainclusaodesc { get; set; }
        public string? incluidopordesc { get; set; }
        public double? kp { get; set; }
        public int? kp2 { get; set; }
        public string? orient_desmont { get; set; }
        public double qtd { get; set; }
        public long? coduniadicional { get; set; }
        public DateTime? dataalteradescadic { get; set; }
        public string? alteradopordescadic { get; set; }
        public int? nivel { get; set; }
        public string? carga { get; set; }
        public string? class_solucao { get; set; }
        public long? id_aprovado { get; set; }
        public string? historico { get; set; }
        public string? agrupar { get; set; }
        public string? motivos { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }
        public string? alterado_por { get; set; }
        public DateTime? alterado_em { get; set; }
    }
}
