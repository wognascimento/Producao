using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("qrybaseanalisecliente1b", Schema = "producao")]
    public class BaseAnaliseClienteModel
    {
        public DateTime? data_de_expedicao { get; set; }
        public DateTime? fechamento_shopp { get; set; }
        public string? sigla { get; set; }
        public string? id { get; set; }
        public string? item_memorial { get; set; }
        public string? local_shoppings { get; set; }
        public string? planilha { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public double? qtd_compl { get; set; }
        public string? complementoadicional { get; set; }
        public double? qtd_detalhe { get; set; }
        public string? orient_producao { get; set; }
        public string? orient_montagem { get; set; }
        public long? coddetalhescompl { get; set; }
        public long? codcompl { get; set; }
        public double? somadeqtd_expedida { get; set; }
        public string? resp_prod { get; set; }
        public string? unidade { get; set; }
        public long? codcompladicional { get; set; }
        public long? nivel { get; set; }
        public string? confirmado { get; set; }
        public string? revisado { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string? alterado_por { get; set; }
        public string? carga { get; set; }
        public string? caminhao { get; set; }
        public string? coordenador { get; set; }
    }
}
