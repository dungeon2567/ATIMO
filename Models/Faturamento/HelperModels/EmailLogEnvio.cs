using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ATIMO.Models.Faturamento
{
    [Table("EMAIL_LOG_ENVIO")]
    public class EmailLogEnvio
    {

        public EmailLogEnvio()
        {
            SenderName = string.Empty;
            EmailDestino = string.Empty;
            Titulo = string.Empty;
            CorpoEmail = string.Empty;
            HTML = string.Empty;
            ErroEnvio = string.Empty;
        }

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("SENDER_NAME")]
        public string SenderName { get; set; }

        [Column("EMAIL_DESTINO")]
        public string EmailDestino { get; set; }

        [Column("TITULO")]
        public string Titulo { get; set; }

        [Column("CORPO_EMAIL")]
        public string CorpoEmail { get; set; }

        [Column("HTML")]
        public string HTML { get; set; }

        [Column("DATA_CADASTRADA")]
        public DateTime DataCadastrada { get; set; }

        [Column("DATA_ENVIADA")]
        public DateTime? DataEnviada { get; set; }

        [Column("ERRO_ENVIO")]
        public string ErroEnvio { get; set; }


        internal MailMessage getAsMailMessage()
        {
            MailMessage lobjRet = new MailMessage();

            lobjRet.Body = CorpoEmail;
            lobjRet.IsBodyHtml = (HTML == "S");
            lobjRet.Subject = Titulo;
            string[] lstrTo = EmailDestino.Split(';');

            foreach (string end_mail in lstrTo)
            {
                try
                {
                    lobjRet.To.Add(new MailAddress(end_mail));
                }
                catch (Exception)
                {

                }
            }

            /*
            if (ArquivoAnexo01 != string.Empty)
                lobjRet.Attachments.Add(new Attachment(ArquivoAnexo01));

            if (ArquivoAnexo02 != string.Empty)
                lobjRet.Attachments.Add(new Attachment(ArquivoAnexo02));

            if (ArquivoAnexo03 != string.Empty)
                lobjRet.Attachments.Add(new Attachment(ArquivoAnexo03));

            if (ArquivoAnexo04 != string.Empty)
                lobjRet.Attachments.Add(new Attachment(ArquivoAnexo04));

            if (ArquivoAnexo05 != string.Empty)
                lobjRet.Attachments.Add(new Attachment(ArquivoAnexo05));
                */
            return lobjRet;
        }
    }

    /*
    [Table("EMAIL_LOG_ENVIO_ANEXO")]
    public class EmailLogEnvioAnexo
    {

        public EmailLogEnvioAnexo()
        {
            SenderName = string.Empty;
            EmailDestino = string.Empty;
            Titulo = string.Empty;
            CorpoEmail = string.Empty;
            HTML = string.Empty;
            ErroEnvio = string.Empty;
        }

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("SENDER_NAME")]
        public string SenderName { get; set; }

        [Column("EMAIL_DESTINO")]
        public string EmailDestino { get; set; }

        [Column("TITULO")]
        public string Titulo { get; set; }

        [Column("CORPO_EMAIL")]
        public string CorpoEmail { get; set; }

        [Column("HTML")]
        public string HTML { get; set; }

        [Column("DATA_CADASTRADA")]
        public DateTime DataCadastrada { get; set; }

        [Column("DATA_ENVIADA")]
        public DateTime? DataEnviada { get; set; }

        [Column("ERRO_ENVIO")]
        public string ErroEnvio { get; set; }

    }
    */
}