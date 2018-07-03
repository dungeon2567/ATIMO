using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using ATIMO.ViewModel;
using System.Data.Entity.Migrations;

namespace Atimo.Controllers
{
    public class ClienteSatisfacaoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();



        public async Task<ActionResult> Edit(Int32 id)
        {
            var satisfacao = await _db
                .CLIENTE_SATISFACAO
               .FirstOrDefaultAsync(cs => cs.OSSB == id);

            if (satisfacao == null)
            {
                satisfacao = new CLIENTE_SATISFACAO()
                {
                    OSSB = id,
                };
            };

            return View(satisfacao);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CLIENTE_SATISFACAO satisfacao)
        {

            if (ModelState.IsValid)
            {
                if (satisfacao.POSTURA_EQUIPE == 0 && satisfacao.SATISFACAO_SERVICO == 0 && satisfacao.CUMPRIMENTO_HORARIO == 0 && satisfacao.ASSINATURA == null && satisfacao.COBRANCA == null)
                    _db.CLIENTE_SATISFACAO.Remove(satisfacao);
                else
                    _db.CLIENTE_SATISFACAO.AddOrUpdate(satisfacao);
                

                await _db.SaveChangesAsync();

                return Redirect("/VisitaInicial/Index/?num=" + satisfacao.OSSB);
            }


            return View(satisfacao);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}