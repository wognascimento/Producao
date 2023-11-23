using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao
{
    [Keyless]
    [Table("view_historico_modelo", Schema = "modelos")]
    public class HistoricoModelo
    {
        public long? codcompladicional_modelo {get; set; }
        public long? idtema { get; set; }
        public long? codcompladicional_receita {get; set; }
        public double? media_qtd_modelo {get; set; }
        public double? media_qtd_producao {get; set; }
        public string? descricao_completa {get; set; }
        public string? inativo { get; set; }
    }
}
