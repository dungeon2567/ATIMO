using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System.Drawing;

namespace Atimo.Controllers
{
    public class CanceladaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /Cancelada/


        public async Task<ActionResult> Index()
        {
                var ossb = await _db.OSSB.Include(o => o.CONTRATO1)
                    .Include(o => o.PESSOA)
                    .Include(o => o.PROJETO1)
                    .Where(o => o.SITUACAO == "C")
                    .ToArrayAsync();

                return View(ossb);         
        }

        //
        // GET: /Cancelada/Edit/5

        public ActionResult Edit(int id = 0)
        {
          
                var ossb = _db.OSSB.Find(id);

                if (ossb == null)
                {
                    return HttpNotFound();
                }

                ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", ossb.PROJETO);
                ViewBag.CLIENTE = new SelectList(_db.PESSOA.Where(p => p.SITUACAO == "A").Where(p => p.CLIENTE == 1), "ID", "RAZAO", ossb.CLIENTE);
                ViewBag.CONTRATO = new SelectList(_db.CONTRATO.Where(c => c.SITUACAO == "A"), "ID", "DESCRICAO", ossb.CONTRATO);


                return View(ossb);

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}