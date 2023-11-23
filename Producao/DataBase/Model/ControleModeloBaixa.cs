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
    [Table("qry_controle_modelo_baixa", Schema = "modelos")]
    public class ControleModeloBaixa
    {
        public string? sigla {get; set; }
        public string? item_memorial {get; set; }
        public string? tema {get; set; }
        public string? local_shoppings {get; set; }
        public long? codcompladicional {get; set; }
        public string? planilha {get; set; }
        public string? descricao {get; set; }
        public string? descricao_adicional {get; set; }
        public string? complementoadicional {get; set; }
        public string? unidade {get; set; }
        public long? codcompl {get; set; }
        public double? qtd_chk {get; set; }
        public long? coddetalhescompl {get; set; }
        public double? qtd_compl_chk {get; set; }
        public string? obs {get; set; }
        public long? id_modelo {get; set; }
        public long? codproduto {get; set; }
        public long? coduniadicional {get; set; }
        public string? local_producao {get; set; }
        public DateTime? fechamento_shopp {get; set; }
        public string? producao {get; set; }
        public long? id_aprovado {get; set; }
        public long? idtema {get; set; }
        public string? descricao_completa { get; set; }
    }
}
