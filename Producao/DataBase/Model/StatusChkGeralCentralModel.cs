using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_status_chk_geral_central", Schema = "modelos")]
    public class StatusChkGeralCentralModel
    {
        public string? sigla { get; set; }
        public string? tema { get; set; }
        public long? idtema { get; set; }
        public string? local_shoppings { get; set; }
        public string? ordem { get; set; }
        public string? item_memorial { get; set; }
        public string? obs { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public long? codproduto { get; set; }
        public long? codcompl { get; set; }
        public double? qtde_chk { get; set; }
        public long? coduniadicional { get; set; }
        public string? descricao_adicional { get; set; }
        public long? codcompladicional { get; set; }
        public string? complementoadicional { get; set; }
        public long? coddetalhescompl { get; set; }
        public double? qtde_compl { get; set; }
        public long? id_modelo { get; set; }
        public double? qtde_os { get; set; }
        public string? status { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public string? galpao { get; set; }
        public DateTime? fechamento_shopp { get; set; }
        public string? em_producao { get; set; }
        public string? obs_iluminacao { get; set; }
        public DateTime? data_memo_visual { get; set; }
        public string? obs_especial {  get; set; }
        public string? descricao_completa {  get; set; }
        public string? laco {  get; set; }
    }
}
