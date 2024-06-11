using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("proposta_descricaocomercial", Schema = "comercial")]
    public class PropostaDescricaoComercialModel
    {
        [Key]
        public long? coddesccoml { get; set; }
        public string? familia { get; set; }
        public string? descricaocomercial { get; set; }
        public string? alex { get; set; }
        public string? ativo { get; set; }
        public string? planilha_predomina { get; set; }
    }
}
