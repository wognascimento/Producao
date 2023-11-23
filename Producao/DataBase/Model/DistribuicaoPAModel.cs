using Microsoft.EntityFrameworkCore;
//using Microsoft.Office.Interop.Excel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_detalhes_modelo_distribuicao_pa_excel", Schema = "modelos")]
    public class DistribuicaoPAModel
    {
        public long? codcompladicional { get; set; }
        public string? produto { get; set; }
        public long? id_modelo { get; set; }
        public string? tema { get; set; }
        public string? cliente { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? complementoadicional { get; set; }
        public string? descricao_produto { get; set; }
        public string? unidade { get; set; }
        public double? qtd_modelo { get; set; }
        public double? qtd_producao { get; set; }
        public string? observacao { get; set; }
        public double? qtd { get; set; }
        public double? t { get; set; }
        public double? p { get; set; }
        public double? anel1 { get; set; }
        public double? anel2 { get; set; }
        public double? anel3 { get; set; }
        public double? anel4 { get; set; }
        public double? anel5 { get; set; }
        public double? anel6 { get; set; }
        public double? anel7 { get; set; }
        public double? anel8 { get; set; }
        public double? anel9 { get; set; }
        public double? anel10 { get; set; }
        public double? anel11 { get; set; }
        public double? anel12 { get; set; }
        public double? anel13 { get; set; }
        public double? anel14 { get; set; }
        public double? anel15 { get; set; }
        public double? anel16 { get; set; }
        public double? anel17 { get; set; }
        public double? anel18 { get; set; }
        public double? anel19 { get; set; }
        public double? anel20 { get; set; }
        public double? anel21 { get; set; }
        public double? anel22 { get; set; }
        public double? ponga { get; set; }
        public double? tripe { get; set; }
        public double? anel_1 { get; set; }
        public double? anel_2 { get; set; }
        public double? anel_3 { get; set; }
        public double? anel_4 { get; set; }
        public double? anel_5 { get; set; }
        public double? anel_6 { get; set; }
        public double? anel_7 { get; set; }
        public double? anel_8 { get; set; }
        public double? anel_9 { get; set; }
        public double? anel_10 { get; set; }
        public double? anel_11 { get; set; }
        public double? anel_12 { get; set; }
        public double? anel_13 { get; set; }
        public double? anel_14 { get; set; }
        public double? anel_15 { get; set; }
        public double? anel_16 { get; set; }
        public double? anel_17 { get; set; }
        public double? anel_18 { get; set; }
        public double? anel_19 { get; set; }
        public double? anel_20 { get; set; }
        public double? anel_21 { get; set; }
        public double? anel_22 { get; set; }
        public double? total_galhos { get; set; }
        public string? inativo {  get; set; }

    }
}
