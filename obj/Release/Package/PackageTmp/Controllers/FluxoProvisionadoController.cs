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
    public class FluxoProvisionadoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index(string[] situacao = null, string[] tipo = null, int? projeto = null)
        {
            var query = from s in _db.OSSB
                        select new
                        {
                            OSSB = s,
                            VALOR = s.OSSB_SERVICO.Select(serv => (serv.VALOR_MA_BDI + serv.VALOR_MO_BDI) * serv.QUANTIDADE).DefaultIfEmpty().Sum() - s.CONTAS_RECEBER.Select(cr => cr.VALOR_LIQUIDO).DefaultIfEmpty().Sum()
                        };

            if (situacao != null)
            {
                query = query.Where(os => situacao.Contains(os.OSSB.SITUACAO));
            }

            if (tipo != null)
            {
                query = query.Where(os => tipo.Contains(os.OSSB.TIPO));

            }

            if (projeto != null)
            {
                query = query.Where(os => os.OSSB.PROJETO == projeto);
            }

            ViewBag.PROJETO = new SelectList(await _db.PROJETO

                .ToArrayAsync(), "ID", "DESCRICAO");

            return View(await query.Select(q => q.VALOR).DefaultIfEmpty().SumAsync());
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
