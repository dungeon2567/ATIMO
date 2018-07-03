using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();
        
        public EmailTemplateRepository()
        {
            
        }

        public IQueryable<EmailTemplate> All
        {
            get { return context.EmailTemplate; }
        }

        public EmailTemplate Find(int id)
        {
            return context.EmailTemplate.Find(id);
        }

        public EmailTemplate Find(EmailTemplate.enmTemplate aenmTemplate)
        {
            return context.EmailTemplate.Find(Convert.ToInt32(aenmTemplate));
        }

        public void InsertOrUpdate(EmailTemplate email)
        {
            if (!context.EmailTemplate.AsNoTracking().Where(p => p.ID == email.ID).Any())
            {
                // New entity
                context.EmailTemplate.Add(email);
            }
            else
            {
                // Existing entity
                context.Entry(email).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var servico = context.EmailTemplate.Find(id);
            context.EmailTemplate.Remove(servico);
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

    public interface IEmailTemplateRepository : IDisposable
    {
        IQueryable<EmailTemplate> All { get; }
        EmailTemplate Find(int id);
        EmailTemplate Find(EmailTemplate.enmTemplate aenmTemplate);
        void InsertOrUpdate(EmailTemplate empresa);
        void Delete(int id);
        void Save();
    }
}