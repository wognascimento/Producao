using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao.DataBase.Model
{
    public class EstabilidadeItem
    {
        public long? cod_linha_qdfecha { get; set; }
        public string? familia { get; set; }
        public string? descricaocomercial { get; set; }
        public long? coddesccoml { get; set; }
        public string? dimensao { get; set; }
        public string? local { get; set; }
        public string? detalhe_local { get; set; }
        public long? coddimensao { get; set; }
        public string? relatorio_estabilidade { get; set; }
        public string? sigla { get; set; }
    }
}
