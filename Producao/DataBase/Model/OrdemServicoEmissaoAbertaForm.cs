using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_os_emissao_aberta_form", Schema = "producao")]
    public class OrdemServicoEmissaoAbertaForm
    {
        public string? tema { get; set; }
        public DateTime? fechamento_shopp { get; set; }
        public long? num_os_produto { get; set; }
        public long? num_caminho { get; set; }
        public DateTime? data_emissao { get; set; }
        public string? responsavel_emissao { get; set; }
        public string? tipo { get; set; }
        public double? quantidade { get; set; }
        public string? orientacao_caminho { get; set; }
        public string? distribuir_os { get; set; }
        public string? setor_caminho { get; set; }
        public string? cliente { get; set; }
        public string? localizacao { get; set; }
        public long? id_modelo { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public long? codigo_setor { get; set; }
        public long? num_os_servico {  get; set; }
        public string? descricao_completa {  get; set; }
        public bool? cancelar { get; set; }
        public long? cod_obs { get; set; }
    }
}
