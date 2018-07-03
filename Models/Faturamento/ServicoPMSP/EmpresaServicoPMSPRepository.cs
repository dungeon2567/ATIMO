using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class EmpresaServicoPMSPRepository : IEmpresaServicoPMSPRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();

        public IQueryable<EmpresaServicoPMSP> All
        {
            get { return context.EmpresaServicoPMSP; }
        }

        public IQueryable<EmpresaServicoPMSP> Find(int idEmpresa)
        {
            return context.EmpresaServicoPMSP.AsNoTracking().Where(p => p.Empresa.ID == idEmpresa);
        }

        public EmpresaServicoPMSP Find(int idEmpresa, int idServico)
        {
            return context.EmpresaServicoPMSP.Find(idEmpresa ,idServico);
        }

        public void InsertOrUpdate(EmpresaServicoPMSP servico)
        {
            if (!context.EmpresaServicoPMSP.AsNoTracking().Where(p => p.Empresa.ID == servico.Empresa.ID && p.ServicoPMSP.Codigo == servico.ServicoPMSP.Codigo).Any())
            {
                // New entity
                context.EmpresaServicoPMSP.Add(servico);
            }
            else
            {
                // Existing entity
                context.Entry(servico).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int idEmpresa, string codServico)
        {
            var servico = context.EmpresaServicoPMSP.Find(idEmpresa, codServico);
            context.EmpresaServicoPMSP.Remove(servico);
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

    public interface IEmpresaServicoPMSPRepository : IDisposable
    {
        IQueryable<EmpresaServicoPMSP> All { get; }
        EmpresaServicoPMSP Find(int idEmpresa, int idServico);
        IQueryable<EmpresaServicoPMSP> Find(int idEmpresa);
        void InsertOrUpdate(EmpresaServicoPMSP empresa);
        void Delete(int idEmpresa, string codServico);
        void Save();
    }
}