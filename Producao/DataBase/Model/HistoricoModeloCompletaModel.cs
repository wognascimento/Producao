using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("view_historico_modelo_completa", Schema = "modelos")]
    public class HistoricoModeloCompletaModel
    {
        public long? id_modelo { get; set; }
        public string? obs_modelo { get; set; }
        public string? tema { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public string? descricao_completa { get; set; }
        public long? id_linha { get; set; }
        public long? itens_receita { get; set; }
        public string? planilha_receita { get; set; }
        public string? descricao_receita { get; set; }
        public string? descricao_adicional_receita { get; set; }
        public string? complementoadicional_receita { get; set; }
        public string? descricao_completa_receita { get; set; }
        public double? qtd_modelo_receita { get; set; }
        public double? qtd_producao_receita { get; set; }
        public long? ano { get; set; }
        public double? custo_total_receita {  get; set; }
        public string? observacao {  get; set; }
    }
}
