using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Keyless]
    [Table("qry_kit_chkgeral", Schema = "kitsolucao")]
    public class KitChkGeralModel
    {
        public long? t_os_mont {get; set; }
        public string? shopping {get; set; }
        public long? distancia {get; set; }
        public string? cidade {get; set; }
        public string? est {get; set; }
        public DateTime? data_emissao {get; set; }
        public long? os {get; set; }
        public DateTime? data_solicitacao {get; set; }
        public DateTime? hora_solicitacao {get; set; }
        public string? solicitante {get; set; }
        public DateTime? concluir_ate {get; set; }
        public DateTime? hora_concluir {get; set; }
        public string? forma_de_envio {get; set; }
        public string? responsavel {get; set; }
        public string? obs_de_envio {get; set; }
        public double? qtd {get; set; }
        public string? planilha {get; set; }
        public string? descricao_completa {get; set; }
        public string? orient_montagem {get; set; }
        public string? unidade {get; set; }
        public string? noite_montagem {get; set; }
        public double? kp {get; set; }
        public long? kp2 {get; set; }
        public string? responsavel_producao {get; set; }
        public string? sigla {get; set; }
        public long? coddetalhescompl {get; set; }
        public long? codcompladicional {get; set; }
        public double? peso {get; set; }
        public double? custo {get; set; }
        public string? obs {get; set; }
        public string? responsavel_pcp {get; set; }
        public string? pcp_tab {get; set; }
        public string? pcp_jac {get; set; }
        public string? class_solucao {get; set; }
        public string? tipo_manutencao {get; set; }
        public DateTime? data {get; set; }
        public string? atendente {get; set; }
        public string? coordenador {get; set; }
        public string? motivos { get; set; }
        public string? nome { get; set; }
    }
}
