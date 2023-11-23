using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_modelos_gerar_os", Schema = "modelos")]
    public class ModeloGerarOsModel
    {
        public string? sigla { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public string? unidade { get; set; }
        public double? qtde { get; set; }
        public long? id_modelo { get; set; }
        public double? qtde_os { get; set; }
        public string? tema { get; set; }
        public long? codproduto { get; set; }
        public long? codcompladicional { get; set; }
        public string? obs { get; set; }
        public string? local_producao { get; set; }
        public string? producao { get; set; }
        public long? coduniadicional { get; set; }
        public DateTime? fechamento_shopp { get; set; }
        public string? descricao_completa { get; set; }
    }
}
