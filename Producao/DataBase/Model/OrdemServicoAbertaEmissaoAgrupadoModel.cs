using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_os_aberta_emissao_agrupado", Schema = "producao")]
    public class OrdemServicoAbertaEmissaoAgrupadoModel
    {
        public long? num_os_produto { get; set; }
        public long? tot_caminho { get; set; }
        public DateTime? data_emissao { get; set; }
        public string? responsavel_emissao { get; set; }
        public string? tipo { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public double? quantidade { get; set; }
        public string? distribuir_os { get; set; }
        public string? cliente { get; set; }
        public string? localizacao { get; set; }
        public long? num_os_servico { get; set; }
        public long? id_modelo {  get; set; }
    }
}
