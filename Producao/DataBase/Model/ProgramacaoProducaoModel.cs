using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("qry_programacao_producao_global_producao", Schema = "producao")]
    public class ProgramacaoProducaoModel
    {
        public int? programacao_ordem { get; set; }
        public string? cliente_os { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public long? num_os { get; set; }
        public double? quantidade_os { get; set; }
        public string? tipo_os { get; set; }
        public string? setor_caminho { get; set; }
        public string? orientacao_caminho { get; set; }
        public string? setor_caminho_proximo { get; set; }
        public string? programacao_status { get; set; }
        public string? programacao_observacao { get; set; }
        public DateTime? data_emissao_os { get; set; }
        //public DateTime? fechamento_shopp { get; set; }
        public double? distancia { get; set; }
        public string? cancelada_os { get; set; }
        public long? codigo_setor { get; set; }
        public DateTime? data_conclusao_os { get; set; }
        public DateTime? data_de_expedicao { get; set; }
        public string? localizacao { get; set; }
        public double? ht { get; set; }
        public string? programacao_inserido_por { get; set; }
        public DateTime? programacao_inserido_data { get; set; }
        public double? custo { get; set; }
        public double? custo_recuperacao { get; set; }
        public double? custo_mao_obra { get; set; }
        //public string? dias_fechamento { get; set; }
        public int? dias_expedicao { get; set; }
        public long? cod_compl_adicional { get; set; }
    }
}
