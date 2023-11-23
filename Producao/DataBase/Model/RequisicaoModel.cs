using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("t_requisicao", Schema = "producao")]
    public class RequisicaoModel
    {
        [Key]
        public long? num_requisicao {set; get;}
        public long? num_os_servico {set; get;}
        public DateTime? data {set; get;}
        public string? alterado_por {set; get;}
        public bool? concluida { set; get; }
    }
}
