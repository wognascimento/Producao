using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_os_emissao_producao_imprimir", Schema = "producao")]
    public class OsEmissaoProducaoImprimirModel
    {
        public long? num_os_servico { get; set; }
        public long? num_os_produto { get; set; }
        public string? tipo { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public string? setor_caminho { get; set; }
        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
        public double? quantidade { get; set; }
        public string? orientacao_caminho { get; set; }
        public string? retrabalho { get; set; }
        public string? cancelada_os { get; set; }
        public string? setor_caminho_proximo { get; set; }
        public DateTime? emitida_data { get; set; }
        public string? emitida_por { get; set; }
        public DateTime? recebido_setor_data { get; set; }
        public DateTime? concluida_os_data { get; set; }
        public string? cliente { get; set; }
        public string? tema { get; set; }
        public DateTime? meta_data { get; set; }
        public string? turno { get; set; }
        public string? responsavel_emissao_os { get; set; }
        public string? distribuir_os { get; set; }
        public double? distancia { get; set; }
        public int? nivel { get; set; }
        public long? cod_compl_adicional { get; set; }
        public string? nome { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public float? meta { get; set; }
        public string? resp_cliente { get; set; }
        public int? pagina { get; set; }
        public string? acabamento_construcao { get; set; }
        public string? acabamento_fibra { get; set; }
        public string? acabamento_moveis { get; set; }
        public string? laco { get; set; }
        public string? obs_iluminacao { get; set; }
        public string? solicitado_por { get; set; }
        public string? impresso { get; set; }
        public double? meta_peca { get; set; }
        public TimeSpan? meta_peca_hora {  get; set; }
        public long? codigo_setor { get; set; }
        public long? num_caminho { get; set; }
    }
}
