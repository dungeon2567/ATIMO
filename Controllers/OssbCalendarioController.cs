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
    public class OssbCalendarioController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index(String mes_ano = null, string[] tipo = null, string[] situacao = null, int? projeto = null)
        {
            if (mes_ano == null)
                mes_ano = DateTime.Today.ToString("MM/yyyy");


            Int32 MES = Int32.Parse(mes_ano.Substring(0, 2));

            Int32 ANO = Int32.Parse(mes_ano.Substring(3, 4));

            ViewBag.MES_ANO = mes_ano;

            DateTime start = new DateTime(ANO, MES, 1);

            DateTime end = start.AddMonths(1);

            var query = from ocl in _db.OSSB_CHECK_LIST
                        where ocl.VISITADO == null && ocl.AGENDADO >= start && ocl.AGENDADO <= end
                        select ocl;

            if(situacao != null)
            {
                query = query.Where(ocl => situacao.Contains(ocl.OSSB1.SITUACAO));
            }

            if (tipo != null)
            {
                query = query.Where(ocl => tipo.Contains(ocl.OSSB1.TIPO));
            }

            if(projeto != null)
            {
                query = query.Where(ocl => projeto == ocl.OSSB1.PROJETO);
            }

            var items = await (from ocl in query
                               group ocl by DbFunctions.TruncateTime(ocl.AGENDADO) into g
                               select new
                               {
                                   DATA = (DateTime)g.Key,
                                   CHECKLISTS = g.Select(ocl => ocl)
                               }).ToDictionaryAsync(k => k.DATA, k => k.CHECKLISTS);


            DateTime first = start;

            while (first.DayOfWeek != DayOfWeek.Sunday)
            {
                first = first.AddDays(-1);
            }

            ViewBag.FIRST_SUNDAY = first;

            ViewBag.START = start;

            ViewBag.END = end;

            ViewBag.PROJETO = new SelectList(await _db.PROJETO
                .ToArrayAsync(), "ID", "DESCRICAO");


            return View(items);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
