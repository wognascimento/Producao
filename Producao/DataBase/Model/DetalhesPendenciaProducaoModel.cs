using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("detalhes_pendencia_producao", Schema = "pcp")]
    public class DetalhesPendenciaProducaoModel
    {
        public DateTime? saida_caminao { get; set; } 
        public DateTime? data_aprovado { get; set; } 
        public DateTime? fechamento_shopp { get; set; } 
        public double? distancia { get; set; } 
        public string? sigla { get; set; } 
        public long? nivel { get; set; } 
        public string? coordenador { get; set; } 
        public string? local_shoppings { get; set; } 
        public string? resp_cenas { get; set; } 
        public string? planilha { get; set; } 
        public string? descricao { get; set; } 
        public string? descricao_adicional { get; set; } 
        public string? complementoadicional { get; set; } 
        public string? unidade { get; set; } 
        public long? codcompl { get; set; } 
        public long? coddetalhescompl { get; set; } 
        public double? qtd_chlist { get; set; } 
        public double? qtd_det_compl { get; set; } 
        public double? somadeqtd_expedida { get; set; } 
        public string? revisado { get; set; } 
        public string? confirmado { get; set; } 
        public string? obs { get; set; } 
        public string? tema { get; set; } 
        public string? encarregado { get; set; } 
        public DateTime? meta_de_producao { get; set; } 
        public string? caminao { get; set; } 
        public DateTime? data_individual_caminhao { get; set; } 
        public long? numero_de_caminhoes { get; set; } 
    }
}
