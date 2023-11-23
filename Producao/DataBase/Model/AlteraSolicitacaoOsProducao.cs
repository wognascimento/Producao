using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_altera_solicitacao_os_producao", Schema = "producao")]
    public class AlteraSolicitacaoOsProducao
    {
        public long? num_os_produto { get; set; }
        public string? tipo { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public double? quantidade { get; set; }
        public DateTime? data_emissao { get; set; }
        public string? responsavel_emissao { get; set; }
        public long? cod_compl_adicional { get; set; }
    }
}
