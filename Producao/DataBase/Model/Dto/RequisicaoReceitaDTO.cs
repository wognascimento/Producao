using System;

namespace Producao.DataBase.Model.Dto;

public class RequisicaoReceitaDTO
{
    public int CodComplAdicionalProduto { get; set; }
    public string PlanilhaProduto { get; set; }
    public string DescricaoProduto { get; set; }
    public string UnidadeProduto { get; set; }
    public int CodComplAdicionalReceita { get; set; }
    public string PlanilhaReceita { get; set; }
    public string DescricaoReceita { get; set; }
    public string UnidadeReceita { get; set; }
    public decimal Quantidade { get; set; }
    public string InseridoPor { get; set; }
    public DateTime InseridoEm { get; set; }
}

