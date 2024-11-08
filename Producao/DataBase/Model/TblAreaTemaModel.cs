using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_area_temas", Schema = "producao")]
    public class TblAreaTemaModel
    {
        [Key]
        public long codareatema { get; set; } = 0;
        public string? sigla { get; set; } 
        public string? tema { get; set; }
        public int? ano { get; set; } = Convert.ToInt32(DataBaseSettings.Instance.Database); //DateTime.Now.Year;
        public string? local { get; set; } 
        public string? trem { get; set; } 
        public double? area_total_memorial { get; set; } 
        public double? area_total_planta { get; set; } 
        public double? trilha_memorial { get; set; } 
        public double? trilha_planta { get; set; } 
        public double? pa { get; set; } 
        public double? planta_liquida { get; set; } 
        public double? perimetro_planta { get; set; } 
        public double? construcao_total { get; set; } 
        public double? cenografia_planta { get; set; } 
        public string? incluido_por { get; set; } 
        public DateTime? data_inclusao { get; set; } 
        public string? alterad_por { get; set; } 
        public DateTime? data_altera { get; set; }
    }
}
