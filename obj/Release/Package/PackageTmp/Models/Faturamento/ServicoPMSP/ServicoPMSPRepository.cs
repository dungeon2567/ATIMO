using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class ServicoPMSPRepository : IServicoPMSPRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();
        private readonly IEmpresaServicoPMSPRepository m_empresaservicoPMSPRepository;

        public ServicoPMSPRepository()
        {
            m_empresaservicoPMSPRepository = new EmpresaServicoPMSPRepository();
        }

        public IQueryable<ServicoPMSP> All
        {
            get { return context.ServicosPMSP; }
        }

        public ServicoPMSP Find(string id)
        {
            return context.ServicosPMSP.Find(id);
        }
        
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <returns></returns>
        public List<ServicoPMSP> FindByEmpresa(int id)
        {
            List<ServicoPMSP> lobjRet = new List<ServicoPMSP>();
            IQueryable<EmpresaServicoPMSP> lobjEmpServ = m_empresaservicoPMSPRepository.All.Where(p => p.Empresa.ID == id);

            foreach (EmpresaServicoPMSP item in lobjEmpServ.All<EmpresaServicoPMSP>()
                lobjRet.Add(item.ServicoPMSP);

            lobjRet.Sort((x, y) => x.Codigo.CompareTo(y.Codigo));

            return lobjRet;
        }
        */

        public void InsertOrUpdate(ServicoPMSP servico)
        {
            if (!context.ServicosPMSP.AsNoTracking().Where(p => p.Codigo == servico.Codigo).Any())
            {
                // New entity
                context.ServicosPMSP.Add(servico);
            }
            else
            {
                // Existing entity
                context.Entry(servico).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var servico = context.ServicosPMSP.Find(id);
            context.ServicosPMSP.Remove(servico);
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

    public interface IServicoPMSPRepository : IDisposable
    {
        IQueryable<ServicoPMSP> All { get; }
        ServicoPMSP Find(string id);
        void InsertOrUpdate(ServicoPMSP empresa);
        void Delete(int id);
        void Save();
        //List<ServicoPMSP> FindByEmpresa(int id);
    }
}