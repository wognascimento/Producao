﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao
{
    [Table("tbl_produto_os", Schema = "producao")]
    public class ProdutoOsModel
    {
        [Key]
        public long? num_os_produto {set; get;}
        public string? tipo {set; get;}
        public string? planilha {set; get;}
        public long? cod_produto {set; get;}
        public long? cod_desc_adicional {set; get;}
        public long? cod_compl_adicional {set; get;}
        public double? quantidade {set; get;}
        public DateTime? data_emissao {set; get;}
        public string? responsavel_emissao {set; get;}
        public long? id_modelo {set; get;}
        public string? solicitado_por {set; get;}
        public long? codigo_saida { set; get; }
        public string? cliente { set; get; }

        // Propriedade de navegação para as observações associadas a este produto
        public ICollection<ObsOsModel>? Observacoes { get; set; } = [];
    }
}
