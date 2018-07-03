using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class EmailSMTPRepository : IEmailSMTPRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();
        
        public EmailSMTPRepository()
        {
            
        }

        public IQueryable<EmailSMTP> All
        {
            get { return context.EmailSMTP; }
        }

        public EmailSMTP Find(int id)
        {
            return context.EmailSMTP.Find(id);
        }

        public void InsertOrUpdate(EmailSMTP email)
        {
            if (!context.EmailSMTP.AsNoTracking().Where(p => p.ID == email.ID).Any())
            {
                // New entity
                context.EmailSMTP.Add(email);
            }
            else
            {
                // Existing entity
                context.Entry(email).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var servico = context.EmailSMTP.Find(id);
            context.EmailSMTP.Remove(servico);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }

    public interface IEmailSMTPRepository : IDisposable
    {
        IQueryable<EmailSMTP> All { get; }
        EmailSMTP Find(int id);
        void InsertOrUpdate(EmailSMTP empresa);
        void Delete(int id);
        void Save();
    }
}