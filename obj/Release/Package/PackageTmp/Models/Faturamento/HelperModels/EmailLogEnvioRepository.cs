using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class EmailLogEnvioRepository : IEmailLogEnvioRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();
        
        public EmailLogEnvioRepository()
        {
            
        }

        public IQueryable<EmailLogEnvio> All
        {
            get { return context.EmailLogEnvio; }
        }

        public EmailLogEnvio Find(int id)
        {
            return context.EmailLogEnvio.Find(id);
        }
        
        public void InsertOrUpdate(EmailLogEnvio email)
        {
            if (!context.EmailLogEnvio.AsNoTracking().Where(p => p.ID == email.ID).Any())
            {
                // New entity
                context.EmailLogEnvio.Add(email);
            }
            else
            {
                // Existing entity
                context.Entry(email).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var servico = context.EmailLogEnvio.Find(id);
            context.EmailLogEnvio.Remove(servico);
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

    public interface IEmailLogEnvioRepository : IDisposable
    {
        IQueryable<EmailLogEnvio> All { get; }
        EmailLogEnvio Find(int id);
        void InsertOrUpdate(EmailLogEnvio empresa);
        void Delete(int id);
        void Save();
    }
}