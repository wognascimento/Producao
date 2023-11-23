using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("t_global", Schema = "ht")]
    public class TGlobalModel
    {
        [Key]
        public long? num_os { get; set; }
        public string? tipo_os { get; set; }
        public long? cod_setor { get; set; }
        public string? descricao_setor { get; set; }
        public string? descricao_servico { get; set; }
        public string? planilha { get; set; }
        public DateTime? data_emissao_os { get; set; }
        public DateTime? data_conclusao_os { get; set; }
        public double? quantidade_os { get; set; }
        public string? cliente_os { get; set; }
        public string? responsavel_os { get; set; }
        public string? cancelada_os { get; set; }
        public string? custo_despesa { get; set; }
        public string? direta_indireta { get; set; }
        public string? origem_custo_depto { get; set; }
        public string? destino_custo_depto { get; set; }
        public string? make_to { get; set; }
        public string? desc_juridica { get; set; }
        public string? lote { get; set; }
        public int? programacao_ordem { get; set; }
        public string? programacao_status { get; set; }
        public string? programacao_observacao { get; set; }
        public string? programacao_inserido_por { get; set; }
        public DateTime? programacao_inserido_data { get; set; }
        public double? ht { get; set; }
        public double? custo_mao_obra { get; set; }
        public long? produtos_servico_num_os_servico { get; set; }
        public string? agrupamento { get; set; }
        public string? ativo { get; set; }
        public string? backup { get; set; }
        public string? baia { get; set; }
        public string? cce_sob_encomenda { get; set; }
        public string? complexidade { get; set; }
        public string? coordenador { get; set; }
        public string? diretor_estoque { get; set; }
        public string? encarregado { get; set; }
        public string? est { get; set; }
        public string? estoque { get; set; }
        public string? familia_produto { get; set; }
        public string? ficha_tecnica { get; set; }
        public string? funcionalidade { get; set; }
        public string? gi { get; set; }
        public long? id { get; set; }
        public string? lead_time { get; set; }
        public string? lider_setor { get; set; }
        public string? local_de_armazenamento { get; set; }
        public string? origem { get; set; }
        public string? process { get; set; }
        public string? producao { get; set; }
        public string? resp_compras { get; set; }
        public string? retorno { get; set; }
        public string? seguranca { get; set; }
        public string? suporte_outra_unidade { get; set; }
        public string? tipo_custo { get; set; }
        public string? tipo_saldo { get; set; }
        public double? meta { get; set; }
    }
}
