using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("view_controle_memorial", Schema = "producao")]
    public class ControleMemorialModel
    {
        [Key]
        public long? cod_linha_qdfecha {get; set; }
        public DateTime? data_aprovado {get; set; }
        public string? sigla {get; set; }
        public string? sigla_serv {get; set; }
        public DateTime? memo_data {get; set; }
        public DateTime? data_memo_visual {get; set; }
        public string? item {get; set; }
        public string? tema {get; set; }
        public string? familia {get; set; }
        public double? qtd {get; set; }
        public string? descricaocomercial {get; set; }
        public string? dimensao {get; set; }
        public int? bloco {get; set; }
        public DateTime? data_revisado {get; set; }
        public string? obs_memorial {get; set; }
        public string? obs_fecha {get; set; }
        public string? obs_interna {get; set; }
        public string? obs_alteracao {get; set; }
        public string? status {get; set; }
        public string? liberado {get; set; }
        public string? ok {get; set; }
        public string? resp_revisao {get; set; }
        public DateTime? prazo_revisao {get; set; }
        public string? obs_revisao {get; set; }
        public string? local {get; set; }
        public string? altera_ok {get; set; }
        public string? confirma_alteracao_por {get; set; }
        public DateTime? confirma_alteracao_data {get; set; }
        public string? memorial_alterado_por {get; set; }
        public DateTime? memorial_data_alterado {get; set; }
        public DateTime? fechamento_shopp {get; set; }
        public string? revisado_por {get; set; }
        public DateTime? data_revisado_por {get; set; }
        public DateTime? data_de_expedicao {get; set; }
        public DateTime? conclusao_planta_pca {get; set; }
        public string? motivo_alt_pos_revisao {get; set; }
        public string? ok_revisao_alterada {get; set; }
        public string? revisao_alt_por {get; set; }
        public DateTime? data_alt_revisao {get; set; }
        public string? detalhe_local {get; set; }
        public string? pendencia { get; set; }
    }
}
