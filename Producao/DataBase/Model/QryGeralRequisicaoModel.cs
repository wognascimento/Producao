using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Keyless]
    [Table("qry_geral_requisicao", Schema = "producao")]
    public class QryGeralRequisicaoModel
    {
        public long? num_requisicao { get; set; }
        public long? id_aprovado { get; set; }
        public string? cliente { get; set; }
        public string? barcode { get; set; }
        public long? codigo { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        //public string? descricao { get; set; }
        //public string? descricao_adicional { get; set; }
        //public string? complementoadicional { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }
        public string? retorno { get; set; }
    }
}
