using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("qry_aprovados", Schema = "producao")]
    public class AprovadoModel
    { 
        [Key]
        [Column("id_aprovado")]
        public long? IdAprovado { get; set; }
        [Column("ordem")]
        public double? Ordem { get; set; }
        [Column("ordem_sigla_serv")]
        public double? OrdemSiglaServ { get; set; }
        [Column("nivel")]
        public int? Nivel { get; set; }
        [Column("data_aprovado")]
        public DateTimeOffset? DataAprovado { get; set; }
        [Column("data_de_expedicao")]
        public DateTime? DataExpedicao { get; set; }
        [Column("meta_producao")]
        public DateTimeOffset? MetaProducao { get; set; }
        [Column("sigla")]
        public string? Sigla { get; set; }
        [Column("sigla_serv")]
        public string? SiglaServ { get; set; }
        [Column("nome")]
        public string? Nome { get; set; }
        [Column("tema")]
        public string? Tema { get; set; }
        [Column("obs_especial")]
        public string? ObsEspecial { get; set; }
        [Column("resp_cliente")]
        public string? RespCliente { get; set; }
        [Column("est")]
        public string? Est { get; set; }
        [Column("cidade")]
        public string? Cidade { get; set; }
        [Column("memo_resp")]
        public string? MemoResp { get; set; }
        [Column("memo_data")]
        public DateTime? MemoData { get; set; }
        [Column("chk_resp")]
        public string? ChkResp { get; set; }
        [Column("chk_data")]
        public DateTime? ChkData { get; set; }
        [Column("cronog_data")]
        public DateTimeOffset? CronogData { get; set; }
        [Column("cronog_resp")]
        public string? CronogResp { get; set; }
        [Column("as_built_plantas")]
        public string? AsBuiltPlantas { get; set; }
        [Column("as_built_plantas_data")]
        public DateTimeOffset? AsBuiltPlantasData { get; set; }
        [Column("rel_inflamabilidade")]
        public string? RelInflamabilidade { get; set; }
        [Column("data_rel_inflamabilidade")]
        public DateTime? DataRelInflamabilidade { get; set; }
        [Column("meta_rel_inflamabilidade")]
        public DateTime? MetaRelInflamabilidade { get; set; }
        [Column("pa")]
        public string? Pa { get; set; }
        [Column("mlamp_led")]
        public string? MlampLed { get; set; }
        [Column("obs_iluminacao")]
        public string? ObsIluminacao { get; set; }
        [Column("resp_memo_visual")]
        public string? RespMemoVisual { get; set; }
        [Column("data_memo_visual")]
        public DateTime? DataMemoVisual { get; set; }
        [Column("resp_cenas")]
        public string? RespCenas { get; set; }
        [Column("resp_trilha")]
        public string? RespTrilha { get; set; }
        [Column("data_reuniao_conceito")]
        public DateTime? DataReuniaoConceito { get; set; }
        [Column("resp_planta_pca")]
        public string? RespPlantaPca { get; set; }
        [Column("prazo_planta_pca")]
        public DateTime? PrazoPlantaPca { get; set; }
        [Column("conclusao_planta_pca")]
        public DateTime? ConclusaoPlantaPca { get; set; }
        [Column("resp_revisao_planta")]
        public string? RespRevisaoPlanta { get; set; }
        [Column("data_revisao_planta")]
        public DateTime? DataRevisaoPlanta { get; set; }
        [Column("obs_revisao")]
        public string? ObsRevisao { get; set; }
        [Column("ok_planta_pca")]
        public bool? OkPlantaPca { get; set; }
        [Column("planta_pca")]
        public string? PlantaPca { get; set; }
        [Column("liberacao_planta_pca")]
        public DateTimeOffset? LiberacaoPlantaPca { get; set; }
        [Column("resp_planta_mall")]
        public string? RespPlantaMall { get; set; }
        [Column("prazo_planta_mall")]
        public DateTime? PrazoPlantaMall { get; set; }
        [Column("ok_planta_mall")]
        public bool? OkPlantaMall { get; set; }
        [Column("planta_mall")]
        public string? PlantaMall { get; set; }
        [Column("conclusao_planta_mall")]
        public DateTimeOffset? ConclusaoPlantaMall { get; set; }
        [Column("laco")]
        public string? Laco { get; set; }
        [Column("acabamento_construcao")]
        public string? AcabamentoConstrucao { get; set; }
        [Column("acabamento_fibra")]
        public string? AcabamentoFibra { get; set; }
        [Column("acabamento_moveis")]
        public string? AcabamentoMoveis { get; set; }
        [Column("rede")]
        public string? Rede { get; set; }
        [Column("memorial_visual_liberado_por")]
        public string? MemorialVisualLiberadoPor { get; set; }
        [Column("memorial_visual_liberado_em")]
        public DateTime? MemorialVisualLiberadoEm { get; set; }
        [Column("somadecubagem_total")]
        public double? SomaDeCubagemTotal { get; set; }
        [Column("tipo_evento")]
        public string? TipoEvento { get; set; }
        [Column("divi_caminhao")]
        public string? DiviCaminhao { get; set; }
        [Column("resp_planta_fachada")]
        public string? RespPlantaFachada { get; set; }
        [Column("prazo_planta_fachada")]
        public DateTime? PrazoPlantaFachada { get; set; }
        [Column("ok_planta_fachada")]
        public bool? OkPlantaFachada { get; set; }
        [Column("planta_fachada")]
        public string? PlantaFachada { get; set; }
        [Column("conclusao_planta_fachada")]
        public DateTimeOffset? ConclusaoPlantaFachada { get; set; }
        [Column("resp_estruturas")]
        public string? RespEstruturas { get; set; }
        [Column("projeto_novo")]
        public string? ProjetoNovo { get; set; }
        [Column("cor_predominante")]
        public string? CorPredominante { get; set; }
        [Column("resp_planta_base")]
        public string? RespPlantaBase { get; set; }
        [Column("prazo_planta_base")]
        public DateTime? PrazoPlantaBase { get; set; }
        [Column("conclusao_planta_base")]
        public DateTime? ConclusaoPlantaBase { get; set; }
        [Column("resp_revisao_planta_base")]
        public string? RespRevisaoPlantaBase { get; set; }
        [Column("data_revisao_planta_base")]
        public DateTime? DataRevisaoPlantaBase { get; set; }
        [Column("ok_planta_base")]
        public bool? OkPlantaBase { get; set; }
        [Column("planta_base")]
        public string? PlantaBase { get; set; }
        [Column("liberacao_planta_base")]
        public DateTimeOffset? LiberacaoPlantaBase { get; set; }

    }
}
