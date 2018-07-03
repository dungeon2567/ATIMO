using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ATIMO.Models.Faturamento
{
    [Table("EMAIL_SMTP")]
    public class EmailSMTP
    {

        public EmailSMTP()
        {
            NomeConta = string.Empty;
            EnderecoSMTP = string.Empty;
            EmailOrigem = string.Empty;
            Senha = string.Empty;
        }

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("NOME_CONTA")]
        public string NomeConta { get ; set ; }

        [Column("ENDERECO_SMTP")]
        public string EnderecoSMTP { get; set; }

        [Column("EMAIL_ORIGEM")]
        public string EmailOrigem { get; set; }

        [Column("SENHA")]
        public string Senha { get; set; }

        [Column("PORTA")]
        public int Porta { get; set; }
        
    }
}