using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("qry_chklist_nao_completado", Schema = "producao")]
    public class ChklistNaoCompletadoModel
    {
        public long? coddetalhescompl { get; set; }
        public string? ordem { get; set; }
        public string? agrupamento { get; set; }
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
        public double? qtd { get; set; }
        public long? coduniadicional { get; set; }
        public DateTime? dataalteradescadic { get; set; }
        public string? alteradopordescadic { get; set; }
        public long? codcompl { get; set; }
        public int? nivel { get; set; }
        public string? descricao { get; set; }
        public string? planilha { get; set; }
        public string? descricao_adicional { get; set; }
        public string? ok { get; set; }
        public string? confirmado { get; set; }
        public DateTime? fechamento_shopp { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public string? baia_local { get; set; }
        public string? ok_revisao_alterada { get; set; }
        public long? id_aprovado { get; set; }
    }
}
