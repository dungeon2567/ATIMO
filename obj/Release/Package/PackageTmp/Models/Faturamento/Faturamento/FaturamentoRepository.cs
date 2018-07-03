using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class FaturamentoRepository : IFaturamentoRepository
    {
        FATURAMENTOEntities context = new FATURAMENTOEntities();

        public IQueryable<Faturamento> All
        {
            get { return context.Faturamentos; }
        }

        public IQueryable<Faturamento> AllAsNoTracking
        {
            get { return context.Faturamentos.AsNoTracking(); }
        }

        public Faturamento Find(int id)
        {
            return context.Faturamentos.Find(id);
        }

        public Faturamento FindAsNoTracking(int id)
        {
            context.Configuration.ProxyCreationEnabled = false;
            Faturamento l_Faturamento = context.Faturamentos.AsNoTracking().Where(p => p.ID == id).FirstOrDefault();
            l_Faturamento.Retencoes = context.FaturamentoRetencoes.AsNoTracking().Where(p => p.FaturamentoID == id).ToList();
            context.Configuration.ProxyCreationEnabled = true;
            
            return l_Faturamento;
        }

        public void InsertOrUpdate(Faturamento faturamento)
        {
            faturamento.Cliente = null;
            faturamento.OSSB = null;
            faturamento.ServicoPMSP = null;
            faturamento.EmpresaEmissao = null;
            List<FaturamentoRetencoes> l_Retencoes = new List<FaturamentoRetencoes>();

            if (faturamento.ID == default(int))
            {
                // New entity
                context.Faturamentos.Add(faturamento);
            }
            else
            {
                if (faturamento.Retencoes != null)
                {
                    if (faturamento.Retencoes.Any())
                    {
                        l_Retencoes = new List<FaturamentoRetencoes>(faturamento.Retencoes);
                        faturamento.Retencoes = null;
                    }
                }
                // Existing entity
                context.Entry(faturamento).State = System.Data.Entity.EntityState.Modified;

                faturamento.Retencoes = l_Retencoes;
            }

            //Save Functionalities Changes
            List<FaturamentoRetencoes> l_RetencoesToAdd;

            if (faturamento.Retencoes != null)
                l_RetencoesToAdd = new List<FaturamentoRetencoes>(faturamento.Retencoes.ToList());
            else
            {
                l_RetencoesToAdd = new List<FaturamentoRetencoes>();
                faturamento.Retencoes = new List<FaturamentoRetencoes>();
            }

            if(faturamento.ID != 0)
            { 
                foreach (FaturamentoRetencoes item in context.FaturamentoRetencoes.Where(p => p.FaturamentoID == faturamento.ID))
                {
                    context.FaturamentoRetencoes.Remove(item);
                }
            }

            //if (faturamento.Retencoes.Count() > 0 && faturamento.ID != 0)
            if (l_RetencoesToAdd.Count() > 0 && faturamento.ID != 0)
            {
                foreach (FaturamentoRetencoes item in l_RetencoesToAdd)
                {
                    //if (context.FaturamentoRetencoes.Where(p => p.FaturamentoID == faturamento.ID &&
                    //                                            p.TipoImpostoID == item.TipoImpostoID).Any())
                    //    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    //else
                    context.FaturamentoRetencoes.Add(item);
                }
            }

        }

        public void Delete(int id)
        {
            //Exclui os itens filhos antes de excluir o item pai
            foreach (FaturamentoRetencoes item in context.FaturamentoRetencoes.Where(p => p.FaturamentoID == id))
            {
                context.FaturamentoRetencoes.Remove(item);
            }

            var faturamento = context.Faturamentos.Find(id);
            context.Faturamentos.Remove(faturamento);
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

    public interface IFaturamentoRepository : IDisposable
    {
        IQueryable<Faturamento> All { get; }
        IQueryable<Faturamento> AllAsNoTracking { get; }
        Faturamento Find(int id);
        Faturamento FindAsNoTracking(int id);
        void InsertOrUpdate(Faturamento entity);
        void Delete(int id);
        void Save();
    }
}