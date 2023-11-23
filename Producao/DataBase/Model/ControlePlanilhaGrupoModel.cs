using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("view_controle_planilha_grupo", Schema = "producao")]
    public class ControlePlanilhaGrupoModel
    {
        [Key]
        public long? coddetalhescompl { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public string? id { get; set; }
        public DateTime? fechamento_shopp { get; set; }
        public string? revisado { get; set; }
        public string? confirmado { get; set; }
        public string? justificativa { get; set; }
        public string? produto_novo { get; set; }
        public string? local_producao { get; set; }
        public string? planilha { get; set; }
        public string? sigla { get; set; }
        public string? tema { get; set; }
        public string? local_shoppings { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public double? qtd_complemento { get; set; }
        public double? qtd_detalhe { get; set; }
        public double? qtd_exp { get; set; }
        public string? complementoadicional { get; set; }
        public string? unidade { get; set; }
        public DateTime? producao { get; set; }
        public string? supermercado { get; set; }
        public DateTime? enviado_baia { get; set; }
        public string? obs_planilheiro { get; set; }
        public string? req { get; set; }
        public string? os { get; set; }
        public string? transf { get; set; }
        public string? orient_montagem { get; set; }
        public string? obs { get; set; }
        public long? codcompl { get; set; }
        public string? item { get; set; }
        public string? resp_cenas { get; set; }
        public string? resp_trilha { get; set; }
        public long? num_os_produto { get; set; }
        public string? caminhao { get; set; }
        public string? agrupamento { get; set; }
        public string? resp_estruturas { get; set; }
        public string? tamanho_construcao { get; set; }
        public string? resp_prod { get; set; }
        public string? descricao_completa { get; set; }
    }
}
