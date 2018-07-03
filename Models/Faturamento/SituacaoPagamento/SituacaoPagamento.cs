using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    [Table("SITUACAOPAGTO")]
    public class SituacaoPagamento
    {
        [Column("COD_SITUACAO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        
        [Column("DESCR_SITUACAO")]
        public string Descricao { get; set; }

    }
}