using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("temas", Schema = "comercial")]
    public class TemaModel
    {
        [Key]
        public long? idtema {get; set; }
        public string? temas {get; set; }
        public double? ano {get; set; }
        public string? tipos {get; set; }
        public string? caracteristicas {get; set; }
        public string? atalho {get; set; }
        public double? cod_tema {get; set; }
        public string? catalogo2006 {get; set; }
        public string? dir_ideia {get; set; }
        public string? dir_criacao {get; set; }
        public string? assist_tema {get; set; }
        public string? palavra_chave {get; set; }
        public string? ativo {get; set; }
        public double? inic {get; set; }
        public double? video {get; set; }
        public string? novo {get; set; }
        public string? descricao {get; set; }
        public double? preco {get; set; }
        public string? idealizador {get; set; }
        public string? obs {get; set; }
        public string? temas_novos_2007 {get; set; }
        public string? local_tema {get; set; }
        public string? inserido_por {get; set; }
        public DateTime? data_cadastro {get; set; }
        public string? nome_ingles {get; set; }
        public string? nome_espanhol {get; set; }
        public string? faixapreco {get; set; }
        public double? indiceproposta {get; set; }
        public string? briefing {get; set; }
        public long? idvalor {get; set; }
        public long? estoque {get; set; }
        public long? pedido {get; set; }
        public long? aprovado {get; set; }
        public string? fichatema {get; set; }
        public DateTime? data_atualizacao {get; set; }
        public string? status {get; set; }
        public string? alterado_por {get; set; }
        public DateTime? data_altera {get; set; }
        public string? nome_fantasia {get; set; }
        public string? composto {get; set; }
        public string? kit_fotos {get; set; }
        public string? planta_referencia {get; set; }
        public string? reuniao_conceito {get; set; }
        public string? orient_catalogo {get; set; }
        public string? exibe_catalogo {get; set; }
        public string? maquete {get; set; }
        public string? tema_interno {get; set; }
        public string? obs_tema_interno {get; set; }
        public string? construcao_tipo {get; set; }
        public string? construcao_principal {get; set; }
        public string? interatividade_principal {get; set; }
        public string? cor_predominante {get; set; }
        public string? animatronico {get; set; }
        public string? conceito_2 {get; set; }
        public string? conceito_3 {get; set; }
        public string? linha {get; set; }
        public string? codinome { get; set; }
    }
}
