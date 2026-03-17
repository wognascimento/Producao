namespace Producao.DataBase.Model.Dto;

public class CustoProdutoDTO
{
    public string? planilha { get; set; }
    public string? descricao { get; set; }
    public string? descricao_adicional { get; set; }
    public string? complementoadicional { get; set; }
    public long?   codcompladicional { get; set; }
    public string? descricao_completa { get; set; }
    public string? unidade { get; set; }
    public int?    vida_util { get; set; }
    public string? inativo { get; set; }
    public string? prodcontrolado { get; set; }
    public double? custo_atual { get; set; }
    public double? custo_anterior { get; set; }
    public double? custo_retrasado { get; set; }
    public string? tipocusto { get; set; }
    public double? custo_rec_atual { get; set; }
    public double? custo_rec_anterior { get; set; }
    public double? custo_rec_retrasado { get; set; }
    public double? process_atual { get; set; }
    public double? process_anterior { get; set; }
    public double? process_retrasado { get; set; }
    public double? media_pn_pro1_v { get; set; }
    public double? media_pn_pro2_v { get; set; }
    public double? media_rc_pro2_v { get; set; }
    public double? media_pro3_v { get; set; }
    public double? media_total_process { get; set; }


}
