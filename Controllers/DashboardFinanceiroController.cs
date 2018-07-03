using ATIMO.Models;
using ATIMO.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System;
using ATIMO;

namespace Atimo.Controllers
{
    public class DashboardFinanceiroController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index()
        {
            if (Session.IsFuncionario())
            {
                DateTime startingDate = DateTime.Now.Date.AddMonths(-12);

                startingDate = new DateTime(startingDate.Year, startingDate.Month, 1);

                var saida = (await (from cp in _db.CONTAS_PAGAS
                                    where cp.DATA_PAGAMENTO >= startingDate

                                    group cp by new { YEAR = cp.DATA_PAGAMENTO.Value.Year, MONTH = cp.DATA_PAGAMENTO.Value.Month } into g
                                    select new
                                    {
                                        MONTH = g.Key.MONTH,
                                        YEAR = g.Key.YEAR,
                                        VALUE = g.Select(pt => pt.VALOR).DefaultIfEmpty().Sum()
                                    })
                                   .OrderBy(k => k.YEAR)
                                   .ThenBy(k => k.MONTH)
                                   .ToArrayAsync())
                                   .Select(k => new KeyValuePair<DateTime, Decimal>(new DateTime(k.YEAR, k.MONTH, 1), k.VALUE));


                var entrada = (await (from cr in _db.CONTAS_RECEBER
                                      where cr.DATA_RECEBIMENTO != null
                                      group cr by new { MONTH = ((DateTime)cr.DATA_RECEBIMENTO).Month, YEAR = ((DateTime)cr.DATA_RECEBIMENTO).Year } into g
                                      select new
                                      {
                                          MONTH = g.Key.MONTH,
                                          YEAR = g.Key.YEAR,
                                          VALUE = g.Select(cr => cr.VALOR_LIQUIDO).Sum()
                                      }
                    ).OrderBy(k => k.YEAR * 12 + k.MONTH)
                    .ToArrayAsync())
                    .Select(k => new KeyValuePair<DateTime, Decimal>(new DateTime(k.YEAR, k.MONTH, 1), k.VALUE));


                return View(new DashboardFinanceiroViewModel() { Saida = saida, Entrada = entrada });
            }

            return RedirectToAction("", "");
        }
    }
}
