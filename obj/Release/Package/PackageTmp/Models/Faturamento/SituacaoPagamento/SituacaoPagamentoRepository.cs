using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class SituacaoPagamentoRepository : ISituacaoPagamentoRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();

        public IQueryable<SituacaoPagamento> All
        {
            get { return context.SituacoesPagamento; }
        }

        public SituacaoPagamento FindAsNoTracking(int id)
        {
            return context.SituacoesPagamento.AsNoTracking().Where(p => p.ID == id).FirstOrDefault();
        }

        public SituacaoPagamento Find(int id)
        {
            return context.SituacoesPagamento.Find(id);
        }

        public void InsertOrUpdate(SituacaoPagamento entity)
        {
            if (entity.ID != 0)
            {
                // New entity
                context.SituacoesPagamento.Add(entity);
            }
            else
            {
                // Existing entity
                context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var entity = this.Find(id);
            context.SituacoesPagamento.Remove(entity);
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

    public interface ISituacaoPagamentoRepository : IDisposable
    {
        IQueryable<SituacaoPagamento> All { get; }
        SituacaoPagamento Find(int Id);
        SituacaoPagamento FindAsNoTracking(int Id);
        void InsertOrUpdate(SituacaoPagamento entity);
        void Delete(int Id);
        void Save();
    }
}