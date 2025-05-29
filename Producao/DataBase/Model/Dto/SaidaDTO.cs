using System;

namespace Producao.DataBase.Model.Dto;

public class SaidaDTO
{
    public long? codigo_saida { get; set; }
    public long? codcompladicional { get; set; }
    public double? quantidade { get; set; }
    public string? destino { get; set; }
    public string? processado { get; set; }
    public DateTime? saida_data { get; set; }
    public string? saida_por { get; set; }
    public string? descricao_completa { get; set; }
    public string? unidade { get; set; }
}
