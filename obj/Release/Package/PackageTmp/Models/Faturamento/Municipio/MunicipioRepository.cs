using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class MunicipioRepository : IMunicipioRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();

        public IQueryable<Municipio> All
        {
            get { return context.Municipios; }
        }
        
        public Municipio Find(int id)
        {
            return context.Municipios.Find(id);
        }

        public Municipio FindByCodigoIBGE(string idCodigoIBGE)
        {
            return context.Municipios.SingleOrDefault(m => m.CodigoIBGE == idCodigoIBGE);
        }

        public void InsertOrUpdate(Municipio municipio)
        {
            if (municipio.Codigo == default(int))
            {
                // New entity
                context.Municipios.Add(municipio);
            }
            else
            {
                // Existing entity
                context.Entry(municipio).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var municipio = context.Municipios.Find(id);
            context.Municipios.Remove(municipio);
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

    public interface IMunicipioRepository : IDisposable
    {
        IQueryable<Municipio> All { get; }
        Municipio Find(int id);
        void InsertOrUpdate(Municipio empresa);
        void Delete(int id);
        void Save();
    }
}