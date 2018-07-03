using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Charting;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class RelatorioGastosGanhosController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index(String de = null, String ate = null, Int32? projeto = null)
        {
            IQueryable<CONTAS_RECEBER> queryReceita = _db.CONTAS_RECEBER
                .Where(cr => cr.DATA_RECEBIMENTO != null);

            IQueryable<PAGAMENTO> queryPagamento = _db.PAGAMENTO.Where(p => p.DATA_PAGAMENTO != null);



            if (de != null)
            {
                DateTime deDate = DateTime.Parse(de);

                queryReceita = queryReceita.Where(cr => cr.DATA_RECEBIMENTO >= deDate);

                queryPagamento = queryPagamento.Where(p => p.DATA_PAGAMENTO >= deDate);

            }


            if (ate != null)
            {
                DateTime ateDate = DateTime.Parse(ate);

                queryReceita = queryReceita.Where(cr => cr.DATA_RECEBIMENTO <= ateDate);

                queryPagamento = queryPagamento.Where(p => p.DATA_PAGAMENTO <= ateDate);
            }

            if(projeto != null)
            {
                queryReceita = queryReceita.Where(r => r.OSSB1.PROJETO == projeto);

                queryPagamento = queryPagamento.Where(p => p.PROJETO == projeto);
            }

            var receitaPorProjeto = await ((from cr in queryReceita
                                            group cr by cr.OSSB1.PROJETO1 into g
                                            select new
                                            {
                                                PROJETO = g.Key,

                                                VALOR = g.Select(cr => cr.VALOR_LIQUIDO).DefaultIfEmpty().Sum(),
                                            }).ToDictionaryAsync(ks => ks.PROJETO, ks => ks.VALOR));


            var despesaPorProjeto = await (from p in queryPagamento
                                           group p by p.PROJETO1 into g
                                                      select new
                                                      {
                                                          PROJETO = g.Key,
                                                          VALOR = g.Select(p => p.VALOR).DefaultIfEmpty().Sum(),
                                                      })
                                                      .ToDictionaryAsync(d => d.PROJETO, d => d.VALOR);

            var saldoPorProjeto = await (from d in (from p in queryPagamento
                                                    select new
                                                    {
                                                        PROJETO = p.PROJETO1,
                                                        VALOR = -p.VALOR
                                                    }).Concat(from r in queryReceita
                                                                        select new
                                                                        {
                                                                            PROJETO = r.OSSB1.PROJETO1,
                                                                            VALOR = r.VALOR_LIQUIDO
                                                                        })
                                         group d by d.PROJETO into g
                                         select new
                                         {
                                             PROJETO = g.Key,
                                             VALOR = g.Select(d => d.VALOR).DefaultIfEmpty().Sum()
                                         }).ToDictionaryAsync(d => d.PROJETO, d => d.VALOR);

            var items = await (queryPagamento.Select(p => new RelatorioGastoGanhosViewModel.Item() { Ossb= p.OSSB, Descricao = p.DESPESA1.DESCRICAO, Data = (DateTime)p.DATA_PAGAMENTO, Pessoa = p.PESSOA1, Projeto = p.PROJETO1, Valor = -p.VALOR })
                .Concat(queryReceita.Select(r => new RelatorioGastoGanhosViewModel.Item() {Ossb=r.OSSB, Descricao=r.OSSB1.OCORRENCIA, Data = (DateTime)r.DATA_RECEBIMENTO, Pessoa = r.OSSB1.PESSOA, Projeto = r.OSSB1.PROJETO1, Valor = r.VALOR_LIQUIDO }))
                .OrderBy(i => i.Data))
                .ToArrayAsync();


            return View(new RelatorioGastoGanhosViewModel() { Items = items, ReceitaPorProjeto = receitaPorProjeto, DespesaPorProjeto = despesaPorProjeto, SaldoPorProjeto = saldoPorProjeto });
        }

        public async Task<ActionResult> Search()
        {

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO");

            return View();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}