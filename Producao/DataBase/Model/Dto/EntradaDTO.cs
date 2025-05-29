using System;

namespace Producao.DataBase.Model.Dto;

public class EntradaDTO
{
    public long? codigo_entrada { get; set; }
    public long? codcompladicional  { get; set; }
    public double? quantidade  { get; set; }
    public string? procedencia  { get; set; }
    public string? processado  { get; set; }
    public DateTime? entrada_data  { get; set; }
    public string? entrada_por  { get; set; }
    public string? descricao_completa  { get; set; }
    public string? unidade { get; set; }
}

