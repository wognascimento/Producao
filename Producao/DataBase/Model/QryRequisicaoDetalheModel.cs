using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_req_detalhes_relatorio_os_chk", Schema = "producao")]
    public class QryRequisicaoDetalheModel
    {
        public long? num_requisicao {get; set; }
        public string? planilha {get; set; }
        public string? descricao {get; set; }
        public string? descricao_adicional {get; set; }
        public string? complementoadicional {get; set; }
        public string? descricao_completa {get; set; }
        public double? quantidade {get; set; }
        public long? codcompladicional {get; set; }
        public long? num_os_servico {get; set; }
        public DateTime? data {get; set; }
        public string? alterado_por {get; set; }
        public string? barcode {get; set; }
        public long? codigo_setor {get; set; }
        public string? setor_caminho {get; set; }
        public string? cliente {get; set; }
        public string? unidade {get; set; }
        public string? ok_expedido {get; set; }
        public string? observacao {get; set; }
        public string? tipo {get; set; }
        public string? tema {get; set; }
        public string? item_memorial {get; set; }
        public long? coddetalhescompl {get; set; }
        public string? sigla {get; set; }
        public string? produtocompleto {get; set; }
        public long? cod_det_req {get; set; }
        public long? cod_produto {get; set; }
        public long? cod_desc_adicional {get; set; }
        public long? cod_compl_adicional { get; set; }
        public long? coduniadicional { get; set; }
        public long? codigo { get; set; }
        public double? saldo_estoque { get; set; }
        public int? volume { get; set; }
    }
}
