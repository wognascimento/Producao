using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_setor", Schema = "producao")]
    public class SetorProducaoModel
    {
        [Key]
        public long? codigo_setor {set; get;}
        public string? setor {set; get;}
        public string? localizacao {set; get;}
        public string? galpao {set; get;}
        public string? responsavel {set; get;}
        public string? lider {set; get;}
        public string? alterado_por {set; get;}
        public DateTime? data_altera {set; get;}
        public string? login_resp {set; get;}
        public string? relatorio_noturno {set; get;}
        public string? permissao_vaga {set; get;}
        public string? inativo { set; get; }
    }
}
