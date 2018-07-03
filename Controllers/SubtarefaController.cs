using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;

namespace Atimo.Controllers
{
    public class SubtarefaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Create(int id = 0)
        {
            return View(new SUBTAREFA() { TAREFA = id });
        }

        //
        // POST: /OssbMaterial/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SUBTAREFA subtarefa)
        {
            if (string.IsNullOrEmpty(subtarefa.DESCRICAO))
                ModelState.AddModelError(string.Empty, "informe uma descrição!");


            if (ModelState.IsValid)
            {
                subtarefa.SITUACAO = "P";

                _db.SUBTAREFA.Add(subtarefa);

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Tarefa");
            }

            return View(subtarefa);
        }

        public async Task<JsonResult> MarcarComoConcluido(int id)
        {
            var st = await _db
                .SUBTAREFA
                .FirstOrDefaultAsync(v => v.ID == id);

            st.SITUACAO = "C";

            _db.Entry(st).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> MarcarComoCancelado(int id)
        {
            var st = await _db.SUBTAREFA.FirstOrDefaultAsync(v => v.ID == id);

            st.SITUACAO = "I";

            _db.Entry(st).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Json("", JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();

            base.Dispose(disposing);
        }
    }
}