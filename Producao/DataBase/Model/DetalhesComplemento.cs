using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbldetalhescomplemento", Schema = "producao")]
    public class DetalhesComplemento
    {
        [Key]
        public long? coddetalhescompl { get; set; }
        public long? codcompladicional { get; set; }
        public double qtd  { get; set; }
        public DateTime? data_alteracao  { get; set; }
        public string? alterado_por  { get; set; }
        public long? codcompl  { get; set; }
        public long? id_modelo  { get; set; }
        public string? confirmado  { get; set; }
        public string? os  { get; set; }
        public string? req  { get; set; }
        public string? transf  { get; set; }
        public string? local_producao  { get; set; }
        public string? justificativa  { get; set; }
        public DateTime? producao  { get; set; }
        public string? supermercado  { get; set; }
        public DateTime? enviado_baia  { get; set; }
        public string? obs_planilheiro  { get; set; }
        public string? resp_prod  { get; set; }
        public string? confirmado_por  { get; set; }
        public DateTime? confirmado_data  { get; set; }
        public string? desabilitado_confirmado_por  { get; set; }
        public DateTime? desabilitado_confirmado_data  { get; set; }
        public double? transf_galpao  { get; set; }
        public string? terceiro  { get; set; }
        public string? em_producao  { get; set; }
        public DateTime? meta_producao  { get; set; }
        public long? num_os_produto  { get; set; }
        public DateTime? data_inserido  { get; set; }
        public string? inserido_por  { get; set; }
        public string? status_producao  { get; set; }
        public string? status_transferencia  { get; set; }
        public string? status_atualizado_por  { get; set; }
        public DateTime? status_atualizado_em  { get; set; }
    }
}
