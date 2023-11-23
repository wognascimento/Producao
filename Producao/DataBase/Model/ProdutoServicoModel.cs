using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_produtos_servico", Schema = "producao")]
    public class ProdutoServicoModel
    {
        [Key]
        public long? num_os_servico {set; get;}
        public long? num_os_produto {set; get;}
        public string? tipo {set; get;}
        public long? codigo_setor {set; get;}
        public string? setor_caminho {set; get;}
        public double? quantidade {set; get;}
        public DateTime? data_inicio {set; get;}
        public DateTime? data_fim {set; get;}
        public string? cliente {set; get;}
        public string? tema {set; get;}
        public string? orientacao_caminho {set; get;}
        public long? codigo_setor_proximo {set; get;}
        public string? setor_caminho_proximo {set; get;}
        public string? fase {set; get;}
        public string? responsavel_emissao_os {set; get;}
        public string? emitida_por {set; get;}
        public DateTime? emitida_data {set; get;}
        public DateTime? meta_data {set; get;}
        public string? turno {set; get;}
        public string? ajuste_projeto {set; get;}
        public string? cancelada_os {set; get;}
        public string? retrabalho {set; get;}
        public DateTime? recebido_setor_data {set; get;}
        public DateTime? concluida_os_data {set; get;}
        public string? impresso {set; get;}
        public long? cod_detalhe_compl {set; get;}
        public long? id_modelo {set; get;}
        public string? alterado_por {set; get;}
        public DateTime? alterado_data {set; get;}
        public string? status {set; get;}
        public DateTime? data_status {set; get;}
        public string? status_por {set; get;}
        public string? motivo_cancelamento {set; get;}
        public string? aprovado {set; get;}
        public string? aprovado_por {set; get;}
        public DateTime? aprovado_em {set; get;}
        public int? programacao_ordem {set; get;}
        public string? programacao_status {set; get;}
        public string? programacao_observacao {set; get;}
        public string? programacao_inserido_por {set; get;}
        public DateTime? programacao_inserido_data {set; get;}
        public DateTime? meta_lider {set; get;}
        public int? pagina { set; get; }
    }
}
