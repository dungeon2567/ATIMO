using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    public class FinalizadaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /Finalizada/

        public ActionResult Index()
        {
            if (Session.IsFuncionario())
            {
                var ossb = _db.OSSB.Include(o => o.CONTRATO1).Include(o => o.PESSOA).Include(o => o.PROJETO1).Where(o => o.SITUACAO == "F");
                return View(ossb.ToList());
            }
            else
                return RedirectToAction("", "");
        }

        //
        // GET: /Finalizada/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {

                var ossb = _db.OSSB.Find(id);

                if (ossb == null)
                {
                    return HttpNotFound();
                }

                ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", ossb.PROJETO);
                ViewBag.CLIENTE = new SelectList(_db.PESSOA.Where(p => p.SITUACAO == "A")
                    .Where(p => p.CLIENTE == 1), "ID", "RAZAO", ossb.CLIENTE);

                ViewBag.CONTRATO = new SelectList(_db.CONTRATO.Where(c => c.SITUACAO == "A"), "ID", "DESCRICAO", ossb.CONTRATO);

                return View(ossb);
            }
            else
                return RedirectToAction("", "");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}