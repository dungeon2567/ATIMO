using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class TipoImpostoRepository : ITipoImpostoRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();

        public IQueryable<TipoImposto> All
        {
            get { return context.TiposImposto; }
        }
        
        public TipoImposto Find(int id)
        {
            return context.TiposImposto.Find(id);
        }
        
        public void InsertOrUpdate(TipoImposto entity)
        {
            if (entity.ID == default(int))
            {
                // New entity
                context.TiposImposto.Add(entity);
            }
            else
            {
                // Existing entity
                context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var entity = context.TiposImposto.Find(id);
            context.TiposImposto.Remove(entity);
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

    public interface ITipoImpostoRepository : IDisposable
    {
        IQueryable<TipoImposto> All { get; }
        TipoImposto Find(int id);
        void InsertOrUpdate(TipoImposto empresa);
        void Delete(int id);
        void Save();
    }
}