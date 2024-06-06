using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("clientes", Schema = "comercial")]
    public class ClienteModel
    {
        [Key]        
        public string? sigla { get; set; }
        public string? nome { get; set; }
        public string? endereco { get; set; }
        public string? cidade { get; set; }
        public string? bairro { get; set; }
        public string? est { get; set; }
    }
}
