using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("proposta_dimensaodescricaocomercial", Schema = "comercial")]
    public class PropostaDimensaoDescricaoComercialModel
    {
        [Key]
        public long? coddimensao {  get; set; }
        public double? cargaeletrica_led { get; set; }
    }
}
