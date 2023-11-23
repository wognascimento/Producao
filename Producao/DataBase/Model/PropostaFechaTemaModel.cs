using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("proposta_fecha_tema", Schema = "comercial")]
    public class PropostaFechaTemaModel
    {
        public long? cod_brief { get; set; }
        public string? ordem_escolha { get; set; }
        public string? tema { get; set; }
        public string? faixapreco { get; set; }
        public string? resp_conclusao_preco { get; set; }
        public DateTime? data_tema_fecha { get; set; }
        public int? indice { get; set; }
        public string? resp_tema { get; set; }
        public string? texto { get; set; }
        public long? idtema { get; set; }
    }
}
