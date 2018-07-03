using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class EmpresaRepository : IEmpresaRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();

        public IQueryable<Empresa> All
        {
            get { return context.Empresas; }
        }

        public Empresa Find(int id)
        {
            return context.Empresas.Find(id);
        }

        public void InsertOrUpdate(Empresa empresas)
        {
            if (empresas.ID == default(int))
            {
                // New entity
                context.Empresas.Add(empresas);
            }
            else
            {
                // Existing entity
                context.Entry(empresas).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var empresa = context.Empresas.Find(id);
            context.Empresas.Remove(empresa);
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

    public interface IEmpresaRepository : IDisposable
    {
        IQueryable<Empresa> All { get; }
        Empresa Find(int id);
        void InsertOrUpdate(Empresa empresa);
        void Delete(int id);
        void Save();
    }
}