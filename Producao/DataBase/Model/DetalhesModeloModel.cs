using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("qry_detalhes_modelo", Schema = "modelos")]
    public class DetalhesModeloModel
    {
        public long? id_modelo { get; set; }
        public string? planilha { get; set; }
        public string? planilha_modelo { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public double? qtd_modelo { get; set; }
        public double? qtd_producao { get; set; }
        public long? codcompladicional { get; set; }
        public string? observacao { get; set; }
        public double? pa { get; set; }
        public double? qtd {  get; set; }
    }
}
