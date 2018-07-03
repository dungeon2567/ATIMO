using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ATIMO;
using System.Web;

namespace Atimo.Controllers
{
    public class ParcelamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Display(int id)
        {
            var ossb = await _db.OSSB
                .Include(os => os.CONTRATO1)
                         .Include(os => os.LOJA1)
                         .Include(os => os.PESSOA)
                         .Include(os => os.PROJETO1)
                         .Include(os => os.RESPONSAVEL1)
                         .Include(os => os.OSSB_SERVICO)
                         .Include(os => os.OSSB_SERVICO.Select(oss => oss.OSSB_SERVICO_TERCEIRO))
                         .Include(os => os.OSSB_SERVICO.Select(oss => oss.OSSB_SERVICO_TERCEIRO.Select(ost => ost.PAGAMENTO1)))
                         .Include(os => os.OSSB_COMUNICACAO)
                         .Include(os => os.OSSB_COMUNICACAO.Select(osc => osc.PESSOA1))
                         .Include(os => os.CONTAS_RECEBER)
                .FirstOrDefaultAsync(os => os.ID == id);

            if (ossb == null)
                return HttpNotFound();

            ViewBag.CONTA_BANCARIA = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO");

            return View(ossb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MudarParaCobranca(CONTAS_RECEBER cr)
        {

            if (cr.DATA_RECEBIMENTO != null && cr.DATA_RECEBIMENTO < DateTime.Today.AddDays(-1))
                {
                    return Content("A data de recebimento não pode ser inferior a data atual.");
                };

                var ossb = await _db.OSSB
                    .FirstOrDefaultAsync(os => os.ID == cr.OSSB);

                if (ossb == null)
                    return HttpNotFound();

                ossb.SITUACAO = "K";

                _db.CONTAS_RECEBER.Add(cr);

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "ContasReceber");
            
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}