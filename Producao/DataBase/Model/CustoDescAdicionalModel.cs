using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model;

[Table("tblcustodescadicional", Schema = "comercial")]
public class CustoDescAdicionalModel
{
    [Key]
    public long? codcompladicional { get; set; }
    public double? custo { get; set; }
    public string? tipocusto { get; set; }
    public string? tipoindice { get; set; }
    public double? valorindice { get; set; }
    public DateTime? dataaltera { get; set; }
    public string? alteradopor { get; set; }
    public double? indiceproposta { get; set; }
    public string? led { get; set; }
    public string? obs { get; set; }
    public double? customaoobra { get; set; }
    public double? custoproduto { get; set; }
    public int? complementoadicional_codcompladicional { get; set; }
    public int? custo_complementoadicional_codcompladicional { get; set; }
    public double? custo_recuperacao { get; set; }
    public double? pn_pro1 { get; set; }
    public double? pn_pro2 { get; set; }
    public double? rc_pro2 { get; set; }
    public double? pro3 { get; set; }
    public double? process { get; set; }
    public double? custo_proximo_ano { get; set; }
    public double? custo_rec_proximo_ano { get; set; }
    public double? custo_process_prox_ano { get; set; }
    public string? distancia { get; set; }
}
