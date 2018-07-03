using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ATIMO.Models.Faturamento
{
    [Table("EMAIL_TEMPLATE")]
    public class EmailTemplate
    {

        public enum enmTemplate
        {
            enmEmailNotaFiscalParaCliente = 1
        }

        public EmailTemplate()
        {
            NomeTemplate = string.Empty;
            CorpoTemplate = string.Empty;
            TituloTemplate = string.Empty;
            SenderNameTemplate = string.Empty;
        }

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("NOME_TEMPLATE")]
        public string NomeTemplate { get ; set ; }

        [Column("CORPO_TEMPLATE")]
        public string CorpoTemplate { get; set; }

        [Column("TITULO_TEMPLATE")]
        public string TituloTemplate { get; set; }

        [Column("SENDER_NAME_TEMPLATE")]
        public string SenderNameTemplate { get; set; }
        
    }
}