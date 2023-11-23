using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("proposta_fecha_siglas", Schema = "comercial")]
    public class PropostaFechaSiglaModel
    {
        public string? tipo_evento { get; set; }
        public string? sigla { get; set; }
        public string? nome { get; set; }
        public long? codbriefing { get; set; }
        public string? diretorcliente { get; set; }
        public string? responsavelprojeto { get; set; }
        public string? praca { get; set; }
    }
}
