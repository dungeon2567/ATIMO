using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class FaturamentoDescricaoRepository : IFaturamentoDescricaoRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();

        public IQueryable<FaturamentoDescricao> All
        {
            get { return context.FaturamentoDescricoes; }
        }

        public IQueryable<FaturamentoDescricao> AllAsNoTracking
        {
            get { return context.FaturamentoDescricoes.AsNoTracking(); }
        }

        public FaturamentoDescricao Find(int id)
        {
            return context.FaturamentoDescricoes.Find(id);
        }

        public void InsertOrUpdate(FaturamentoDescricao faturamentoDescricao)
        {
            if (faturamentoDescricao.ID == default(int))
            {
                // New entity
                context.FaturamentoDescricoes.Add(faturamentoDescricao);
            }
            else
            {
                // Existing entity
                context.Entry(faturamentoDescricao).State = System.Data.Entity.EntityState.Modified;
            }
            
        }

        public void Delete(int id)
        {
            var faturamentoDescricao = context.FaturamentoDescricoes.Find(id);
            context.FaturamentoDescricoes.Remove(faturamentoDescricao);
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

    public interface IFaturamentoDescricaoRepository : IDisposable
    {
        IQueryable<FaturamentoDescricao> All { get; }
        IQueryable<FaturamentoDescricao> AllAsNoTracking { get; }
        FaturamentoDescricao Find(int id);
        void InsertOrUpdate(FaturamentoDescricao entity);
        void Delete(int id);
        void Save();
    }
}