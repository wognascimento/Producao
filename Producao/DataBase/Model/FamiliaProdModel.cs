using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao
{
    [Table("tblfamiliaprod", Schema = "compras")]
    public class FamiliaProdModel
    {
        [Key]
        public long codigofamilia { get; set; }
        public string nomefamilia { get; set; }
        public string res_compra { get; set;}
    }
}
