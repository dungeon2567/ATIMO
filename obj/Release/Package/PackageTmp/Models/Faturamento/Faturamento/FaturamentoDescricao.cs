using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    [Table("FAT_DESCRICAO_FATURAMENTO")]
    public class FaturamentoDescricao
    {
        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("TITULO_DESCRICAO")]
        public string TituloDescricao { get; set; }

        [Column("DESCRICAO")]
        public string Descricao { get; set; }

        [Column("QUANDO_UTILIZAR")]
        public string QuandoUtilizar { get; set; }

        
        [Column("DATA_CADASTRO")]
        public DateTime DataCadastro { get; set; }
        
        [Column("ID_PESSOA_CADASTRO")]
        public int PessoaCadastroID { get; set; }
        [NotMapped]
        public virtual PESSOA PessoaCadastro { get; set; }
    }
}