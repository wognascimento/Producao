using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("qry_aprovados", Schema = "producao")]
    public class AprovadoModel
    { 
        [Key]
        public long? id_aprovado { get; set; }
        public double? ordem { get; set; }
        public double? ordem_sigla_serv { get; set; }
        public int? nivel { get; set; }
        public DateTime? data_aprovado { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public DateTime? meta_producao { get; set; }
        public string? sigla { get; set; }
        public string? sigla_serv { get; set; }
        public string? nome { get; set; }
        public string? tema { get; set; }
        public string? obs_especial { get; set; }
        public string? resp_cliente { get; set; }
        public string? est { get; set; }
        public string? cidade { get; set; }
        public string? memo_resp { get; set; }
        public DateTime? memo_data { get; set; }
        public string? chk_resp { get; set; }
        public DateTime? chk_data { get; set; }
        public DateTime? cronog_data { get; set; }
        public string? cronog_resp { get; set; }
        public string? as_built_plantas { get; set; }
        public DateTime? as_built_plantas_data { get; set; }
        public string? rel_inflamabilidade { get; set; }
        public DateTime? data_rel_inflamabilidade { get; set; }
        public DateTime? meta_rel_inflamabilidade { get; set; }
        public string? pa { get; set; }
        public string? mlamp_led { get; set; }
        public string? obs_iluminacao { get; set; }
        public string? resp_memo_visual { get; set; }
        public DateTime? data_memo_visual { get; set; }
        public string? resp_cenas { get; set; }
        public string? resp_trilha { get; set; }
        public DateTime? data_reuniao_conceito { get; set; }
        public string? resp_planta_pca { get; set; }
        public DateTime? prazo_planta_pca { get; set; }
        public DateTime? conclusao_planta_pca { get; set; }
        public string? resp_revisao_planta { get; set; }
        public DateTime? data_revisao_planta { get; set; }
        public string? obs_revisao { get; set; }
        public bool? ok_planta_pca { get; set; }
        public string? planta_pca { get; set; }
        public DateTime? liberacao_planta_pca { get; set; }
        public string? resp_planta_mall { get; set; }
        public DateTime? prazo_planta_mall { get; set; }
        public bool? ok_planta_mall { get; set; }
        public string? planta_mall { get; set; }
        public DateTime? conclusao_planta_mall { get; set; }
        public string? laco { get; set; }
        public string? acabamento_construcao { get; set; }
        public string? acabamento_fibra { get; set; }
        public string? acabamento_moveis { get; set; }
        public string? rede { get; set; }
        public string? memorial_visual_liberado_por { get; set; }
        public DateTime? memorial_visual_liberado_em { get; set; }
        public double? somadecubagem_total { get; set; }
        public string? tipo_evento { get; set; }
        public string? divi_caminhao { get; set; }
        public string? resp_planta_fachada { get; set; }
        public DateTime? prazo_planta_fachada { get; set; }
        public bool? ok_planta_fachada { get; set; }
        public string? planta_fachada { get; set; }
        public DateTime? conclusao_planta_fachada { get; set; }
        public string? resp_estruturas { get; set; }
        public string? projeto_novo { get; set; }
        public string? cor_predominante { get; set; }
        public string? resp_planta_base { get; set; }
        public DateTime? prazo_planta_base { get; set; }
        public DateTime? conclusao_planta_base { get; set; }
        //public string? resp_revisao_planta_base { get; set; }
        public DateTime? data_revisao_planta_base { get; set; }
        public bool? ok_planta_base { get; set; }
        public string? planta_base { get; set; }
        public DateTime? liberacao_planta_base { get; set; }
        
        public string? resp_planta_cercas { get; set; }
        public DateTime? prazo_planta_cercas { get; set; }
        public bool? conclusao_planta_cerca { get; set; }
        public string? planta_cercas_concluida_por { get; set; }
        public DateTime? data_conclusao_planta_cercas { get; set; }
        
        public string? resp_revisao_planta_base { get; set; }
        public bool? conclusao_revisao_planta_base { get; set; }
        public string? revisao_planta_base_concluida_por { get; set; }

        public string? resp_revisao_planta_praca { get; set; }
        public bool? conclusao_revisao_planta_praca { get; set; }
        public string? revisao_planta_praca_concluida_por { get; set; }
        public DateTime? data_conclusso_revisao_planta_praca { get; set; }

        public string? resp_revisao_final { get; set; }
        public bool? conclusao_revisao_final { get; set; }
        public string? revisao_final_concluida_por { get; set; }
        public DateTime? data_conclusao_revisao_final { get; set; }

        public string? ajuste { get; set; }
        public string? obs_ajuste { get; set; }

        public string? resp_vt { get; set; }
        public DateTime? data_vt { get; set; }
        public bool? conclusao_retorno_vt { get; set; }
        public string? resp_retorno_vt { get; set; }
        public DateTime? data_retorno_vt { get; set; }
        public string? alteracao_planta_praca_vt { get; set; }
        public string? obs_planta_praca_vt { get; set; }
        public string? alteracao_planta_mall_vt { get; set; }
        public string? obs_planta_mall_vt { get; set; }
        public string? alteracao_planta_fachada_vt { get; set; }
        public string? obs_planta_fachada_vt { get; set; }

        public string? resp_planta_corte_elevacao { get; set; }
        public DateTime? prazo_planta_corte_elevacao { get; set; }
        public bool? conclusao_planta_corte_elevacao { get; set; }
        public string? planta_corte_elevacao_concluida_por { get; set; }
        public DateTime? data_conclusao_planta_corte_elevacao { get; set; }

        public string? obs_gerais { get; set; }
        public string? resp_planta_as_built { get; set; }
        public DateTime? prazo_planta_as_built { get; set; }
        public bool? conclusao_planta_as_built { get; set; }
        
    }
}
