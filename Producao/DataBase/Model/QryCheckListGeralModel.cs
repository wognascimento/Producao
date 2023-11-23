using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qrychkgeral", Schema = "producao")]
    public class QryCheckListGeralModel
    {
        public string? sigla { get; set; }
        public string? id { get; set; }
        public string? item_memorial { get; set; }
        public string? carga { get; set; }
        public string? local_shoppings { get; set; }
        public string? planilha { get; set; }
        public double qtd { get; set; }
        public string? descricao { get; set; }
        public string? descricao_adicional { get; set; }
        public string? orient_montagem { get; set; }
        public string? obs { get; set; }
        public string? alteradopor { get; set; }
        public DateTime? dataalteracaodesc { get; set; }
        public long? codcompl { get; set; }
        public string? orient_desmont { get; set; }
        public long? coduniadicional { get; set; }
        public long? codigo { get; set; }
        public string? revisao { get; set; }
        public string? obsproducaoobrigatoria { get; set; }
        public string? incluidopordesc { get; set; }
        public string? unidade { get; set; }
        public int? bloco_revisao { get; set; }
        public string? resp_revisao { get; set; }
        public string? ok_revisao_alterada { get; set; }
        public double? kp { get; set; }
        public string? class_solucao { get; set; }
        public string? motivos { get; set; }
        public int? condicao { get; set; }
        public long? id_aprovado { get; set; }
        public string? historico { get; set; }
        public int? nivel { get; set; }
        public string? agrupar { get; set; }
        public string? alteradopordescadic { get; set; }
        public DateTime? dataalteradescadic { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }
        public string? alterado_por { get; set; }
        public DateTime? alterado_em { get; set; }
    }
}
