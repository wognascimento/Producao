using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qrychkgeral_complemento", Schema = "producao")]
    public class QryCheckListGeralComplementoModel
    {
        public long? coddetalhescompl { get; set; }
        public long? codcompladicional { get; set; }
        public double qtd { get; set; }
        public double? saldoestoque { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string? alterado_por { get; set; }
        public long? codcompl { get; set; }
        public string? confirmado { get; set; }
        public string? complementoadicional { get; set; }
        public string? unidade { get; set; }
        public string? local_producao { get; set; }
        public string? justificativa { get; set; }
        public string? resp_prod { get; set; }
        public string? confirmado_por { get; set; }
        public DateTime? confirmado_data { get; set; }
        public string? desabilitado_confirmado_por { get; set; }
        public DateTime? desabilitado_confirmado_data { get; set; }
        public double? transf_galpao { get; set; }
        public string? terceiro { get; set; }
        public DateTime? meta_producao { get; set; }
        public string? os { get; set; }
        public string? req { get; set; }
        public string? status_producao { get; set; }
        public string? status_transferencia { get; set; }
        public long? num_os_produto { get; set; }
        public double? qtd_expedida { get; set; }
    }
}
