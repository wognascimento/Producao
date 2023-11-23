using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("t_controlados_recebidos", Schema = "expedicao")]
    public class ControladoRecebidoModel
    {
        [Key, Column(Order = 0)]
        public long? id_aprovado { get; set; }
        [Key, Column(Order = 1)]
        public long? codcompladicional { get; set; }
        public double? qtd { get; set; }
        public string? atualizado_por { get; set; }
        public DateTime? atualizado_em { get; set; }
        public string? cancelar_cobraca { get; set; }
        public string? justificativa { get; set; }
        public string? entrada_estoque { get; set; }
        public string? entrada_estoque_por { get; set; }
        public DateTime? entrada_estoque_em { get; set; }
    }
}
