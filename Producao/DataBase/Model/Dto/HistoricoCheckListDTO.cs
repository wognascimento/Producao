namespace Producao.DataBase.Model.Dto;

public class HistoricoCheckListDTO
{
    public int ano { get; set; }
    public string sigla { get; set; }
    public string tema { get; set; }
    public int codcompl { get; set; }
    public string ordem { get; set; }
    public string item_memorial { get; set; }
    public string local_shoppings { get; set; }
    public string obs { get; set; }
    public string orient_montagem { get; set; }
    public string orient_desmont { get; set; }
    public int codproduto { get; set; }
    public int coduniadicional { get; set; }
    public string planilha { get; set; }
    public double qtd_chk { get; set; }
    public int coddetalhescompl { get; set; }
    public int codcompladicional { get; set; }
    public string descricao_completa { get; set; }
    public double qtd_comple { get; set; }
}
